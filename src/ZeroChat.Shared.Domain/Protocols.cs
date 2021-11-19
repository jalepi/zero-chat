namespace ZeroChat.Shared.Protocols;

public record Message(string Topic, string Payload);

public record Request(string Topic, string Payload);

public record Response(string Payload);

public record RequestCall(Request Request, Callback<Response> Callback);