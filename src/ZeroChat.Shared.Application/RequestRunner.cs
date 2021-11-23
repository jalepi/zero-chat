namespace ZeroChat.Shared;

public record RequestOptions(ReceiveAsync<RequestCall> ReceiveAsync);

public record RequestRunner(string ConnectionString) : IRunner<RequestOptions>
{
    public async Task RunAsync(RequestOptions options, CancellationToken cancellationToken)
    {
        using var socket = new RequestSocket(ConnectionString);

        while (!cancellationToken.IsCancellationRequested)
        {
            var requestCall = await options.ReceiveAsync(cancellationToken);

            var (request, callback) = (requestCall.Request, requestCall.Callback);

            socket.SendMoreFrame(request.Topic);
            socket.SendFrame(request.Payload);

            var responseMessage = socket.ReceiveMultipartMessage();
            var responsePayload = responseMessage.Pop().ConvertToString();

            var response = new Response(Payload: responsePayload);

            callback.Invoke(response);
        }
    }
}
