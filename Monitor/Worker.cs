using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monitor.Services;
using Monitor.Settings;

namespace Monitor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppStartSettings _settings;
        private readonly IServiceProvider _provider;
        public Worker(
            ILogger<Worker> logger,
            IOptions<AppStartSettings> options, 
            IServiceProvider provider)
        {
            _settings = options.Value;
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using var scope = _provider.CreateAsyncScope();
                var delayService = scope.ServiceProvider.GetRequiredService<IDelay>();
                if(delayService.IsCanSleep)
                    await Task.Delay(delayService.GetDelay(), stoppingToken).ConfigureAwait(false);

                string processName = _settings.NameApp;
                string processPath = _settings.AppPath;
                int delay = _settings.TimeInterval;

                if (!File.Exists(processPath))
                {
                    _logger.LogError($"File {processPath ?? string.Empty} not exists");
                    throw new ArgumentException($"{processPath}");
                }
                try
                {
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Process:{1} starting {2}", processName, DateTimeOffset.Now);
                        }
                        Process.Start(processPath);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e,$"Error starting process {processPath ?? string.Empty}");
                }
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
