namespace ZeroChat.Client.Wpf;

internal static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, AppSettings appSettings)
    {
        _ = appSettings;

        services.AddSingleton(provider =>
        {
            return new ApplicationViewModel(channels: new ObservableCollection<ChannelViewModel>
            { 
                new ChannelViewModel(messages: new ObservableCollection<ChannelMessageViewModel>
                { 
                
                })
                {
                    ChannelId = "Default Channel",
                },
            });
        });
        services.AddSingleton<MainWindow>();
        
        return services;
    }
}
