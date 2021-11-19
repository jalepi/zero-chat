namespace ZeroChat.Server.Application;

public record SubscriberRunnerOptions(string ConnectionString, string Topic, PushAsync<Message> PushAsync);

public record SubscriberRunner : IRunner<SubscriberRunnerOptions>
{
    public async Task RunAsync(SubscriberRunnerOptions Options, CancellationToken cancellationToken)
    {
        using var socket = new SubscriberSocket(Options.ConnectionString);
        socket.Subscribe(Options.Topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            var multipartMessage = socket.ReceiveMultipartMessage();

            var topic = multipartMessage.Pop().ConvertToString();
            var content = multipartMessage.Pop().ConvertToString();

            var message = new Message(topic, content);

            Console.WriteLine($"sub => message: {message}");
            await Options.PushAsync(message, cancellationToken);
        }
    }
}
