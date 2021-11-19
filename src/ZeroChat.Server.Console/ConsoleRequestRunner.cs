using System.Threading.Channels;
using ZeroChat.Contracts;
using ZeroChat.Contracts.Requests;
using ZeroChat.Server.Application;

internal record ConsoleRequestRunner(PushAsync<RequestCall> PushAsync) : IRunner
{
    public async Task RunAsync(CancellationToken cancellationToken)
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
                await PushAsync(requestCall, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
