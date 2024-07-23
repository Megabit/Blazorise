#region Using directives
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video;

/// <summary>
/// Defines the player media.
/// </summary>
public record VideoMedia
{
    #region Members

    // Create a dictionary to map file extensions to MIME types
    private static Dictionary<string, string> VideoTypes = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
    {
        { "mp4", "video/mp4" },
        { "webm", "video/webm" },
        { "3gp", "video/3gp" },
        { "ogg", "video/ogg" },
        { "avi", "video/avi" },
        { "mpeg", "video/mpeg" }
    };

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for the media source information.
    /// </summary>
    /// <param name="source">Media source, or url.</param>
    public VideoMedia( string source )
        : this( source, null, null, null )
    {
    }

    /// <summary>
    /// Default constructor for all the media information.
    /// </summary>
    /// <param name="source">Media source, or url.</param>
    /// <param name="type">Media type, eg. "video/mp4" or "video/webm".</param>
    /// <param name="size">Media size, eg. 720 or 1080.</param>
    public VideoMedia( string source, string type, int? size )
        : this( source, type, null, size )
    {
    }

    /// <summary>
    /// Default constructor for all the media information.
    /// </summary>
    /// <param name="source">Media source, or url.</param>
    /// <param name="type">Media type, eg. "video/mp4" or "video/webm".</param>
    /// <param name="width">Media width, eg. 720 or 1080.</param>
    /// <param name="height">Media height, eg. 720 or 1080.</param>
    public VideoMedia( string source, string type, int? width, int? height )
    {
        Source = source;
        Type = type;
        Width = width;
        Height = height;

        if ( string.IsNullOrWhiteSpace( type ) )
            Type = GetVideoType( source );
    }

    #endregion

    #region Methods

    private static string GetVideoType( string url )
    {
        if ( string.IsNullOrWhiteSpace( url ) )
            return null;

        // Extract the file extension from the URL
        var uri = new Uri( url );
        var fileExtension = System.IO.Path.GetExtension( uri.LocalPath ).TrimStart( '.' ).ToLower();

        // Check if the extracted extension is in the dictionary
        return VideoTypes.ContainsKey( fileExtension ) ? VideoTypes[fileExtension] : null;
    }

    #endregion

    #region Properties

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
    [Obsolete( "Size property is deprecated and will be removed, please use the Height property instead." )]
    public int? Size { get => Height; set => Height = value; }

    /// <summary>
    /// Gets or sets the media width.
    /// </summary>
    [JsonPropertyName( "width" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the media height.
    /// </summary>
    [JsonPropertyName( "height" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Height { get; set; }

    #endregion
}