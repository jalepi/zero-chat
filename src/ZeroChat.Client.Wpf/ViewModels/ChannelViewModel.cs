namespace ZeroChat.Client.ViewModels;

public class ChannelViewModel : BaseViewModel
{
    public ChannelViewModel(Dispatcher dispatcher, SendAsync<RequestCall> sendRequest)
    {
        ComposeMessageCommand = new AsyncCommand<string>(execute: (s, ct) =>
        {
            var request = new Request("default", "hello world");
            var requestCall = new RequestCall(request, response =>
            {
                dispatcher.BeginInvoke(() =>
                {
                    Messages.Add(new ChannelMessageViewModel
                    {
                        AuthorId = "me",
                        Text = response.Payload,
                        Timestamp = DateTime.Now,
                    });

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
}
