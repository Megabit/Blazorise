#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Limits options define the limits per axis for pan and zoom.
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#limit-options</para>
/// </summary>
public class ChartZoomLimitsOptions
{
    /// <summary>
    /// Limits for x-axis.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomScaleLimitsOptions X { get; set; }

    /// <summary>
    /// Limits for y-axis.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomScaleLimitsOptions Y { get; set; }
}