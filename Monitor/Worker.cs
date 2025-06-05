using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monitor.Settings;

namespace Monitor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppStartSettings _settings;
        public Worker(ILogger<Worker> logger, IOptions<AppStartSettings> options)
        {
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
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

    public class Worker1 : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpListener _listener;
        public Worker1(ILogger<Worker> logger, IOptions<AppStartSettings> options)
        {
            _logger = logger;
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _listener.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();

                var request = context.Request;
                var response = context.Response;

                string requestBody = "";
                if (request.HasEntityBody)
                {
                    using var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                    requestBody = await reader.ReadToEndAsync(stoppingToken);
                }

                var point = JsonSerializer.Deserialize<Point>(requestBody);

                // Формируем ответ
                string responseString = $"Принято сообщение: {requestBody}";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/plain; charset=utf-8";
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length, stoppingToken);
                response.OutputStream.Close();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
