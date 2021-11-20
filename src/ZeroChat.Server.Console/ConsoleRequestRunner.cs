using ZeroChat.Shared;
using ZeroChat.Shared.Protocols;

internal record ConsoleRequestOptions(PushAsync<RequestCall> PushAsync);
internal record ConsoleRequestRunner : IRunner<ConsoleRequestOptions>
{
    public async Task RunAsync(ConsoleRequestOptions options, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var command = Console.ReadLine() ?? "";
                var split = command.IndexOf(' ');

                var topic = command[..split];
                var payload = command[(split + 1)..];

                var request = new Request(topic, payload);

                var requestCall = new RequestCall(request, response =>
                {
                    Console.WriteLine($"console out: response: {response}");
                });

                Console.WriteLine($"console in: request: {request}");
                await options.PushAsync(requestCall, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
