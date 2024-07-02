#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Defines the chart plugins options.
/// </summary>
public class ChartPlugins
{
    /// <summary>
    /// Configuration for the chart legend.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartLegend Legend { get; set; }

    /// <summary>
    /// Configuration for the chart tooltips.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartTooltips Tooltips { get; set; }

    /// <summary>
    /// Configuration for the chart title.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartTitle Title { get; set; }

    /// <summary>
    /// Configuration for the chart subtitle.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartSubtitle Subtitle { get; set; }

    /// <summary>
    /// Configuration for chart data decimation.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartDecimation Decimation { get; set; }

}
