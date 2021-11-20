namespace ZeroChat.Shared;

public record PublisherOptions(PullAsync<Message> PullAsync);

public record PublisherRunner(string ConnectionString) : IRunner<PublisherOptions>
{
    public async Task RunAsync(PublisherOptions options, CancellationToken cancellationToken)
    {
        using var socket = new PublisherSocket(ConnectionString);

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await options.PullAsync(cancellationToken);

            socket.SendMoreFrame(message.Topic);
            socket.SendFrame(message.Payload);

            Console.WriteLine($"pub => message: {message}");
        }
    }
}
