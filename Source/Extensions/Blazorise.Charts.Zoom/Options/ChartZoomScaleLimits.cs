#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// The Chart Zoom plugin scale limits
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#scale-limits</para>
/// </summary>
public class ChartZoomScaleLimits
{
    /// <summary>
    /// Minimum allowed value for scale.min
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Min { get; set; }

    /// <summary>
    /// Maximum allowed value for scale.max
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Max { get; set; }

    /// <summary>
    /// Minimum allowed range (max - min). This defines the max zoom level.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? MinRange { get; set; }
}