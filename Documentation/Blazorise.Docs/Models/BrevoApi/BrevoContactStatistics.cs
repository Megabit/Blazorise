using System.Text.Json.Serialization;

namespace Blazorise.Docs.Models.BrevoApi;

public class BrevoContactStatistics
{
    [JsonPropertyName( "messagesSent" )]
    public int MessagesSent { get; set; }

    [JsonPropertyName( "hardBounces" )]
    public int HardBounces { get; set; }

    [JsonPropertyName( "softBounces" )]
    public int SoftBounces { get; set; }

    [JsonPropertyName( "complaints" )]
    public int Complaints { get; set; }
}