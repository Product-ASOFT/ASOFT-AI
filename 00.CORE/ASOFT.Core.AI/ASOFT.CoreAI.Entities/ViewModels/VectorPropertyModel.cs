using Microsoft.Extensions.AI;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ASOFT.CoreAI.Entities;

public class VectorPropertyModel(string modelName, Type type) : PropertyModel(modelName, type)
{
    private int _dimensions;

    public int Dimensions
    {
        get => this._dimensions;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Dimensions must be greater than zero.");
            }
            this._dimensions = value;
        }
    }

    public string? IndexKind { get; set; }

    public string? DistanceFunction { get; set; }

    [AllowNull]
    public Type EmbeddingType { get; set; } = null!;

    public IEmbeddingGenerator? EmbeddingGenerator { get; set; }

    public virtual Type? ResolveEmbeddingType<TEmbedding>(IEmbeddingGenerator embeddingGenerator, Type? userRequestedEmbeddingType)
        where TEmbedding : Embedding
        => embeddingGenerator switch
        {
            IEmbeddingGenerator<string, TEmbedding> when this.Type == typeof(string) && (userRequestedEmbeddingType is null || userRequestedEmbeddingType == typeof(TEmbedding))
                => typeof(TEmbedding),
            IEmbeddingGenerator<DataContent, TEmbedding> when this.Type == typeof(DataContent) && (userRequestedEmbeddingType is null || userRequestedEmbeddingType == typeof(TEmbedding))
                => typeof(TEmbedding),
            null => throw new ArgumentNullException(nameof(embeddingGenerator), "This method should only be called when an embedding generator is configured."),
            _ => null
        };

    public virtual bool TryGenerateEmbedding<TRecord, TEmbedding>(TRecord record, CancellationToken cancellationToken, [NotNullWhen(true)] out Task<TEmbedding>? task)
      where TRecord : class
      where TEmbedding : Embedding
    {
        switch (this.EmbeddingGenerator)
        {
            case IEmbeddingGenerator<string, TEmbedding> generator when this.EmbeddingType == typeof(TEmbedding):
                {
                    task = generator.GenerateAsync(
                        this.GetValueAsObject(record) is var value && value is string s
                            ? s
                            : throw new InvalidOperationException($"Property '{this.ModelName}' was configured with an embedding generator accepting a string, but {value?.GetType().Name ?? "null"} was provided."),
                        options: null,
                        cancellationToken);
                    return true;
                }
            case IEmbeddingGenerator<DataContent, TEmbedding> generator when this.EmbeddingType == typeof(TEmbedding):
                {
                    task = generator.GenerateAsync(
                        this.GetValueAsObject(record) is var value && value is DataContent c
                            ? c
                            : throw new InvalidOperationException($"Property '{this.ModelName}' was configured with an embedding generator accepting a {nameof(DataContent)}, but {value?.GetType().Name ?? "null"} was provided."),
                        options: null,
                        cancellationToken);
                    return true;
                }
            case null:
                throw new UnreachableException("This method should only be called when an embedding generator is configured.");
            default:
                task = null;
                return false;
        }
    }

    public virtual bool TryGenerateEmbeddings<TRecord, TEmbedding>(IEnumerable<TRecord> records, CancellationToken cancellationToken, [NotNullWhen(true)] out Task<GeneratedEmbeddings<TEmbedding>>? task)
        where TRecord : class
        where TEmbedding : Embedding
    {
        switch (this.EmbeddingGenerator)
        {
            case IEmbeddingGenerator<string, TEmbedding> generator when this.EmbeddingType == typeof(TEmbedding):
                task = generator.GenerateAsync(
                    records.Select(r => this.GetValueAsObject(r) is var value && value is string s
                        ? s
                        : throw new InvalidOperationException($"Property '{this.ModelName}' was configured with an embedding generator accepting a string, but {value?.GetType().Name ?? "null"} was provided.")),
                    options: null,
                    cancellationToken);
                return true;

            case IEmbeddingGenerator<DataContent, TEmbedding> generator when this.EmbeddingType == typeof(TEmbedding):
                task = generator.GenerateAsync(
                    records.Select(r => this.GetValueAsObject(r) is var value && value is DataContent c
                        ? c
                        : throw new InvalidOperationException($"Property '{this.ModelName}' was configured with an embedding generator accepting a {nameof(DataContent)}, but {value?.GetType().Name ?? "null"} was provided.")),
                    options: null,
                    cancellationToken);
                return true;

            case null:
                throw new UnreachableException("This method should only be called when an embedding generator is configured.");
            default:
                task = null;
                return false;
        }
    }
}