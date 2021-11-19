namespace ZeroChat.Client.ViewModels;

public class ChannelMessageViewModel
{
    public string AuthorId { get; set; } = "";
    public DateTimeOffset Timestamp { get; set; }
    public string Text { get; set; } = "";
}
