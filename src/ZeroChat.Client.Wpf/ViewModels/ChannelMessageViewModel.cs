namespace ZeroChat.Client.ViewModels;

public record ChannelMessageViewModel
{
    public string AuthorId { get; init; } = "";
    public DateTimeOffset Timestamp { get; init; }
    public string Text { get; init; } = "";
    public bool IsMine { get; init; }
    public HorizontalAlignment HorizontalAlignment { get; init; }
}
