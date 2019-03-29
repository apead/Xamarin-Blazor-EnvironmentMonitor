using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenWeather.Services;
using System.Linq;

namespace OpenWeather.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var weatherService = new OpenWeatherService();

            var forecastData = await weatherService.GetForecastAsync("Johannesburg");

            var orderedData = forecastData.ForeCastList.OrderBy(x => x.DateTimeStamp).Take(8).Select(y => new { Date = y.DateText, Temp = y.Main.Temp }).ToList();


            var values = orderedData.Select(x => x.Temp).ToList();

            var dynValues = new List<dynamic>();

            foreach (var value in values)
                dynValues.Add(value);

          
            Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
}
