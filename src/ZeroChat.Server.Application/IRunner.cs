namespace ZeroChat.Server.Application;

public interface IRunner
{
    Task Run(CancellationToken cancellationToken);
}
