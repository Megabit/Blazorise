#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class Scales
    {
        [DataMember( Name = "x", EmitDefaultValue = false )]
        [JsonPropertyName( "x" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Axis X { get; set; }

        [DataMember( Name = "y", EmitDefaultValue = false )]
        [JsonPropertyName( "y" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Axis Y { get; set; }
    }
}
