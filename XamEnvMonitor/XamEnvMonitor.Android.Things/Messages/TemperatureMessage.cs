using Messenger;

namespace XamEnvMonitor.Droid.Messages
{
    public class TemperatureMessage : MessengerMessage
    {
        public double TemperatureReading { get; set; }

        public TemperatureMessage(object sender, double temperature) : base(sender)
        {
            TemperatureReading = temperature;
        }
    }
}