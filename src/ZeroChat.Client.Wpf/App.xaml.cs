using Microsoft.Extensions.Configuration;

namespace ZeroChat.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var appSettings = configuration.Get<AppSettings>();

            var options = new ServiceProviderOptions { ValidateOnBuild = true };
            
            _serviceProvider = new ServiceCollection()
                .ConfigureServices(appSettings)
                .BuildServiceProvider(options);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _serviceProvider.GetRequiredService<MainWindow>().Show();
        }
    }
}
