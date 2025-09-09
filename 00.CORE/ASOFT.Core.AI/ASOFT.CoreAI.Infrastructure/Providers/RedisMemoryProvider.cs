using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using System.Text.Json;

public class RedisMemoryProvider : IRedisMemoryProvider
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private const string MemoryKeyPrefix = "semantic_memory:";

    // Constructor public nhận IConnectionMultiplexer từ DI
    public RedisMemoryProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _database = _connectionMultiplexer.GetDatabase();
    }

    private async Task<string> CreateAsync(CustomMemoryRecord record, CancellationToken cancellationToken = default)
    {
        var redisKey = $"{MemoryKeyPrefix}{record.CollectionName}:{record.Key}";
        var value = JsonSerializer.Serialize(record);
        await _database.StringSetAsync(redisKey, value);
        return record.Key;
    }

    public async Task<IEnumerable<CustomMemoryRecord>> GetByUserIdAsync(string collectionName, string agentCode, string userId, CancellationToken cancellationToken = default)
    {
        var server = GetServer();

        // Đảm bảo phân cách giữa userId và agentCode
        var pattern = $"{MemoryKeyPrefix}{collectionName}:*_{userId}_{agentCode}";

        var keys = server.Keys(pattern: pattern).ToArray();

        if (keys.Length == 0)
            return Enumerable.Empty<CustomMemoryRecord>();

        var values = await _database.StringGetAsync(keys);

        var result = new List<CustomMemoryRecord>();

        foreach (var val in values)
        {
            if (!val.HasValue) continue;

            try
            {
                var record = System.Text.Json.JsonSerializer.Deserialize<CustomMemoryRecord>(val);
                if (record != null)
                    result.Add(record);
            }
            catch
            {
                // Log hoặc xử lý lỗi deserialize nếu cần
                continue;
            }
        }

        return result.OrderByDescending(r => r.CreatedAt).Take(5).Reverse();
    }

    private IServer GetServer()
    {
        var endpoint = _connectionMultiplexer.GetEndPoints().First();
        return _connectionMultiplexer.GetServer(endpoint);
    }

    public async Task<bool> SaveUserChatToVectorDbAsync(CustomMemoryRecord record, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(record.Data) || string.IsNullOrWhiteSpace(record.Prompt))
        {
            Console.WriteLine("❌ Dữ liệu hoặc Prompt bị thiếu.");
            return false;
        }
        try
        {
            await CreateAsync(record, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi khi lưu vào Vector Database: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IsCheckExistKeyAsync(string cacheKey)
    {
        var cachedKey = await _database.StringGetAsync(cacheKey);
        return !cachedKey.IsNullOrEmpty;
    }

    public async Task<string> GetApiKeyAsync(string cacheKey)
    {
        var cachedKey = await _database.StringGetAsync(cacheKey);
        return cachedKey;
    }

    public async Task<string> SaveAPIKeyAsync(string cacheKey, ModelAIChatConfig config, double day)
    {
        var redisKey = $"{cacheKey}";
        var value = JsonSerializer.Serialize(config);
        var result = await _database.StringSetAsync(redisKey, value, TimeSpan.FromDays(day));
        return result.ToString();
    }

    public async Task<ModelAIChatConfig?> GetOpenAIChatConfigAsync(string cacheKey)
    {
        var cachedJson = await _database.StringGetAsync(cacheKey);
        if (cachedJson.IsNullOrEmpty)
        {
            return null; // hoặc throw, tùy xử lý
        }

        try
        {
            var config = JsonSerializer.Deserialize<ModelAIChatConfig>(cachedJson);
            return config;
        }
        catch (JsonException ex)
        {
            // Xử lý lỗi nếu JSON không hợp lệ
            Console.WriteLine($"Lỗi deserialize JSON: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CollectionExistsAsync(string indexName)
    {
        try
        {
            await this._database.FT().InfoAsync(indexName).ConfigureAwait(false);
            return true;
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Unknown index name"))
        {
            return false;
        }
    }

    /// <summary>
    /// Tạo chỉ mục cho Redis Vector Store với cấu trúc JSON.
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="typeContent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CreateIndexAsync(string operationName, string indexName, CancellationToken cancellationToken = default)
    {
        string prefix = $"{indexName}:";
        await _database.ExecuteAsync(operationName, new object[]
        {
         indexName,
         "ON", "JSON",
         "PREFIX", "1", prefix,
         "SCHEMA",
         "$.Text", "AS", "Text", "TEXT",
         "$.ReferenceDescription", "AS", "ReferenceDescription", "TEXT",
         "$.ReferenceLink", "AS", "ReferenceLink", "TEXT",
         "$.EmbeddingVector", "AS", "EmbeddingVector", "VECTOR", "FLAT", "6",
         "TYPE", "FLOAT32",
         "DIM", "1536",
         "DISTANCE_METRIC", "COSINE"
        });
    }

    /// <summary>
    /// Lưu thông tin TextSnippet vào Redis Vector Store.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="snippet"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task CreateTextSnippetAsync(string collectionName, TextSnippet snippet, CancellationToken cancellationToken = default)
    {
        if (snippet == null) throw new ArgumentNullException(nameof(snippet));

        var redisKey = $"{collectionName}:{snippet.Key}";

        var jsonCommands = _database.JSON(); // lấy giao diện RedisJSON

        await jsonCommands.SetAsync(redisKey, "$", snippet);
    }

    /// <summary>
    /// Lưu nhiều TextSnippet vào Redis Vector Store theo collectionName.
    /// </summary>
    /// <param name="collectionName"></param>
    /// <param name="snippets"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<string>> CreateTextSnippetsBatchAsync(string collectionName, IEnumerable<TextSnippet> snippets, CancellationToken cancellationToken = default)
    {
        var keys = new List<string>();

        foreach (var snippet in snippets)
        {
            await CreateTextSnippetAsync(collectionName, snippet, cancellationToken);
        }
        return keys;
    }

    /// <summary>
    /// Tìm kiếm các vector gần nhất trong Redis Vector Store theo vector embedding.
    /// </summary>
    /// <param name="indexName"></param>
    /// <param name="vectorField"></param>
    /// <param name="queryEmbedding"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    public async Task<RedisResult?> SearchByVectorAsync(string indexName, string vectorField, float[] queryEmbedding, int k)
    {
        if (queryEmbedding == null || queryEmbedding.Length == 0)
            throw new ArgumentException("queryEmbedding cannot be null or empty", nameof(queryEmbedding));

        if (queryEmbedding.Length != 1536)
            throw new ArgumentException("Embedding dimension mismatch, expected 1536.", nameof(queryEmbedding));

        byte[] vectorBytes = new byte[queryEmbedding.Length * 4];
        Buffer.BlockCopy(queryEmbedding, 0, vectorBytes, 0, vectorBytes.Length);

        string query = $"*=>[KNN {k} @{vectorField} $vec_param AS vector_score]";
        try
        {
            RedisResult result = await _database.ExecuteAsync("FT.SEARCH", new object[]
            {
                indexName,
                query,
                "PARAMS", "2", "vec_param", vectorBytes,
                "SORTBY", "vector_score",
                "RETURN", "1", "Text",
                "DIALECT", "2",
                "LIMIT", "0", k.ToString()
            }).ConfigureAwait(false);

            return result;
        }
        catch (RedisServerException ex)
        {
            // Index không tồn tại, trả về null
            return null;
        }
    }

    public async Task<RedisResult> SearchByKeyOrTextAsync(string indexName, string? keyPrefix = null, int limit = 100, string? keyword = "*")
    {
        if (string.IsNullOrWhiteSpace(indexName))
            throw new ArgumentException("Index name must not be empty", nameof(indexName));

        // Tạo query FT.SEARCH
        string query = "*";
        try
        {
            var result = await _database.ExecuteAsync("FT.SEARCH", new object[]
            {
                indexName,
                query,
                "RETURN", "3", "Text", "ReferenceDescription", "ReferenceLink",
                "DIALECT", "2",
                "LIMIT", "0", "10"
            });

            return result;
        }
        catch (RedisServerException ex)
        {
            // Trường hợp index không tồn tại hoặc lỗi truy vấn
            return null;
        }
    }

    public async Task<string?> GetFileCacheAsync(string filePath, string cacheKey)
    {
        var jsonCommands = _database.JSON();
        try
        {
            var cachedText = await jsonCommands.GetAsync<string>(cacheKey);
            return cachedText;
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveFileCacheAsync(string filePath, string textContent, string cacheKey)
    {
        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists || string.IsNullOrEmpty(textContent) || string.IsNullOrEmpty(cacheKey))
            return;

        var jsonCommands = _database.JSON();
        try
        {
            var serialized = JsonSerializer.Serialize(textContent);
            await jsonCommands.SetAsync(cacheKey, "$", serialized);
            await _database.KeyExpireAsync(cacheKey, TimeSpan.FromDays(1));
        }
        catch (Exception)
        {
            throw;
        }
    }
}