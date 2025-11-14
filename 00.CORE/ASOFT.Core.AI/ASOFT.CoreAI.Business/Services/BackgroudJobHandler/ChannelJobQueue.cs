// ChannelJobQueue.cs
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure.Interface;
using System.Threading.Channels;

public sealed class ChannelJobQueue : IJobQueue
{
    private readonly Channel<ReadFileJob> _channel;

    public ChannelJobQueue(int capacity = 200)
    {
        _channel = Channel.CreateBounded<ReadFileJob>(
            new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public ValueTask EnqueueAsync(ReadFileJob job, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(job, ct);

    public ValueTask<ReadFileJob> DequeueAsync(CancellationToken ct)
        => _channel.Reader.ReadAsync(ct);
}
