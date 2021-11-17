namespace ZeroChat.Contracts.Commands;

public record SendMessageCommand(string Channel, string Author, DateTimeOffset Timestamp, string Content);
