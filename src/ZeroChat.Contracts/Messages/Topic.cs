namespace ZeroChat.Contracts.Messages;

public record Topic(string Type, string Name)
{
    public static string ToString(Topic value) => $"{value.Type}:{value.Name}";
    public static Topic FromString(string value)
    {
        var parts = value.Split(":", StringSplitOptions.None);

        return new Topic(
            Type: parts.Length > 0 ? parts[0] : "",
            Name: parts.Length > 1 ? parts[1] : "");
    }

    public static implicit operator string(Topic value) => Topic.ToString(value);
    public static implicit operator Topic(string value) => Topic.FromString(value);
}
