using Messenger;

namespace XamEnvMonitor.Droid.Messages
{
    public class HumidityMessage : MessengerMessage
    {
        public double HumidityReading { get; set; }

        public HumidityMessage(object sender, double humidity) : base(sender)
        {
            HumidityReading = humidity;
        }
    }
}