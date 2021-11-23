using System.Threading.Channels;
using ZeroChat.Shared;
using ZeroChat.Shared.Protocols;

class Program
{
    public static async Task Main(string[] args)
    {
        var (requestPort, messagePort) = ReadPorts(args);

        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        };

        var messageChannel = Channel.CreateBounded<Message>(options);

        var messageRequestHandler = new MessageRequestHandler(
            SendAsync: messageChannel.Writer.WriteAsync);

        var responseConnectionString = $"@tcp://localhost:{requestPort}";
        var responseRunner = new ResponseRunner(ConnectionString: responseConnectionString);
        var responseOptions = new ResponseOptions(
            HandleAsync: messageRequestHandler.HandleAsync);

        var publisherConnectionString = $"@tcp://localhost:{messagePort}";
        var publisherRunner = new PublisherRunner(ConnectionString: publisherConnectionString);
        var publisherOptions = new PublisherOptions(
            ReceiveAsync: messageChannel.Reader.ReadAsync);

        CancellationTokenSource cts = new();

        try
        {
            var backgroundService = new BackgroundService();
            backgroundService.Start(responseRunner, responseOptions, cts.Token);
            backgroundService.Start(publisherRunner, publisherOptions, cts.Token);

            Console.WriteLine($"Listening to Request: {responseConnectionString}, Message: {publisherConnectionString}");
            while (true)
            {
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            cts.Cancel();
            Thread.Sleep(1_000);
        }
    }

    private static (int RequestPort, int MessagePort) ReadPorts(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            return (5559, 5560);
        }

        var arg1 = (args != null && args.Length > 0) ? args[0] : null;
        var arg2 = (args != null && args.Length > 1) ? args[1] : null;

        var requestPort = ReadPort("Provide port number for request socket: ", arg1);
        var messagePort = ReadPort("Provide port number for message socket: ", arg2);

        return (requestPort, messagePort);
    }

    private static int ReadPort(string message, string? arg)
    {
        int port;
        while (!int.TryParse(arg, out port))
        {
            Console.WriteLine(message);
            arg = Console.ReadLine();
        }
        return port;
    }
}


