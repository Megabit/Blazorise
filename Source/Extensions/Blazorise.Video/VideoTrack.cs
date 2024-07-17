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
    ///  Default constructor for the track source information.
    /// </summary>
    /// <param name="source">Track source address.</param>
    public VideoTrack( string source )
    {
        Source = source;
    }

    /// <summary>
    ///  Default constructor for the track source information.
    /// </summary>
    /// <param name="source">Track source address.</param>
    /// <param name="language">Track language.</param>
    public VideoTrack( string source, string language )
        : this( source )
    {
        Language = language;
    }

    /// <summary>
    /// Gets or sets the track type, eg. "vtt".
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the track kind, eg. "subtitles".
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