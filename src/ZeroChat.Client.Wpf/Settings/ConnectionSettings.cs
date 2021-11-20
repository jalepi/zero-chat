namespace ZeroChat.Client.Settings;

public class ConnectionSettings
{
    public const string DefaultRequestService = "tcp://localhost:5559";
    public const string DefaultMessageService = "tcp://localhost:5560";
    public string RequestService { get; init; } = DefaultRequestService;
    public string MessageService { get; init; } = DefaultMessageService;
}
