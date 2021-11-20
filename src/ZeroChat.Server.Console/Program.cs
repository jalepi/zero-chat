using System.Threading.Channels;
using ZeroChat.Shared;
using ZeroChat.Shared.Protocols;

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

        var messageChannel = Channel.CreateBounded<Message>(options);
        var requestChannel = Channel.CreateBounded<RequestCall>(options);

        var messageRequestHandler = new MessageRequestHandler(
            PushAsync: messageChannel.Writer.WriteAsync);

        var responseRunner = new ResponseRunner(ConnectionString: "@tcp://localhost:5559");
        var responseOptions = new ResponseOptions(
            HandlAsync: messageRequestHandler.HandleAsync);

        var publisherRunner = new PublisherRunner(ConnectionString: "@tcp://localhost:5560");
        var publisherOptions = new PublisherOptions(
            PullAsync: messageChannel.Reader.ReadAsync);

        var subscriberRunner = new SubscriberRunner(ConnectionString: "tcp://localhost:5560");
        var subscriberOptions = new SubscriberOptions(
            Topic: "",
            PushAsync: (message, ct) => ValueTask.CompletedTask);

        var requestRunner = new RequestRunner(ConnectionString: "tcp://localhost:5559");
        var requestOptions = new RequestOptions(
            PullAsync: requestChannel.Reader.ReadAsync);

        var consoleRequestRunner = new ConsoleRequestRunner();
        var consoleRequestOptions = new ConsoleRequestOptions(
            PushAsync: requestChannel.Writer.WriteAsync);

        CancellationTokenSource cancellationTokenSource = new();

        try
        {
            StartRunner(responseRunner, responseOptions, cancellationTokenSource.Token);
            StartRunner(publisherRunner, publisherOptions, cancellationTokenSource.Token);
            StartRunner(subscriberRunner, subscriberOptions, cancellationTokenSource.Token);
            StartRunner(requestRunner, requestOptions, cancellationTokenSource.Token);
            StartRunner(consoleRequestRunner, consoleRequestOptions, cancellationTokenSource.Token);

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

    private static void StartRunner<T>(IRunner<T> runner, T options, CancellationToken cancellationToken)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                runner.RunAsync(options, cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        });
    }
}


