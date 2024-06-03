#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video;

/// <summary>
/// Defines the track object.
/// </summary>
public record VideoTrack
{
    /// <summary>
    /// Gets or sets the track type, eg. "captions".
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Kind { get; set; }

    /// <summary>
    /// Gets or sets the track display name.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Label { get; set; }

    /// <summary>
    /// Gets or sets the track language.
    /// </summary>
    [JsonPropertyName( "srclang" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the track source.
    /// </summary>
    [JsonPropertyName( "src" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Source { get; set; }

    /// <summary>
    /// True if the track should be default.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Default { get; set; }
}