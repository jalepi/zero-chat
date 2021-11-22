namespace ZeroChat.Shared;

public record SubscriberOptions(string Topic, SendAsync<Message> SendAsync);

public record SubscriberRunner(string ConnectionString) : IRunner<SubscriberOptions>
{
    public async Task RunAsync(SubscriberOptions options, CancellationToken cancellationToken)
    {
        using var socket = new SubscriberSocket(ConnectionString);
        socket.Subscribe(options.Topic);

        while (!cancellationToken.IsCancellationRequested)
        {
            var multipartMessage = socket.ReceiveMultipartMessage();

            var topic = multipartMessage.Pop().ConvertToString();
            var content = multipartMessage.Pop().ConvertToString();

            var message = new Message(topic, content);

            Console.WriteLine($"sub => message: {message}");
            await options.SendAsync(message, cancellationToken);
        }
    }
}
