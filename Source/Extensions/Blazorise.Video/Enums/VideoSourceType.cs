using System.Text.Json.Serialization;
using Blazorise.Utilities.JsonConverters;

namespace Blazorise.Video;

/// <summary>
/// Defines the media source type.
/// </summary>
[JsonConverter( typeof( CamelCaseEnumJsonConverter ) )]
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