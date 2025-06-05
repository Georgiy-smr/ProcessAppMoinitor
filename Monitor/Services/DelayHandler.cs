using MediatR;
using MonitorAppLib;

namespace Monitor.Services;

public class DelayHandler : INotificationHandler<SetMonitorDelay>
{
    private readonly ISetDelay _delaySet;

    public DelayHandler(ISetDelay delaySet)
    {
        _delaySet = delaySet;
    }

    public Task Handle(SetMonitorDelay notification, CancellationToken cancellationToken)
    {
        _delaySet.SetDelay(notification.Delay);
        return Task.CompletedTask;
    }
}