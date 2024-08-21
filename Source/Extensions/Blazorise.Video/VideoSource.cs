#region Using directives
using System.Linq;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video;

/// <summary>
/// Defines the player source.
/// </summary>
public record VideoSource
{
    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public VideoSource()
    {
    }

    /// <summary>
    /// Default constructor for single source.
    /// </summary>
    /// <param name="source">Source address.</param>
    public VideoSource( string source )
    {
        Medias = new ValueEqualityList<VideoMedia>
        {
            new VideoMedia( source )
        };
    }

    /// <summary>
    /// Default constructor for multiple source.
    /// </summary>
    /// <param name="sources">Source addresses.</param>
    public VideoSource( string[] sources )
    {
        if ( sources != null )
        {
            Medias = new ValueEqualityList<VideoMedia>();

            foreach ( var source in sources )
            {
                Medias.Add( new VideoMedia( source ) );
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Implicit converted for the string source.
    /// </summary>
    /// <param name="source">Single source address.</param>
    public static implicit operator VideoSource( string source )
    {
        return new( source );
    }

    /// <summary>
    /// Implicit converted for the array of string source.
    /// </summary>
    /// <param name="sources">Multiple source addresses.</param>
    public static implicit operator VideoSource( string[] sources )
    {
        return new( sources );
    }

    #endregion

    #region Properties

    /// <summary>
    /// True if the source contains more than one media element.
    /// </summary>
    public bool HasMultipleMedia => Medias?.Count > 1;

    /// <summary>
    /// Either video or audio. Note: YouTube and Vimeo are currently not supported as audio sources.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public VideoSourceType? Type { get; set; } = VideoSourceType.Video;

    /// <summary>
    /// Optional. Title of the new media. Used for the aria-label attribute on the play button, and outer container. YouTube and Vimeo are populated automatically.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Title { get; set; }

    /// <summary>
    /// This is an array of media sources. For HTML5 media, the properties of this object are mapped directly to HTML attributes so more can be added to the object if required.
    /// </summary>
    [JsonPropertyName( "sources" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ValueEqualityList<VideoMedia> Medias { get; set; }

    /// <summary>
    /// The URL for the poster image (HTML5 video only).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Poster { get; set; }

    /// <summary>
    /// An array of track objects. Each element in the array is mapped directly to a track element and any keys mapped directly to HTML attributes so
    /// as in the example above, it will render as <code>&lt;track kind="captions" label="English" srclang="en" src="https://cdn.selz.com/plyr/1.0/example_captions_en.vtt" default&gt;</code>
    /// and similar for the French version. Booleans are converted to HTML5 value-less attributes.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ValueEqualityList<VideoTrack> Tracks { get; set; }

    #endregion
}