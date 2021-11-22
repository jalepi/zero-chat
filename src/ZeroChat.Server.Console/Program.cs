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
        var requestChannel = Channel.CreateBounded<RequestCall>(options);

        var messageRequestHandler = new MessageRequestHandler(
            SendAsync: messageChannel.Writer.WriteAsync);

        var responseRunner = new ResponseRunner(ConnectionString: $"@tcp://localhost:{requestPort}");
        var responseOptions = new ResponseOptions(
            HandlAsync: messageRequestHandler.HandleAsync);

        var publisherRunner = new PublisherRunner(ConnectionString: $"@tcp://localhost:{messagePort}");
        var publisherOptions = new PublisherOptions(
            ReceiveAsync: messageChannel.Reader.ReadAsync);

        var subscriberRunner = new SubscriberRunner(ConnectionString: $"tcp://localhost:{messagePort}");
        var subscriberOptions = new SubscriberOptions(
            Topic: "",
            SendAsync: (message, ct) => ValueTask.CompletedTask);

        var requestRunner = new RequestRunner(ConnectionString: $"tcp://localhost:{requestPort}");
        var requestOptions = new RequestOptions(
            ReceiveAsync: requestChannel.Reader.ReadAsync);

        var consoleRequestRunner = new ConsoleRequestRunner();
        var consoleRequestOptions = new ConsoleRequestOptions(
            SendAsync: requestChannel.Writer.WriteAsync);

        CancellationTokenSource cts = new();

        var backgroundService = new BackgroundService();
        try
        {
            backgroundService.Start(responseRunner, responseOptions, cts.Token);
            backgroundService.Start(publisherRunner, publisherOptions, cts.Token);
            backgroundService.Start(subscriberRunner, subscriberOptions, cts.Token);
            backgroundService.Start(requestRunner, requestOptions, cts.Token);
            backgroundService.Start(consoleRequestRunner, consoleRequestOptions, cts.Token);

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
            return (8859, 8860);
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


