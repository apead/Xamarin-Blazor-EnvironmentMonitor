using System;
using System.Windows.Input;
using OpenWeather.Services;
using Xamarin.Forms;

namespace XamEnvMonitor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _city;

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }



        private string _temperature;

        public string Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }

        private string _pressure;

        public string Pressure
        {
            get => _pressure;
            set => SetProperty(ref _pressure, value);
        }

        private string _humidity;
        public string Humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity, value);
        }


        private string _tempMin;
        public string TempMin
        {
            get => _tempMin;
            set => SetProperty(ref _tempMin, value);
        }


        private string _tempMax;
        public string TempMax
        {
            get => _tempMax;
            set => SetProperty(ref _tempMax, value);
        }

        private string _windSpeed;
        public string WindSpeed
        {
            get => _windSpeed;
            set => SetProperty(ref _windSpeed, value);
        }

        private string _iconUrl;
        public string IconUrl
        {
            get => _iconUrl;
            set => SetProperty(ref _iconUrl, value);
        }
        public ICommand RefreshCommand { protected set; get; }


        public MainViewModel()
        {
            City = "Seattle, United States";

            RefreshCommand = new Command(Refresh);

            Device.StartTimer(TimeSpan.FromMinutes(5), () =>
            {
                //Refresh(null);

                return true; 
            });
        }


        public async void Refresh(object param)
        {
            /*
            var weatherService = new OpenWeatherService();

            var weatherData = await weatherService.GetWeatherAsync(City);

            TempMin = weatherData.Main.TempMin + "°C";
            TempMax = weatherData.Main.TempMax + "°C";
            WindSpeed = weatherData.Wind.Speed + " m/s";
            Pressure = weatherData.Main.Pressure + " hPa";
            Humidity = weatherData.Main.Humidity + "%";
            Temperature = weatherData.Main.Temperature + "°C";

            if (weatherData.Weather.Length > 0)
                IconUrl = "http://openweathermap.org/img/w/" + weatherData.Weather[0].Icon+".png";
            else IconUrl = string.Empty;
            */
        }
    }
}
