using System;

namespace QuestBoard.Controllers
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public DateTime Date { get; internal set; }
        public int TemperatureC { get; internal set; }
        public string Summary { get; internal set; }
    }
}