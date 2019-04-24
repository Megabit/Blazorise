using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Blazorise.Demo.Bootstrap.Server.Controllers
{
    [Route( "api/[controller]" )]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet( "[action]" )]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays( index ),
                TemperatureC = rng.Next( -20, 55 ),
                Summary = Summaries[rng.Next( Summaries.Length )]
            } );
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; internal set; }
        public int TemperatureC { get; internal set; }
        public string Summary { get; internal set; }
    }
}
