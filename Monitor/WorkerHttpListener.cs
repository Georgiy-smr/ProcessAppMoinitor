using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonitorAppLib;

namespace Monitor;

public class WorkerHttpListener : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpListener _listener;
    private readonly IMediator _mediator;

    public WorkerHttpListener(
        ILogger<Worker> logger,
        HttpListener listener,
        IMediator mediator)
    {
        _logger = logger;
        _listener = listener;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Listener starting...");
        _listener.Start();

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync();
                }
                catch (HttpListenerException ex) when (ex.ErrorCode == 995) // ERROR_OPERATION_ABORTED
                {
                    // Listener был остановлен
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting HTTP context");
                    continue; // Попробовать снова
                }

                var request = context.Request;
                string requestBody = "";

                if (request.HasEntityBody)
                {
                    try
                    {
                        using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                        requestBody = await reader.ReadToEndAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error reading request body");
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                        continue;
                    }
                }

                SetMonitorDelay? delayNotify = null;
                try
                {
                    delayNotify = JsonSerializer.Deserialize<SetMonitorDelay>(requestBody);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Invalid JSON received");
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    continue;
                }

                if (delayNotify is { Delay: > 0 })
                {
                    _logger.LogInformation($"Received delay notification: {delayNotify}");
                    try
                    {
                        await _mediator.Publish(delayNotify, stoppingToken).ConfigureAwait(false);
                        context.Response.StatusCode = 200;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error publishing notification");
                        context.Response.StatusCode = 500;
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                }

                context.Response.Close();
            }
        }
        finally
        {
            _listener.Stop();
            _logger.LogInformation("Listener stopped.");
        }
    }
}