using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Split directions
/// </summary>
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum SplitDirection
{
    /// <summary>
    /// Split horizontally
    /// </summary>
    [JsonPropertyName( "horizontal" )]
    Horizontal,

    /// <summary>
    /// Split vertically
    /// </summary>
    [JsonPropertyName( "vertical" )]
    Vertical
}