namespace ZeroChat.Client.ViewModels;

public class ChannelViewModel : BaseViewModel
{
    private readonly Dispatcher dispatcher;
    public ChannelViewModel(Dispatcher dispatcher, SendAsync<RequestCall> sendRequest)
    {
        this.dispatcher = dispatcher;
        ComposeMessageCommand = new AsyncCommand<string>(execute: (s, ct) =>
        {
            var chatMessage = new ChatMessage(
                AuthorId: this.AuthorId,
                Text: ComposingText ?? "",
                Timestamp: DateTimeOffset.UtcNow);

            // TODO: 2021-11-23 implement proper serialization
            var payload = JsonSerializer.Serialize(chatMessage);

            var request = new Request(
                Topic: ChannelId,
                Payload: payload);

            var requestCall = new RequestCall(request, response =>
            {
                dispatcher.BeginInvoke(() =>
                {
                    ComposingText = null;
                });
            });
            return sendRequest(requestCall, ct);
        });
    }

    public string AuthorId { get; init; } = Guid.NewGuid().ToString();

    public string ChannelId { get; init; } = "";

    public AsyncCommand<string> ComposeMessageCommand { get; }

    private string? _composingText = null;
    public string? ComposingText
    {
        get => _composingText;
        set => SetProperty(ref _composingText, value, ComposeMessageCommand.RaiseCanExecuteChanged);
    }

    public ICollection<ChannelMessageViewModel> Messages { get; } = new ObservableCollection<ChannelMessageViewModel>();

    public ValueTask ReceiveMessageAsync(Message message, CancellationToken cancellationToken)
    {
        dispatcher.InvokeAsync(() =>
        {
            // TODO: 2021-11-23 implement proper serialization
            var chatMessage = JsonSerializer.Deserialize<ChatMessage>(message.Payload);
            if (chatMessage == null) return;

            Messages.Add(new ChannelMessageViewModel
            {
                AuthorId = chatMessage.AuthorId,
                Text = chatMessage.Text,
                Timestamp = chatMessage.Timestamp,
                IsMine = chatMessage.AuthorId != this.AuthorId,
                HorizontalAlignment = chatMessage.AuthorId != this.AuthorId
                    ? HorizontalAlignment.Left
                    : HorizontalAlignment.Right,
            });
        },
        priority: DispatcherPriority.Background,
        cancellationToken: cancellationToken);
        return ValueTask.CompletedTask;
    }
}
