namespace ZeroChat.Shared;

public interface IHandler<T, TResult>
{
    ValueTask<TResult> HandleAsync(T value, CancellationToken cancellationToken);
}
