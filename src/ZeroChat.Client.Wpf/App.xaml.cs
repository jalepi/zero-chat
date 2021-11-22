﻿

namespace ZeroChat.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly CancellationTokenSource cts = new();
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            var options = new ServiceProviderOptions { ValidateOnBuild = true };

            var connectionSettings = new ConnectionSettings(
                RequestUrl: "tcp://localhost:5559",
                MessageUrl: "tcp://localhost:5560");

            serviceProvider = new ServiceCollection()
                .ConfigureServices(connectionSettings)
                .BuildServiceProvider(options);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var requestRunner = serviceProvider.GetRequiredService<RequestRunner>();
            var requestOptions = serviceProvider.GetRequiredService<RequestOptions>();

            var backgroundService = serviceProvider.GetRequiredService<BackgroundService>();
            backgroundService.Start(requestRunner, requestOptions, cts.Token);

            serviceProvider.GetRequiredService<MainWindow>().Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            cts.Cancel();
            Thread.Sleep(1000);
            base.OnExit(e);
        }
    }
}
