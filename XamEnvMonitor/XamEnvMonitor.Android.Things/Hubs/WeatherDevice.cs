using XamEnvMonitor.Droid.Helpers;

namespace XamEnvMonitor.Droid.Hubs
{
    public class WeatherDevice : RemoteMonitoringDevice
    {
        public WeatherDevice()
        {
            this.DeviceId = Settings.DeviceId;
            this.DeviceKey = Settings.DeviceKey;
            this.HostName = Settings.HostName;
        }
        public bool CheckConfig()
        {
            if (((this.DeviceId != null) && (this.DeviceKey != null) && (this.HostName != null) &&
                 (this.DeviceId != "") && (this.DeviceKey != "") && (this.HostName != "")))
            {
                Settings.DeviceId = this.DeviceId;
                Settings.DeviceKey = this.DeviceKey;
                Settings.HostName = this.HostName;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
