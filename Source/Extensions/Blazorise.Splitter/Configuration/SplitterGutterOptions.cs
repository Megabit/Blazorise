using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

public class SplitterGutterOptions
{
    /// <summary>
    /// Defines the custom background image for the gutter element.
    /// </summary>
    [JsonPropertyName( "backgroundImage" )]
    public string BackgroundImage { get; init; }
}
