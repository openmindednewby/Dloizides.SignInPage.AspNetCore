using System.Threading.Channels;

namespace SignInPage.AspNetCore;

/// <summary>Bounded queue of submitted reset emails, so every request returns before any lookup or mail.</summary>
public sealed class PasswordResetQueue
{
    private const int Capacity = 1000;

    private readonly Channel<string> _channel = Channel.CreateBounded<string>(
        new BoundedChannelOptions(Capacity) { FullMode = BoundedChannelFullMode.DropWrite, SingleReader = true });

    public void Enqueue(string email) => _channel.Writer.TryWrite(email);

    internal int Count => _channel.Reader.Count;

    internal IAsyncEnumerable<string> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
