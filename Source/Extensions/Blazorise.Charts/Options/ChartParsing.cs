#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class ChartParsing
    {
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string XAxisKey { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public string YAxisKey { get; set; }
    }
}
