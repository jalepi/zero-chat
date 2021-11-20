namespace ZeroChat.Shared;

public record ResponseOptions(Handle<Request, Response> HandlAsync);

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

            Console.WriteLine($"rep in => topic: {topic}, payload: {payload}");

            var request = new Request(topic, payload);

            var response = await options.HandlAsync(request, cancellationToken);
            socket.SendFrame(response.Payload);

            Console.WriteLine($"rep out => payload: {response.Payload}");
        }
    }
}
