using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Gutter alignment between elements
/// </summary>
[JsonConverter( typeof( JsonStringEnumConverter ) )]
public enum SplitGutterAlignment
{
    /// <summary>
    /// Shrinks the first element to fit the gutter
    /// </summary>
    [JsonPropertyName( "start" )]
    Start,

    /// <summary>
    /// Shrinks both elements by the same amount so the gutter sits between
    /// </summary>
    [JsonPropertyName( "center" )]
    Center,

    /// <summary>
    /// Shrinks the second element to fit the gutter
    /// </summary>
    [JsonPropertyName( "end" )]
    End
}