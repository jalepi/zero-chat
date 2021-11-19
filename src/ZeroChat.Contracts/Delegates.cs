namespace ZeroChat.Contracts;

public delegate ValueTask<T> PullAsync<T>(CancellationToken cancellationToken);

public delegate ValueTask PushAsync<T>(T value, CancellationToken cancellationToken);

public delegate ValueTask<TResult> Handle<T, TResult>(T value, CancellationToken cancellationToken);