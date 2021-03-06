namespace ZeroChat.Shared;

public record ResponseOptions(HandleAsync<Request, Response> HandleAsync);

public record ResponseRunner(string ConnectionString) : IRunner<ResponseOptions>
{
    public async Task RunAsync(ResponseOptions options, CancellationToken cancellationToken)
    {
        using var socket = new ResponseSocket(ConnectionString);

        while (!cancellationToken.IsCancellationRequested)
        {
            var message = socket.ReceiveMultipartMessage();
            var topic = message.Pop().ConvertToString();
            var payload = message.Pop().ConvertToString();

            var request = new Request(topic, payload);

            var response = await options.HandleAsync(request, cancellationToken);
            socket.SendFrame(response.Payload);
        }
    }
}
