namespace ZeroChat.Client.ViewModels;

public class ChannelViewModel : BaseViewModel
{
    private Dispatcher dispatcher;
    public ChannelViewModel(Dispatcher dispatcher, SendAsync<RequestCall> sendRequest)
    {
        this.dispatcher = dispatcher;
        ComposeMessageCommand = new AsyncCommand<string>(execute: (s, ct) =>
        {
            var request = new Request(
                Topic: ChannelId,
                Payload: ComposingText);

            var requestCall = new RequestCall(request, response =>
            {
                dispatcher.BeginInvoke(() =>
                {
                    ComposingText = "";
                });
            });
            return sendRequest(requestCall, ct);
        });
    }

    public string ChannelId { get; init; } = "";

    public AsyncCommand<string> ComposeMessageCommand { get; }

    private string _composingText = "";
    public string ComposingText
    {
        get => _composingText;
        set => SetProperty(ref _composingText, value, ComposeMessageCommand.RaiseCanExecuteChanged);
    }

    public ICollection<ChannelMessageViewModel> Messages { get; } = new ObservableCollection<ChannelMessageViewModel>();

    public ValueTask ReceiveMessageAsync(Message message, CancellationToken cancellationToken)
    {
        dispatcher.InvokeAsync(() =>
        {
            Messages.Add(new ChannelMessageViewModel
            {
                AuthorId = "me",
                Text = message.Payload,
                Timestamp = DateTimeOffset.UtcNow,
            });
        }, 
        priority: DispatcherPriority.Background, 
        cancellationToken: cancellationToken);
        return ValueTask.CompletedTask;
    }
}
