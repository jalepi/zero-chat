namespace ZeroChat.Client;

internal static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, ConnectionSettings connectionSettings)
    {
        var options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        };

        var requestChannel = Channel.CreateBounded<RequestCall>(options);

        services.AddSingleton<BackgroundService>();
        services.AddSingleton(Dispatcher.CurrentDispatcher);
        services.AddSingleton<ReceiveAsync<RequestCall>>(requestChannel.Reader.ReadAsync);
        services.AddSingleton<SendAsync<RequestCall>>(requestChannel.Writer.WriteAsync);

        services.AddSingleton(provider => new SubscriberRunner("tcp://localhost:5560"));
        services.AddSingleton(provider => new RequestRunner("tcp://localhost:5559"));
        services.AddSingleton<RequestOptions>();

        services.AddSingleton(provider =>
        {
            var topic = "DefaultChannel";
            var dispatcher = provider.GetRequiredService<Dispatcher>();

            var sendRequest = provider.GetRequiredService<SendAsync<RequestCall>>();
            var channel = new ChannelViewModel(dispatcher, sendRequest)
            {
                ChannelId = topic,
            };
            var backgroundService = provider.GetRequiredService<BackgroundService>();
            var subscriberRunner = provider.GetRequiredService<SubscriberRunner>();
            var subscriberOptions = new SubscriberOptions(
                Topic: channel.ChannelId,
                SendAsync: channel.ReceiveMessageAsync);

            backgroundService.Start(subscriberRunner, subscriberOptions, default);

            return new ApplicationViewModel(channels: new ObservableCollection<ChannelViewModel>
            {
                channel,
            });
        });

        services.AddSingleton<MainWindow>();

        return services;
    }
}
