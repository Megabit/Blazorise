#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

public class DoughnutChartOptions : PieChartOptions
{
    /// <summary>
    /// The portion of the chart that is cut out of the middle. If string and ending with '%', percentage of the chart radius. number is considered to be pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public new object Cutout { get; set; }
}