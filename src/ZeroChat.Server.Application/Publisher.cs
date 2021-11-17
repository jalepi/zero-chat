namespace ZeroChat.Server.Application
{
    public class Publisher : IRunner
    {
        private readonly string connectionString;
        private readonly ChannelReader<Message> channelReader;

        public Publisher(string connectionString, ChannelReader<Message> channelReader)
        {
            this.connectionString = connectionString;
            this.channelReader = channelReader;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            using var socket = new PublisherSocket(connectionString);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await channelReader.ReadAsync(cancellationToken);
                
                socket.SendMoreFrame(message.Topic);
                socket.SendFrame(message.Content);
            }
        }
    }
}