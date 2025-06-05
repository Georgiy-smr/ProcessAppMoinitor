using System.Configuration;
using System.Data;
using System.Net;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monitor.Settings;
using Serilog;

namespace Monitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IHost? _host;
        public IHost Host =>
            _host
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

            services.AddSingleton<HttpListener>(opt =>
            {
                var settings = opt.GetRequiredService<IOptions<AppStartSettings>>();
                var httpListener = new HttpListener();
                httpListener.Prefixes.Add(settings.Value.UriPrefix);

                return httpListener;
            });

            services.AddHostedService<Worker>();
            services.AddHostedService<WorkerHttpListener>();
        }

        public IServiceProvider Services => Host.Services;

        protected override async void OnStartup(StartupEventArgs e)
        {
            using var scope = Services.CreateScope();
            CancellationTokenSource token = new();

            var log = scope.ServiceProvider.GetRequiredService<ILogger<App>>(); 
            log.LogInformation("Monitor App is Started!");
            await Host.RunAsync(token: token.Token);
            //base.OnStartup(e);
        }
    }

}
