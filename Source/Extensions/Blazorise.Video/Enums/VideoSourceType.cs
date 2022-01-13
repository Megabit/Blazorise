using System.Text.Json.Serialization;

namespace Blazorise.Video
{
    /// <summary>
    /// Defines the media source type.
    /// </summary>
    [JsonConverter( typeof( System.Text.Json.Serialization.JsonStringEnumConverter ) )]
    public enum VideoSourceType
    {
        /// <summary>
        /// Defines the video media.
        /// </summary>
        [JsonPropertyName( "video" )]
        Video,

        /// <summary>
        /// Defines the audio media.
        /// </summary>
        [JsonPropertyName( "audio" )]
        Audio,
    }
}
