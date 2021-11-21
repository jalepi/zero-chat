namespace ZeroChat.Shared;

public delegate void Callback<T>(T value);

public delegate ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken);

public delegate ValueTask SendAsync<T>(T value, CancellationToken cancellationToken);

public delegate ValueTask<TResult> HandleAsync<T, TResult>(T value, CancellationToken cancellationToken);