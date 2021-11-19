namespace ZeroChat.Server.Application;

public record ResponseRunnerOptions(string ConnectionString, Handle<Request, Response> HandlAsync);

public record ResponseRunner : IRunner<ResponseRunnerOptions>
{
    public async Task RunAsync(ResponseRunnerOptions options, CancellationToken cancellationToken)
    {
        using var socket = new ResponseSocket(options.ConnectionString);
        
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
