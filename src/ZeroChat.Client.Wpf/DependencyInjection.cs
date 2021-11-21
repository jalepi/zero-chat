namespace ZeroChat.Client;

internal static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, AppSettings appSettings)
    {
        _ = appSettings;

        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        };

        var requestChannel = Channel.CreateBounded<RequestCall>(options);

        services.AddSingleton(Dispatcher.CurrentDispatcher);
        services.AddSingleton<ReceiveAsync<RequestCall>>(requestChannel.Reader.ReadAsync);
        services.AddSingleton<SendAsync<RequestCall>>(requestChannel.Writer.WriteAsync);

        services.AddSingleton(provider => new RequestRunner("tcp://localhost:5559"));
        services.AddSingleton<RequestOptions>();

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

            var dispatcher = provider.GetRequiredService<Dispatcher>();
            var sendRequest = provider.GetRequiredService<SendAsync<RequestCall>>();
            var channel = new ChannelViewModel(dispatcher, sendRequest)
            {
                ChannelId = "DefaultChannel",
            };

            return new ApplicationViewModel(channels: new ObservableCollection<ChannelViewModel>
            {
                channel,
            });
        });

        services.AddSingleton<MainWindow>();

        return services;
    }
}
