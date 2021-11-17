namespace ZeroChat.Contracts.Messages;

public record ChatMessage(string Author, DateTimeOffset Timestamp, string Content);
