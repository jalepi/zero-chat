namespace ZeroChat.Server.Application;

public record MessageRequestHandler(PushAsync<Message> PushAsync) : IHandler<Request, Response>
{
    public async ValueTask<Response> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var message = new Message(request.Topic, request.Payload);
        await PushAsync(message, cancellationToken);
        return new Response(Guid.NewGuid().ToString());
    }
}
