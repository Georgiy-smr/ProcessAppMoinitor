using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Monitor.Settings;
using MonitorApp;
using Serilog;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? _Host;
        public static IHost Host =>
            _Host
            ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        internal static void ConfigureServices
            (HostBuilderContext host, IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.Configure<AppStartSettings>(
                host.Configuration.GetSection(nameof(AppStartSettings)));
            services.AddSerilog(Log.Logger);
            services.AddHostedService<Worker>();
        }

        public static IServiceProvider Services => Host.Services;

        protected override async void OnStartup(StartupEventArgs e)
        {

            var host = Host;
            using var scope = Services.CreateScope();
            CancellationTokenSource token = new();



            await scope.ServiceProvider.GetRequiredService<IHostedService>().StartAsync(token.Token);

            var log = scope.ServiceProvider.GetRequiredService<ILogger<App>>(); 
            log.LogInformation("Monitor App is Started!");

            base.OnStartup(e);
        }
    }

}
