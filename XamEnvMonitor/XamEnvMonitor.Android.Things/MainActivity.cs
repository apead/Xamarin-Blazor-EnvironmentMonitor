using System;

using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Things.Pio;
using Android.Util;
using Google.Android.Things.Contrib.Driver.Apa102;
using Google.Android.Things.Contrib.Driver.Bmx280;
using Google.Android.Things.Contrib.Driver.Button;
using Google.Android.Things.Contrib.Driver.Ht16k33;
using Google.Android.Things.Contrib.Driver.Pwmspeaker;
using Google.Android.Things.Contrib.Driver.Rainbowhat;
using Java.IO;
using Java.Lang;
using XamEnvMonitor.Droid.Callbacks;
using XamEnvMonitor.Droid.Enum;
using XamEnvMonitor.Droid.Helpers;
using XamEnvMonitor.Droid.Hubs;
using XamEnvMonitor.Droid.Messages;
using Button = Google.Android.Things.Contrib.Driver.Button.Button;
using Exception = System.Exception;
using Math = System.Math;
using Android.Content;

namespace XamEnvMonitor.Droid
{
    [Activity(Label = "Environment Monitor", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        private double _lastUpdatedPressure;
        private double _lastUpdatedTemperature;
        private double _lastUpdatedHumidity;
        private ImageView _imageView;
        public static string Tag = typeof(MainActivity).FullName;
        public SensorManager SensorManager { get; set; }

        private SensorManager.DynamicSensorCallback _dynamicSensorCallback;
        private ButtonInputDriver _buttonInputDriver;
        private Bmx280SensorDriver _bmx280SensorDriver;


        private AlphanumericDisplay _display;
        private Apa102 _ledRainbowStrip;
        private IGpio _led;
        private AlphaNumericDisplayMode _displayMode = AlphaNumericDisplayMode.Temperature;

        private int[] _rainbow = new int[7];
        private const int LedstripBrightness = 1;
        public const float BarometerRangeLow = 800f;
        public const float BarometerRangeHigh = 1080f;
        public const float BarometerRangeSunny = 1010f;
        public const float BarometerRangeRainy = 990f;

        private const int SpeakerReadyDelayMs = 300;

        private bool _useHubs = true;  //  Set this to true to use Azure Iot Hubs

        public Speaker Speaker;

        private WeatherDevice _weatherDevice;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            if (_useHubs)
                InitializeHubs();


            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            Initialize();
        }

        public void Initialize()
        {
            if (_useHubs)
                InitializeHubs();


            SensorManager = (SensorManager)GetSystemService(SensorService);
            _dynamicSensorCallback = new WeatherDynamicSensorCallback(this);
          

            PubSubHandler.GetInstance().Subscribe<TemperatureMessage>(OnTemperatureMessage);
            PubSubHandler.GetInstance().Subscribe<PressureMessage>(OnPressureMessage);
            PubSubHandler.GetInstance().Subscribe<HumidityMessage>(OnHumidityMessage);

            try
            {


                _ledRainbowStrip = new Apa102(BoardDefaults.GetSpiBus(), Apa102.Mode.Bgr);
                _ledRainbowStrip.Brightness = LedstripBrightness;
                for (var i = 0; i < _rainbow.Length; i++)
                {
                    float[] hsv = { i * 360f / _rainbow.Length, 1.0f, 1.0f };

                    _rainbow[i] = Color.HSVToColor(255, hsv);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                _ledRainbowStrip = null;
            }

            try
            {
                var pioService = PeripheralManager.Instance;
                _led = pioService.OpenGpio(BoardDefaults.GetLedGpioPin());
                _led.SetEdgeTriggerType(Gpio.EdgeNone);
                _led.SetDirection(Gpio.DirectionOutInitiallyLow);
                _led.SetActiveType(Gpio.ActiveHigh);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }

            try
            {
                _buttonInputDriver = new ButtonInputDriver(BoardDefaults.GetButtonGpioPin(),
                    Button.LogicState.PressedWhenLow,
                    (int)KeyEvent.KeyCodeFromString("KEYCODE_A"));
                _buttonInputDriver.Register();
                Log.Debug(Tag, "Initialized GPIO Button that generates a keypress with KEYCODE_A");
            }
            catch (Exception e)
            {
                throw new Exception("Error initializing GPIO button", e);
            }
            try
            {

                _bmx280SensorDriver = RainbowHat.CreateSensorDriver();
                // Register the drivers with the framework

                SensorManager.RegisterDynamicSensorCallback(_dynamicSensorCallback);

                _bmx280SensorDriver.RegisterTemperatureSensor();
                _bmx280SensorDriver.RegisterPressureSensor();
                _bmx280SensorDriver.RegisterHumiditySensor();
                Log.Debug(Tag, "Initialized I2C BMP280");
            }
            catch (IOException e)
            {
                throw new RuntimeException("Error initializing BMP280", e);
            }

    try
            {
                _display = new AlphanumericDisplay(BoardDefaults.GetI2cBus());
                _display.SetEnabled(true);
                _display.Clear();
                Log.Debug(Tag, "Initialized I2C Display");
            }
            catch (Exception e)
            {
                Log.Error(Tag, "Error initializing display", e);
                Log.Debug(Tag, "Display disabled");
                _display = null;
            }
        }

        private void InitializeHubs()
        {
            _weatherDevice = new WeatherDevice();

            _weatherDevice.AddTelemetry(new TelemetryFormat { Name = "temperature", DisplayName = "Temp", Type = "Double" },
                (double)0);
            _weatherDevice.AddTelemetry(new TelemetryFormat { Name = "pressure", DisplayName = "hPa", Type = "Double" },
                (double)0);

            _weatherDevice.AddTelemetry(new TelemetryFormat { Name = "humidity", DisplayName = "g", Type = "Double" },
                (double)0);



            _weatherDevice.DeviceId = "IgniteJhbThings";
            _weatherDevice.DeviceKey = "uJLwieHWZubfji7/TXoybKXbrSxdyFQgdYNl0YwZJA8=";
            _weatherDevice.HostName = "AzureIotEdgeJhbMsDugHub.azure-devices.net";

            _weatherDevice.SendTelemetryFreq = 60000;
            _weatherDevice.Connect();


            _weatherDevice.onReceivedMessage += WeatherDevice_onReceivedMessage;
        }

        private void WeatherDevice_onReceivedMessage(object sender, EventArgs e)
        {
            var receivedMessage = e as ReceivedMessageEventArgs;

            if (receivedMessage != null)
            {
                if (receivedMessage.Message.Name == "CustomDisplay")
                {
                    _displayMode = AlphaNumericDisplayMode.Custom;

                    _display.Display(receivedMessage.Message.MessageId);
                }

                Log.Debug(Tag, "Message Received: " + receivedMessage.ToString());


            }
        }

        private void OnTemperatureMessage(TemperatureMessage message)
        {
            Log.Debug(Tag, "Temperature Message: " + message.TemperatureReading);

            _lastUpdatedTemperature = message.TemperatureReading;

            if (_display != null)
            {
                if (_displayMode == AlphaNumericDisplayMode.Temperature)
                    UpdateDisplay(message.TemperatureReading);

                if (_useHubs)
                {
                    _weatherDevice.UpdateTelemetryData("temperature", message.TemperatureReading);
                    _weatherDevice.SendTelemetryData = true;
                }
            }
        }

        private void OnPressureMessage(PressureMessage message)
        {
            Log.Debug(Tag, "Pressure Message: " + message.PressureReading);

            _lastUpdatedPressure = message.PressureReading;

            if (_display != null)
            {
                if (_displayMode == AlphaNumericDisplayMode.Pressure)
                    UpdateDisplay(message.PressureReading);

                UpdateBarometer(message.PressureReading);

                if (_useHubs)
                {
                    _weatherDevice.UpdateTelemetryData("pressure", message.PressureReading);
                    _weatherDevice.SendTelemetryData = true;
                }
            }
        }

        private void OnHumidityMessage(HumidityMessage message)
        {
            Log.Debug(Tag, "Humidity Message: " + message.HumidityReading);

            _lastUpdatedHumidity = message.HumidityReading;

            if (_display != null)
            {
                if (_displayMode == AlphaNumericDisplayMode.Temperature)
                    UpdateDisplay(message.HumidityReading);

                if (_useHubs)
                {
                    _weatherDevice.UpdateTelemetryData("humidity", message.HumidityReading);
                    _weatherDevice.SendTelemetryData = true;
                }
            }
        }

        private void UpdateDisplay(double value)
        {
            try
            {
                _display.Display(value);

            }
            catch (Exception e)
            {
                Log.Error(Tag, "Error setting display", e);
            }
        }

        public void UpdateBarometer(float pressure)
        {
 if (_ledRainbowStrip == null)
                return;

            int[] clearColors = new int[_rainbow.Length];

            float t = (pressure - BarometerRangeLow) / (BarometerRangeHigh - BarometerRangeLow);
            int n = (int)Math.Ceiling(_rainbow.Length * t);
            n = Math.Max(0, Math.Min(n, _rainbow.Length));
            int[] colors = new int[_rainbow.Length];
            for (int i = 0; i < n; i++)
            {
                int ri = _rainbow.Length - 1 - i;
                colors[ri] = _rainbow[ri];
            }
            try
            {
                _ledRainbowStrip.Write(clearColors);
                _ledRainbowStrip.Write(colors);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.A)
            {
                _displayMode = AlphaNumericDisplayMode.Pressure;

                UpdateDisplay(_lastUpdatedPressure);

                try
                {
                    _led.Value = true;
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception);
                }
                return true;
            }
            return base.OnKeyUp(keyCode, e);
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.A)
            {
                _displayMode = AlphaNumericDisplayMode.Temperature;
                UpdateDisplay(_lastUpdatedTemperature);
                try
                {
                    _led.Value = false;
                }
                catch (Exception exception)
                {
                    System.Console.WriteLine(exception);
                }
                return true;
            }

            return base.OnKeyUp(keyCode, e);
        }

    }
}