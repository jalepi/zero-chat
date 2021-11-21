

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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var appSettings = configuration.Get<AppSettings>();

            var options = new ServiceProviderOptions { ValidateOnBuild = true };

            serviceProvider = new ServiceCollection()
                .ConfigureServices(appSettings)
                .BuildServiceProvider(options);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var requestRunner = serviceProvider.GetRequiredService<RequestRunner>();
            var requestOptions = serviceProvider.GetRequiredService<RequestOptions>();

            var backgroundService = new BackgroundService();
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
