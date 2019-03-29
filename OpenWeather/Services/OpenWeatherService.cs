using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenWeather.Configuration;
using OpenWeather.Models;
using OpenWeather.Models.ForeCast;
using OpenWeather.Models.Weather;

namespace OpenWeather.Services
{
    public class OpenWeatherService
    {
        private HttpClient _client;

        public OpenWeatherService()
        {
            _client = new HttpClient();
        }

        public async Task<WeatherData> GetWeatherAsync(string cityName)
        {
            WeatherData weatherData = null;

            return await GetWeatherFromEndpoint(GenerateWeatherRequestUri(cityName));
        }

        public async Task<ForecastData> GetForecastAsync(string cityName)
        {
            ForecastData forecastData = null;

            return await GetForecastFromEndpoint(GenerateForecastRequestUri(cityName));
        }

        public async Task<WeatherData> GetWeatherFromEndpoint(string query)
        {
            WeatherData weatherData = null;
            try
            {
               var response = await _client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    weatherData = JsonConvert.DeserializeObject<WeatherData>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\t\tERROR {0}", ex.Message);
            }

            return weatherData;
        }

        public async Task<ForecastData> GetForecastFromEndpoint(string query)
        {
            ForecastData forecastData = null;
            try
            {
                var response = await _client.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();


                    forecastData = JsonConvert.DeserializeObject<ForecastData>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\t\tERROR {0}", ex.Message);
            }

            return forecastData;
        }


        private static string GenerateWeatherRequestUri(string cityName)
        {
            var requestUri = Constants.OpenWeatherMapWeatherEndpoint;
            requestUri += $"?q={cityName}";
            requestUri += "&units=metric"; // or units=metric
            requestUri += $"&APPID={Constants.OpenWeatherMapApiKey}";
            return requestUri;
        }

        private static string GenerateForecastRequestUri(string cityName)
        {
            var requestUri = Constants.OpenWeatherMapForecastEndpoint;
            requestUri += $"?q={cityName}";
            requestUri += "&units=metric"; // or units=metric
            requestUri += $"&APPID={Constants.OpenWeatherMapApiKey}";
            return requestUri;
        }

    }
}
