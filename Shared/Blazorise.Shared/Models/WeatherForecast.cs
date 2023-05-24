using System;
using System.Text.Json.Serialization;

namespace Blazorise.Shared.Models;

public class WeatherForecast
{
    [JsonPropertyName( "date" )]
    public DateTime Date { get; set; }

    [JsonPropertyName( "temperatureC" )]
    public int TemperatureC { get; set; }

    [JsonPropertyName( "summary" )]
    public string Summary { get; set; }
}
