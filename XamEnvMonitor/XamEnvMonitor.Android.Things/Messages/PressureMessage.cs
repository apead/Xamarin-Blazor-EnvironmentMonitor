using Messenger;

namespace XamEnvMonitor.Droid.Messages
{
    public class PressureMessage : MessengerMessage
    {
        public float PressureReading { get; set; }

        public PressureMessage(object sender, float pressure) : base(sender)
        {
            PressureReading = pressure;
        }
    }
}