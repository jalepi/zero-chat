namespace ZeroChat.Shared;

public record ChatMessage(string AuthorId, string Text, DateTimeOffset Timestamp);
