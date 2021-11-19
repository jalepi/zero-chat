namespace ZeroChat.Server.Application;

public record PublisherRunnerOptions(string ConnectionString, PullAsync<Message> PullAsync);

public record PublisherRunner : IRunner<PublisherRunnerOptions>
{
    public async Task RunAsync(PublisherRunnerOptions options, CancellationToken cancellationToken)
    {
        using var socket = new PublisherSocket(options.ConnectionString);

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = await options.PullAsync(cancellationToken);

            socket.SendMoreFrame(message.Topic);
            socket.SendFrame(message.Payload);

            Console.WriteLine($"pub => message: {message}");
        }
    }
}
