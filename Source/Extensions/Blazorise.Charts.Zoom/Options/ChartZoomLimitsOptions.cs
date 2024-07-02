#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// The Chart Zoom plugin limits options
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#limit-options</para>
/// </summary>
public class ChartZoomLimitsOptions
{
    /// <summary>
    /// Limits for x-axis
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomScaleLimits X { get; set; }

    /// <summary>
    /// Limits for y-axis
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomScaleLimits Y { get; set; }
}
