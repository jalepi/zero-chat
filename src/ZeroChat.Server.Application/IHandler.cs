namespace ZeroChat.Server.Application;

public interface IHandler<T, TResult>
{
    ValueTask<TResult> HandleAsync(T value, CancellationToken cancellationToken);
}
