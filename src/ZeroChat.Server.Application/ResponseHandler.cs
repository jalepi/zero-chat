namespace ZeroChat.Server.Application;

public class ResponseHandler : IRunner
{
    private readonly string connectionString;
    private readonly ChannelWriter<Message> channelWriter;

    public ResponseHandler(string connectionString, ChannelWriter<Message> channelWriter)
    {
        this.connectionString = connectionString;
        this.channelWriter = channelWriter;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        using var socket = new ResponseSocket(connectionString);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = socket.ReceiveMultipartMessage();
            var protocol = message.Pop().ConvertToString();
            var content = message.Pop().ConvertToString();
            var guid = Guid.NewGuid().ToString();

            Console.WriteLine($"ResponseSocket protocol={protocol}, content={content}");
            
            switch (protocol)
            {
                case CommandType.SendMessageCommand:
                    
                    var sendMessageCommand = JsonSerializer.Deserialize<SendMessageCommand>(content)!;

                    var chatMessage = new ChatMessage(
                        Author: sendMessageCommand.Author,
                        Timestamp: sendMessageCommand.Timestamp,
                        Content: sendMessageCommand.Content);

                    var channelMessage = new Message(
                        Topic: new Topic(
                            Type: TopicType.SendMessage,
                            Name: sendMessageCommand.Channel),
                        Content: JsonSerializer.Serialize(chatMessage));

                    await channelWriter.WriteAsync(channelMessage, cancellationToken);
                    
                    break;

                default:
                    break;
            }
            socket.SendFrame(guid);
        }
    }
}
