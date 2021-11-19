using System.Threading.Channels;
using ZeroChat.Shared;
using ZeroChat.Shared;
using ZeroChat.Shared.Protocols;
using ZeroChat.Shared.Protocols;

public record RequestCallProcessorRunnerOptions(PullAsync<RequestCall> PullAsync, PushAsync<Message> PushAsync);

public record RequestCallProcessorRunner : IRunner<RequestCallProcessorRunnerOptions>
{
    public async Task RunAsync(RequestCallProcessorRunnerOptions options, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var requestCall = await options.PullAsync(cancellationToken);

            var (request, callback) = (requestCall.Request, requestCall.Callback);

            callback(new Response(Guid.NewGuid().ToString()));

            var message = new Message(Topic: request.Topic, Payload: request.Payload);
            await options.PushAsync(message, cancellationToken);
        }
    }
}
