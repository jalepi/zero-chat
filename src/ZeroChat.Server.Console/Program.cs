using System.Collections.Concurrent;
using System.Threading.Channels;
using ZeroChat.Contracts.Messages;
using ZeroChat.Contracts.Requests;
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

        var messageChannel = Channel.CreateBounded<Message>(options);
        var requestChannel = Channel.CreateBounded<RequestCall>(options);

        var messageRequestHandler = new MessageRequestHandler(
            PushAsync: messageChannel.Writer.WriteAsync);

        var responseRunner = new ResponseRunner();
        var responseRunnerOptions = new ResponseRunnerOptions(
            ConnectionString: "@tcp://localhost:5559",
            HandlAsync: messageRequestHandler.HandleAsync);

        var publisherRunner = new PublisherRunner();
        var publisherRunnerOptions = new PublisherRunnerOptions(
            ConnectionString: "@tcp://localhost:5560",
            PullAsync: messageChannel.Reader.ReadAsync);

        var subscriberRunner = new SubscriberRunner();
        var subscriberRunnerOptions = new SubscriberRunnerOptions(
            ConnectionString: "tcp://localhost:5560",
            Topic: "",
            PushAsync: (message, ct) => ValueTask.CompletedTask);

        var requestRunner = new RequestRunner();
        var requestRunnerOptions = new RequestRunnerOptions(
            ConnectionString: "tcp://localhost:5559",
            PullAsync: requestChannel.Reader.ReadAsync);

        var consoleRequestRunner = new ConsoleRequestRunner(
            PushAsync: requestChannel.Writer.WriteAsync);

        //var requestCallProcessorRunnerOptions = new RequestCallProcessorRunnerOptions(
        //    PullAsync: responseChannel.Reader.ReadAsync,
        //    PushAsync: messageChannel.Writer.WriteAsync);

        //var requestCallProcessorRunner = new RequestCallProcessorRunner();

        CancellationTokenSource cancellationTokenSource = new();

        try
        {
            StartRunner(responseRunner, responseRunnerOptions, cancellationTokenSource.Token);
            StartRunner(publisherRunner, publisherRunnerOptions, cancellationTokenSource.Token);
            StartRunner(subscriberRunner, subscriberRunnerOptions, cancellationTokenSource.Token);
            StartRunner(requestRunner, requestRunnerOptions, cancellationTokenSource.Token);
            StartRunner(consoleRequestRunner, cancellationTokenSource.Token);
            //StartRunner(requestCallProcessorRunner, requestCallProcessorRunnerOptions, cancellationTokenSource.Token);

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
                runner.RunAsync(cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        });
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


