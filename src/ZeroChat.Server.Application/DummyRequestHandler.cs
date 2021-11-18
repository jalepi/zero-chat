namespace ZeroChat.Server.Application;

public class DummyRequestHandler : IRunner
{
    private readonly string connectionString;

    public DummyRequestHandler(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        using var socket = new RequestSocket(connectionString);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var sendMessageCommand = new SendMessageCommand(
                    Channel: "broadcast-channel",
                    Author: "Dummy Request Handler",
                    Timestamp: DateTimeOffset.UtcNow,
                    Content: $"Hello there!");

                socket.SendMoreFrame(CommandType.SendMessageCommand);
                socket.SendFrame(JsonSerializer.Serialize(sendMessageCommand));

                var responseMessage = socket.ReceiveMultipartMessage();
                Console.WriteLine($"ResponseMessage: {responseMessage.Pop().ConvertToString()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}
