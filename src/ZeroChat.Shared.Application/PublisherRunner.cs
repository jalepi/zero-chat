namespace ZeroChat.Shared;

public record PublisherOptions(ReceiveAsync<Message> ReceiveAsync);

public record PublisherRunner(string ConnectionString) : IRunner<PublisherOptions>
{
    public async Task RunAsync(PublisherOptions options, CancellationToken cancellationToken)
    {
        using var socket = new PublisherSocket(ConnectionString);

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await options.ReceiveAsync(cancellationToken);

            socket.SendMoreFrame(message.Topic);
            socket.SendFrame(message.Payload);
        }
    }
}
