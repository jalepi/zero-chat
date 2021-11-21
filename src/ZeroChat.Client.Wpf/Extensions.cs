namespace ZeroChat.Client.Extensions;

public static class TaskExtensions
{
    public static async void SafeFireAndForget(this ValueTask task,
        bool continueOnCapturedContext = false,
        Action<Exception>? onError = null,
        Action? onCompleted = null)
    {
        try
        {
            await task.ConfigureAwait(continueOnCapturedContext);
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
        }
        finally
        {
            onCompleted?.Invoke();
        }
    }
}
