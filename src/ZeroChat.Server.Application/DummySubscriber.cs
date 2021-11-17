namespace ZeroChat.Server.Application;

public class DummySubscriber : IRunner
{
    private readonly string connectionString;

    public DummySubscriber(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        using var socket = new SubscriberSocket(connectionString);
        socket.Subscribe("m:");

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = socket.ReceiveMultipartMessage();

            var topic = message.Pop().ConvertToString();
            var content = message.Pop().ConvertToString();

            var channelMessage = JsonSerializer.Deserialize<ChatMessage>(content)!;

            Console.WriteLine($"SubscriptionMessage: topic={topic}, author={channelMessage.Author}, content={channelMessage.Content}, timestamp={channelMessage.Timestamp:o}");

            await Task.Delay(1000);
        }
    }
}