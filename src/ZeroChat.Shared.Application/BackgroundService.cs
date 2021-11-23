namespace ZeroChat.Shared;

public record BackgroundService
{
    private readonly ConcurrentDictionary<Guid, Task> tasks = new();

    public void Start<TOptions>(IRunner<TOptions> runner, TOptions options, CancellationToken cancellationToken)
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            var key = Guid.NewGuid();

            try
            {
                var task = runner.RunAsync(options, cancellationToken);

                // TODO: 2021-11-23 implement task registry
                _ = tasks.TryAdd(key, task);

                task.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        });
    }
}
