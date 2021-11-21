namespace ZeroChat.Client.Commands;

public class AsyncCommand<T> : ICommand
{
    private readonly SendAsync<T> execute;
    private readonly Func<T, bool> canExecute;
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    public AsyncCommand(SendAsync<T> execute, Func<T, bool>? canExecute = null)
    {
        this.execute = async (value, ct) =>
        {
            try
            {
                await semaphoreSlim.WaitAsync(ct);
                await execute(value, ct);
            }
            finally
            {
                semaphoreSlim.Release();
                RaiseCanExecuteChanged();
            }
        };

        this.canExecute = value =>
        {
            return canExecute?.Invoke(value) ?? true && semaphoreSlim.CurrentCount > 0;
        };
    }

    public void CanExecute(T parameter) => canExecute(parameter);
    public void Execute(T parameter)
    {
        execute(parameter, default).SafeFireAndForget();
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    public bool CanExecute(object? parameter)
    {
        return parameter is T t && canExecute(t);
    }

    public void Execute(object? parameter)
    {
        if (parameter is T t)
        {
            execute(t, default).SafeFireAndForget();
        }
    }
}