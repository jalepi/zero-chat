namespace ZeroChat.Client;

internal static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, AppSettings appSettings)
    {
        _ = appSettings;

        services.AddSingleton(new ConnectionSettings
        {
            RequestService = ConnectionSettings.DefaultRequestService,
            MessageService = ConnectionSettings.DefaultMessageService,
        });

        services.AddSingleton(provider =>
        {
            var messages = new ObservableCollection<ChannelMessageViewModel>
            {

            };

            var composeMessageCommand = new ChannelComposeMessageCommand();

            var channel = new ChannelViewModel(
                ChannelId: "Default Channel",
                ComposeMessageCommand: composeMessageCommand,
                Messages: messages);

            return new ApplicationViewModel(channels: new ObservableCollection<ChannelViewModel>
            {
                channel,
            });
        });

        services.AddSingleton<MainWindow>();

        return services;
    }
}
