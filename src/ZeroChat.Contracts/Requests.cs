namespace ZeroChat.Contracts.Requests;

public record Request(string Topic, string Payload);

public record Response(string Payload);

public delegate Task AsyncCallback<T>(T value, CancellationToken cancellationToken);
public delegate void Callback<T>(T value);

public record RequestCall(Request Request, Callback<Response> Callback);