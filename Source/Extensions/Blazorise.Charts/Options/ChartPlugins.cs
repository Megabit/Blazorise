#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    public class ChartPlugins
    {
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartLegend Legend { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartTooltips Tooltips { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartTitle Title { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartSubtitle Subtitle { get; set; }

        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public ChartDecimation Decimation { get; set; }
    }
}
