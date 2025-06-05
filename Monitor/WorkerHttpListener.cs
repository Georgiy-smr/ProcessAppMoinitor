using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Monitor;

public class WorkerHttpListener : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpListener _listener;
    public WorkerHttpListener(
        ILogger<Worker> logger,
        HttpListener listener)
    {
        _logger = logger;
        _listener = listener;
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