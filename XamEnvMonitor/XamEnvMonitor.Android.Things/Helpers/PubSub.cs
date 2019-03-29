using Messenger;

namespace XamEnvMonitor.Droid.Helpers
{
    public static class PubSubHandler
    {
        private static IMessenger _messenger = new MessengerHub();

        public static IMessenger GetInstance()
        {
            return _messenger;
        }
    }
}