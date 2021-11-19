namespace ZeroChat.Client.Wpf.Commands;

public record ChannelComposeMessageCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        
    }
}
