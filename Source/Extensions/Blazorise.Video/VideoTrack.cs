#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video
{
    public record VideoTrack
    {
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Kind { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Label { get; set; }

        [JsonPropertyName( "srclang" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Language { get; set; }

        [JsonPropertyName( "src" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Source { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool Default { get; set; }
    }
}
