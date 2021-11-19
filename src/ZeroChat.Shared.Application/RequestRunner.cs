namespace ZeroChat.Shared;

public record RequestRunnerOptions(string ConnectionString, PullAsync<RequestCall> PullAsync);

public record RequestRunner : IRunner<RequestRunnerOptions>
{
    public async Task RunAsync(RequestRunnerOptions Options, CancellationToken cancellationToken)
    {
        using var socket = new RequestSocket(Options.ConnectionString);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var requestCall = await Options.PullAsync(cancellationToken);

            var (request, callback) = (requestCall.Request, requestCall.Callback);

            socket.SendMoreFrame(request.Topic);
            socket.SendFrame(request.Payload);

            Console.WriteLine($"req out => request: {request}");

            var responseMessage = socket.ReceiveMultipartMessage();
            var responsePayload = responseMessage.Pop().ConvertToString();

            var response = new Response(Payload: responsePayload);

            callback.Invoke(response);

            Console.WriteLine($"req in => response: {response}");
        }
    }
}
