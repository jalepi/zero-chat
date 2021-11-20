namespace ZeroChat.Client.ViewModels;

public record ChannelViewModel(
    string ChannelId,
    ChannelComposeMessageCommand ComposeMessageCommand,
    ICollection<ChannelMessageViewModel> Messages) : BaseViewModel
{
    private string _composingText = "";
    public string ConsposingText
    {
        get => _composingText;
        set => SetProperty(ref _composingText, value);
    }
}
