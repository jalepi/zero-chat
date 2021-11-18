using System.Threading.Channels;
using ZeroChat.Contracts.Messages;
using ZeroChat.Server.Application;

class Program
{
    public static async Task Main(string[] args)
    {
        _ = args;
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        };
        var channel = Channel.CreateBounded<Message>(options);

        var requestHandler = new ResponseHandler(
            connectionString: "@tcp://localhost:5559",
            channelWriter: channel.Writer);

        var publisher = new Publisher(
            connectionString: "@tcp://localhost:5560",
            channelReader: channel.Reader);

        var dummyRequestHandler = new DummyRequestHandler(
            connectionString: "tcp://localhost:5559");

        var dummySubscriber = new DummySubscriber(
            connectionString: "tcp://localhost:5560");

        CancellationTokenSource cancellationTokenSource = new();
        try
        {
            StartRunner(requestHandler, cancellationTokenSource.Token);
            StartRunner(publisher, cancellationTokenSource.Token);
            StartRunner(dummyRequestHandler, cancellationTokenSource.Token);
            StartRunner(dummySubscriber, cancellationTokenSource.Token);

            while (true)
            {
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            cancellationTokenSource.Cancel();
            Thread.Sleep(1_000);
        }
    }

    private static void StartRunner(IRunner runner, CancellationToken cancellationToken)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                runner.Run(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        });
    }
}


