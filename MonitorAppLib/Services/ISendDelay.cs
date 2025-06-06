using System.Net;
using Microsoft.Extensions.Logging;
using MonitorAppLib.Clients;

namespace MonitorAppLib.Services;

public interface ISendDelay
{
    Task<HttpStatusCode> SendDelayAsync(TimeSpan delay);
}

internal class SendDelayService : ISendDelay
{
    private readonly ILogger<SendDelayService> _logger;
    private readonly DelayClient _client;
    public SendDelayService(
        ILogger<SendDelayService> logger,
        DelayClient client)
    {
        _logger = logger;
        _client = client;
    }
    private void LogError(string message) => _logger.LogError($"{nameof(SendDelayService)}:" + message);
    private void LogInfo(string message) => _logger.LogInformation($"{nameof(SendDelayService)}:" + message);

    public async Task<HttpStatusCode> SendDelayAsync(TimeSpan delay)
    {
        LogInfo($"new delay {delay} ");
        try
        {
            return await _client.SendDelay((int)delay.TotalMilliseconds).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            LogError($"Error: {e}");
            return HttpStatusCode.BadRequest;
        }
    }
}
