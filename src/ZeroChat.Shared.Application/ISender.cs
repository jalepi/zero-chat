namespace ZeroChat.Shared;

public interface ISender<T>
{
    ValueTask PushAsync(T value, CancellationToken cancellationToken);
}
