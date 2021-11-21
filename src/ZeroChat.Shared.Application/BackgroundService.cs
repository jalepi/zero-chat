namespace ZeroChat.Shared;

public record BackgroundService
{
    public void Start<TOptions>(IRunner<TOptions> runner, TOptions options, CancellationToken cancellationToken)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                runner.RunAsync(options, cancellationToken).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        });
    }
}
