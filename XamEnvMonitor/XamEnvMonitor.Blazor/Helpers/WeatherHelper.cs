using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJs.Blazor.ChartJS.BarChart;
using ChartJs.Blazor.ChartJS.BarChart.Dataset;
using ChartJs.Blazor.ChartJS.Common.Legends;
using ChartJs.Blazor.ChartJS.LineChart;
using ChartJs.Blazor.ChartJS.MixedChart;
using ChartJs.Blazor.Util.Color;
using OpenWeather.Configuration;
using OpenWeather.Services;

namespace XamEnvMonitor.Blazor.Helpers
{
    public static class WeatherHelper
    {
        public async static Task<BarChartConfig> GetBarChartConfig()
        {
            var weatherService = new OpenWeatherService();

            var forecastData = await weatherService.GetForecastAsync(XamEnvMonitor.Blazor.Constants.City);

            var orderedData = forecastData.ForeCastList.OrderBy(x => x.DateTimeStamp).Take(8).Select(y => new { Date = y.DateText, Temp = y.Main.Temp }).ToList();


            var values = orderedData.Select(x => x.Temp).ToList();
            var dynValues = new List<dynamic>();

            foreach (var value in values)
                dynValues.Add(value);



            var barChartConfig = new BarChartConfig()
            {
                CanvasId = "forcastBarChartCanvas",
                Options = new BarChartOptions
                {
                    Text = "24 Hour Forecast",
                    Display = true,
                    Responsive = true
                },
                Data = new BarChartData
                {
                    Labels = orderedData.Select(x => x.Date).ToList(),
                    Datasets = new List<BaseBarChartDataset>
                    {
                        new BarChartDataset
                        {
                            Label = "Temperature",
                            BackgroundColor = ColorUtil.ColorString(0,50,255),
                            BorderColor =  ColorUtil.ColorString(0,255,128),
                            Data = dynValues
                        }
                    }
                }
            };

            return barChartConfig;
        }

        public async static Task<MixedChartConfig> GetMixedChartConfig()
        {
            var weatherService = new OpenWeatherService();

            var forecastData = await weatherService.GetForecastAsync(XamEnvMonitor.Blazor.Constants.City);

            var orderedData = forecastData.ForeCastList.OrderBy(x => x.DateTimeStamp).Take(8).Select(y => new { Date = y.DateText, Temp = y.Main.Temp, MinTemp = y.Main.TempMin, MaxTemp = y.Main.TempMax }).ToList();


            var values = orderedData.Select(x => x.Temp).ToList();
            var tempValues = new List<dynamic>();
            var tempMinValues = new List<dynamic>();
            var tempMaxValues = new List<dynamic>();

            foreach (var value in orderedData)
            {
                tempValues.Add(value.Temp);
                tempMinValues.Add(value.MinTemp);
                tempMaxValues.Add(value.MaxTemp);
            }


            var  mixedChartConfig = new MixedChartConfig
            {
                CanvasId = "mixedChart",
                Options = new MixedChartOptions
                {
                    Text = "Forcasted Temperature History",
                    Display = true,
                    Responsive = true,
                    Legend = new Legend
                    {
                        Position = LegendPosition.TOP.ToString(),
                        Reverse = true,
                        Labels = new Labels
                        {
                            UsePointStyle = true,
                            BoxWidth = 85,
                            Padding = 55,
                            FontSize = 15,
                            FontColor = ColorUtil.ColorHexString(205, 205, 205),
                            FontStyle = "Helvetica"
                        }
                    }
                },
                Data = new MixedChartData
                {
                    Labels = orderedData.Select(x => x.Date).ToList(),
                    Datasets = new List<IMixableDataset>
                {
                    new BarChartDataset
                    {
                        Label = "Forecasted Temperature",
                        BackgroundColor = "#4465fe",
                        BorderColor = "#4465fe",
                        Data = tempValues
                    },
                    new LineChartDataset
                    {
                        BackgroundColor = "#95e086",
                        BorderColor = "#95e086",
                        Label = "Minimum Temperature",
                        Data = tempMinValues,
                        Fill = true,
                        BorderWidth = 2,
                        PointRadius = 3,
                        PointBorderWidth = 1
                    }
                    ,
                    new LineChartDataset
                    {
                        BackgroundColor = "#ff6384",
                        BorderColor = "#ff6384",
                        Label = "Maximum Temperature",
                        Data = tempMaxValues,
                        Fill = true,
                        BorderWidth = 2,
                        PointRadius = 3,
                        PointBorderWidth = 1
                    }
                }
                }
            };



            /*         test.Datasets


                     var mixedChartConfig = new MixedChartConfig()
                     {
                         CanvasId = "forcastBarChartCanvas",
                         Options = new MixedChartOptions()
                         {
                             Text = "24 Hour Forecast",
                             Display = true,
                             Responsive = true
                         },
                         Data = new List<IMixableDataset>
                         {
                             new BarChartDataset
                             {
                                 Label = "1'st dataset",
                                 BackgroundColor = "#4465fe",
                                 BorderColor = "#4465fe",
                                 Data = new List<object> {19, 12, 5, 3, 3, 2}
                             },
                             new LineChartDataset
                             {
                                 BackgroundColor = "#ff6384",
                                 BorderColor = "#ff6384",
                                 Label = "2'nd dataset",
                                 Data = new List<object> {19, 12, 5, 3, 3, 2},
                                 Fill = false,
                                 BorderWidth = 2,
                                 PointRadius = 3,
                                 PointBorderWidth = 1
                             }
                             ,
                             new BarChartDataset
                             {
                                 Label = "3'rd dataset",
                                 BackgroundColor = "#cc65fe",
                                 BorderColor = "#cc65fe",
                                 Data = new List<object> {19, 12, 5, 3, 3, 2}
                             }
                         }
                     };
                     */
            return mixedChartConfig;
        }
    }
}
