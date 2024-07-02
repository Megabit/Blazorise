#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// The Chart Zoom plugin zoom options
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#zoom-options</para>
/// </summary>
public class ChartZoomOptions
{
    /// <summary>
    /// The Wheel options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomWheel Wheel { get; set; }

    /// <summary>
    /// The Drag options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomDrag Drag { get; set; }

    /// <summary>
    /// The Pinch options
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartZoomPinch Pinch { get; set; }

    /// <summary>
    /// Allowed zoom directions
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Mode { get; set; }

    /// <summary>
    /// Which of the enabled zooming directions should only be available when the mouse cursor is over a scale for that axis
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ScaleMode { get; set; }

}
