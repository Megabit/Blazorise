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
        Wheel = new ChartZoomWheel()
        {
            Enabled = true
        },
        Pinch = new ChartZoomPinch()
        {
            Enabled = true
        },
        Mode = "xy",
    };

    /// <summary>
    /// Pan options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomPanOptions Pan { get; set; } = new ChartZoomPanOptions { };

    /// <summary>
    /// Limits options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomLimitsOptions Limits { get; set; } = new ChartZoomLimitsOptions { };


    /// <summary>
    /// Transition options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomTransition Transition { get; set; }

}
