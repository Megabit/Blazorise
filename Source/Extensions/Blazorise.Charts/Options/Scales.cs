#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class Scales
    {
        [JsonPropertyName( "x" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Axis X { get; set; }

        [JsonPropertyName( "y" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Axis Y { get; set; }
    }
}
