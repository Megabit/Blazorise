#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Video
{
    public record VideoMedia
    {
        [JsonPropertyName( "src" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Source { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string Type { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Size { get; set; }
    }
}
