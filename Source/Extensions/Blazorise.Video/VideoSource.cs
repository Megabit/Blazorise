#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video
{
    public record VideoSource
    {
        #region Constructors

        public VideoSource( string source )
        {
            Medias = new ValueEqualityList<VideoMedia>
            {
                new VideoMedia
                {
                    Source = source,
                }
            };

            Indexed = false;
        }

        public VideoSource( string[] sources )
        {
            if ( sources != null )
            {
                foreach ( var source in sources )
                {
                    Medias = new ValueEqualityList<VideoMedia>
                    {
                        new VideoMedia
                        {
                            Source = source,
                        }
                    };
                }
            }

            Indexed = true;
        }

        #endregion

        #region Methods

        public static implicit operator VideoSource( string source )
        {
            return new( source );
        }

        #endregion

        #region Properties

        public bool Indexed { get; private set; }

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
        public ValueEqualityList<VideoMedia> Tracks { get; set; }

        #endregion
    }
}
