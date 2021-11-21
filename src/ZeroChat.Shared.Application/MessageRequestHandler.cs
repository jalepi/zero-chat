namespace ZeroChat.Shared;

public record MessageRequestHandler(SendAsync<Message> SendAsync) : IHandler<Request, Response>
{
    public async ValueTask<Response> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        var message = new Message(request.Topic, request.Payload);
        await SendAsync(message, cancellationToken);
        return new Response(Guid.NewGuid().ToString());
    }
}
