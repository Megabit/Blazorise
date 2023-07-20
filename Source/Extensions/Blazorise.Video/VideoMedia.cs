﻿#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video;

/// <summary>
/// Defines the player media.
/// </summary>
public record VideoMedia
{
    /// <summary>
    /// Default constructor for the media source information.
    /// </summary>
    /// <param name="source">Media source, or url.</param>
    public VideoMedia( string source )
    {
        Source = source;
    }

    /// <summary>
    /// Default constructor for all the media information.
    /// </summary>
    /// <param name="source">Media source, or url.</param>
    /// <param name="type">Media type, eg. "video/mp4" or "video/webm".</param>
    /// <param name="size">Media size, eg. 720 or 1080.</param>
    public VideoMedia( string source, string type, int? size )
    {
        Source = source;
        Type = type;
        Size = size;
    }

    /// <summary>
    /// Gets or sets the media source, or url.
    /// </summary>
    [JsonPropertyName( "src" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the media type, eg. "video/mp4" or "video/webm".
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the media size, eg. 720 or 1080.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Size { get; set; }
}