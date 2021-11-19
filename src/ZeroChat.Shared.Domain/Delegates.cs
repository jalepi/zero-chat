namespace ZeroChat.Shared;

public delegate Task AsyncCallback<T>(T value, CancellationToken cancellationToken);

public delegate void Callback<T>(T value);

public delegate ValueTask<T> PullAsync<T>(CancellationToken cancellationToken);

public delegate ValueTask PushAsync<T>(T value, CancellationToken cancellationToken);

public delegate ValueTask<TResult> Handle<T, TResult>(T value, CancellationToken cancellationToken);