namespace ZeroChat.Shared;

public interface ISender<T>
{
    ValueTask SendAsync(T value, CancellationToken cancellationToken);
}
