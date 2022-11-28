using System;
using System.Threading.Tasks;
using Blazorise.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Blazorise.Demo.Bootstrap.Server.Controllers;

[Route( "weatherforecast" )]
[ApiController]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private static readonly Random Rand = new Random();

    [HttpGet]
    [Route( "getforecast" )]
    public Task<WeatherForecast> GetForecast()
    {
        return Task.FromResult( new WeatherForecast
        {
            Date = DateTime.Now,
            TemperatureC = Rand.Next( -20, 55 ),
            Summary = Summaries[Rand.Next( Summaries.Length )]
        } );
    }
}
