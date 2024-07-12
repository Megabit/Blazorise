#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html
/// </summary>
public class ChartZoomPluginOptions
{

    /// <summary>
    /// Zoom options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomOptions Zoom { get; set; } = new ChartZoomOptions
    {
        Wheel = new ChartZoomWheelOptions()
        {
            Enabled = true
        },
        Pinch = new ChartZoomPinchOptions()
        {
            Enabled = true
        },
        Mode = "xy",
    };

    /// <summary>
    /// Pan options.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomPanOptions Pan { get; set; } = new();

    /// <summary>
    /// Limits options.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomLimitsOptions Limits { get; set; } = new();

    /// <summary>
    /// Transition options.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomTransitionOptions Transition { get; set; }
}
