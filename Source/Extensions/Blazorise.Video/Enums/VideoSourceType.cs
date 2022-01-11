using System.Text.Json.Serialization;

namespace Blazorise.Video
{
    [JsonConverter( typeof( System.Text.Json.Serialization.JsonStringEnumConverter ) )]
    public enum VideoSourceType
    {
        [JsonPropertyName( "video" )]
        Video,

        [JsonPropertyName( "audio" )]
        Audio,
    }
}
