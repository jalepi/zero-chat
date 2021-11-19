namespace ZeroChat.Server.Application;

public interface IRunner
{
    Task RunAsync(CancellationToken cancellationToken);
}

public interface IRunner<T>
{
    Task RunAsync(T options, CancellationToken cancellationToken);
}
