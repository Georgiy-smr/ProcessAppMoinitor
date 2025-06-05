using Microsoft.Extensions.Logging;

namespace Monitor.Services;

public class DelayService : IDelay, ISetDelay
{
    private readonly ILogger<DelayService> _logger;
    public DelayService(ILogger<DelayService> logger)
    {
        _logger = logger;
    }

    private int _delay;
    public bool IsCanSleep => _delay > 0;
    public int GetDelay()
    {
        var delay = _delay;
        _delay = 0;
        _logger.LogInformation($"Get Delay is {delay}");

        return delay;
    }

    public void SetDelay(int delay) 
    {
        _logger.LogInformation($"Set new Delay is {delay}");
        _delay = delay;
    }
}