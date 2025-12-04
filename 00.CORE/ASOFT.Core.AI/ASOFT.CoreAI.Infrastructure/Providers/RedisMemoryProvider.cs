using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using System.Text.Json;

public class RedisMemoryProvider : IRedisMemoryProvider
{
    private readonly IRedisConfigProvider _redisConnectionManager;
    private const string MemoryKeyPrefix = "semantic_memory:";

    public RedisMemoryProvider(IRedisConfigProvider redisConnectionManager)
    {
        _redisConnectionManager = redisConnectionManager ?? throw new ArgumentNullException(nameof(redisConnectionManager));
    }

    // Helper: lấy DB luôn từ connection hiện tại (đã handle lazy reload)
    private async Task<IDatabase> GetDatabaseAsync()
    {
        var conn = await _redisConnectionManager.GetConnectionAsync();
        return conn.GetDatabase();
    }

    // Helper: lấy server từ connection hiện tại
    private async Task<IServer> GetServerAsync()
    {
        var conn = await _redisConnectionManager.GetConnectionAsync();
        var endpoint = conn.GetEndPoints().First();
        return conn.GetServer(endpoint);
    }

    private async Task<string> CreateAsync(CustomMemoryRecord record, CancellationToken cancellationToken = default)
    {
        var db = await GetDatabaseAsync();
        var redisKey = $"{MemoryKeyPrefix}{record.CollectionName}:{record.Key}";
        var value = JsonSerializer.Serialize(record);

        await db.StringSetAsync(redisKey, value);
        return record.Key;
    }

    public async Task<IEnumerable<CustomMemoryRecord>> GetByUserIdAsync(
        string collectionName,
        string agentCode,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var server = await GetServerAsync();
        var db = await GetDatabaseAsync();

        var pattern = $"{MemoryKeyPrefix}{collectionName}:*_{userId}_{agentCode}";

        var keys = server.Keys(pattern: pattern).ToArray();

        if (keys.Length == 0)
            return Enumerable.Empty<CustomMemoryRecord>();

        var values = await db.StringGetAsync(keys);

        var result = new List<CustomMemoryRecord>();

        foreach (var val in values)
        {
            if (!val.HasValue) continue;

            try
            {
                var record = JsonSerializer.Deserialize<CustomMemoryRecord>(val.ToString());
                if (record != null)
                    result.Add(record);
            }
            catch
            {
                continue;
            }
        }

        return result
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Reverse();
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
        var db = await GetDatabaseAsync();
        var cachedKey = await db.StringGetAsync(cacheKey);
        return !cachedKey.IsNullOrEmpty;
    }

    public async Task<string?> GetApiKeyAsync(string cacheKey)
    {
        var db = await GetDatabaseAsync();
        var cachedKey = await db.StringGetAsync(cacheKey);
        return cachedKey;
    }

    public async Task<string> SaveAPIKeyAsync(string cacheKey, ModelAIChatConfig config, double day)
    {
        var db = await GetDatabaseAsync();
        var redisKey = $"{cacheKey}";
        var value = JsonSerializer.Serialize(config);
        var result = await db.StringSetAsync(redisKey, value, TimeSpan.FromDays(day));
        return result.ToString();
    }

    public async Task<ModelAIChatConfig?> GetOpenAIChatConfigAsync(string cacheKey)
    {
        var db = await GetDatabaseAsync();
        var cachedJson = await db.StringGetAsync(cacheKey);
        if (cachedJson.IsNullOrEmpty)
        {
            return null;
        }

        try
        {
            var config = JsonSerializer.Deserialize<ModelAIChatConfig>(cachedJson!);
            return config;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Lỗi deserialize JSON: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CollectionExistsAsync(string indexName)
    {
        var db = await GetDatabaseAsync();
        try
        {
            await db.FT().InfoAsync(indexName).ConfigureAwait(false);
            return true;
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Unknown index name"))
        {
            return false;
        }
    }

    public async Task CreateIndexAsync(string operationName, string indexName, CancellationToken cancellationToken = default)
    {
        var db = await GetDatabaseAsync();

        string prefix = $"{indexName}:";
        await db.ExecuteAsync(operationName, new object[]
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

    public async Task CreateTextSnippetAsync(string collectionName, TextSnippet snippet, CancellationToken cancellationToken = default)
    {
        if (snippet == null) throw new ArgumentNullException(nameof(snippet));

        var db = await GetDatabaseAsync();
        var redisKey = $"{collectionName}:{snippet.Key}";

        var jsonCommands = db.JSON(); // RedisJSON
        await jsonCommands.SetAsync(redisKey, "$", snippet);
    }

    public async Task<IEnumerable<string>> CreateTextSnippetsBatchAsync(string collectionName, IEnumerable<TextSnippet> snippets, CancellationToken cancellationToken = default)
    {
        var keys = new List<string>();

        foreach (var snippet in snippets)
        {
            await CreateTextSnippetAsync(collectionName, snippet, cancellationToken);
            keys.Add(snippet.Key.ToString());
        }

        return keys;
    }

    public async Task<RedisResult?> SearchByVectorAsync(string indexName, string vectorField, float[] queryEmbedding, int k)
    {
        if (queryEmbedding == null || queryEmbedding.Length == 0)
            return null;

        if (queryEmbedding.Length != 1536)
            return null;

        var db = await GetDatabaseAsync();

        byte[] vectorBytes = new byte[queryEmbedding.Length * 4];
        Buffer.BlockCopy(queryEmbedding, 0, vectorBytes, 0, vectorBytes.Length);

        string query = $"*=>[KNN {k} @{vectorField} $vec_param AS vector_score]";

        try
        {
            RedisResult result = await db.ExecuteAsync("FT.SEARCH", new object[]
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
        catch (RedisServerException)
        {
            return null;
        }
    }

    public async Task<RedisResult?> SearchByKeyOrTextAsync(string indexName, string? keyPrefix = null, int limit = 100, string? keyword = "*")
    {
        if (string.IsNullOrWhiteSpace(indexName))
            throw new ArgumentException("Index name must not be empty", nameof(indexName));

        var db = await GetDatabaseAsync();

        string query = "*";
        try
        {
            var result = await db.ExecuteAsync("FT.SEARCH", new object[]
            {
                indexName,
                query,
                "RETURN", "3", "Text", "ReferenceDescription", "ReferenceLink",
                "DIALECT", "2",
                "LIMIT", "0", "10"
            });

            return result;
        }
        catch (RedisServerException)
        {
            return null;
        }
    }

    public async Task<string?> GetFileCacheAsync(string filePath, string cacheKey)
    {
        var db = await GetDatabaseAsync();
        var jsonCommands = db.JSON();

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
        var db = await GetDatabaseAsync();

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists || string.IsNullOrEmpty(textContent) || string.IsNullOrEmpty(cacheKey))
            return;

        var jsonCommands = db.JSON();
        try
        {
            // Nếu textContent đã là string thuần, có thể set trực tiếp, không cần serialize nữa:
            // await jsonCommands.SetAsync(cacheKey, "$", textContent);
            var serialized = JsonSerializer.Serialize(textContent);
            await jsonCommands.SetAsync(cacheKey, "$", serialized);
            await db.KeyExpireAsync(cacheKey, TimeSpan.FromDays(1));
        }
        catch
        {
            throw;
        }
    }
}
