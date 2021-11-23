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

        var requestChannel = Channel.CreateBounded<RequestCall>(options);

        var subscriberConnectionString = $"tcp://localhost:{messagePort}";
        var subscriberRunner = new SubscriberRunner(ConnectionString: subscriberConnectionString);
        var subscriberOptions = new SubscriberOptions(
            Topic: "",
            SendAsync: (message, ct) =>
            {
                Console.WriteLine($"sub: {message}");
                return ValueTask.CompletedTask;
            });

        var requestConnectionString = $"tcp://localhost:{requestPort}";
        var requestRunner = new RequestRunner(ConnectionString: requestConnectionString);
        var requestOptions = new RequestOptions(
            ReceiveAsync: requestChannel.Reader.ReadAsync);

        var consoleRequestRunner = new ConsoleRequestRunner();
        var consoleRequestOptions = new ConsoleRequestOptions(
            SendAsync: requestChannel.Writer.WriteAsync);

        CancellationTokenSource cts = new();

        var backgroundService = new BackgroundService();
        try
        {
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


