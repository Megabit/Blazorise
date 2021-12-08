#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class ChartSize
    {
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Width { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public int? Height { get; set; }
    }
}
