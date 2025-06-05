using MediatR;

namespace MonitorAppLib
{
    public class SetMonitorDelay : INotification
    {
        public int Delay { get; set; }
        public override string ToString() => $"Delay is {Delay}";
    }
}
