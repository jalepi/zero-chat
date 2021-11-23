

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

            var args = Environment.GetCommandLineArgs();

            var requestPort = args != null && args.Length > 0 && int.TryParse(args[0], out var rp) ? rp : 5559;
            var messagePort = args != null && args.Length > 1 && int.TryParse(args[1], out var mp) ? mp : 5560;

            var connectionSettings = new ConnectionSettings(
                RequestUrl: $"tcp://localhost:{requestPort}",
                MessageUrl: $"tcp://localhost:{messagePort}");

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
