#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video
{
    /// <summary>
    /// Defines the player media.
    /// </summary>
    public record VideoMedia
    {
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
        /// Gets or sets the media type, eg. 720 or 1080.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Size { get; set; }
    }
}
