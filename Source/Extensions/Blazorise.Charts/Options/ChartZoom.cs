#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html
/// </summary>
public class ChartZoom
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

}


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

/// <summary>
/// The Chart Zoom plugin zoom wheel 
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#wheel-options</para>
/// </summary>
public class ChartZoomWheel
{
    /// <summary>
    /// Enable zooming via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Factor of zoom speed via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Speed { get; set; }

    /// <summary>
    /// Modifier key required for zooming via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ModifierKey { get; set; }
}

/// <summary>
/// The Chart Zoom plugin zoom drag 
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#drag-options</para>
/// </summary>
public class ChartZoomDrag
{
    /// <summary>
    /// Enable drag-to-zoom
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Fill color
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Stroke color
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Stroke width
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Minimal zoom distance required before actually applying zoom
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Threshold { get; set; }

    /// <summary>
    /// Modifier key required for zooming via mouse wheel
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ModifierKey { get; set; }
}

/// <summary>
/// The Chart Zoom plugin zoom pinch 
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#pinch-options</para>
/// </summary>
public class ChartZoomPinch
{
    /// <summary>
    /// Enable zooming via pinch
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

}

/// <summary>
/// The Chart Zoom plugin pan options
/// <para>https://www.chartjs.org/chartjs-plugin-zoom/latest/guide/options.html#pan-options</para>
/// </summary>
public class ChartZoomPanOptions
{
    /// <summary>
    /// Enable panning
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Enable panning
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Mode { get; set; }

    /// <summary>
    /// Modifier key required for panning with mouse
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ModifierKey { get; set; }

    /// <summary>
    /// Enable panning over a scale for that axis (regardless of mode)
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ScaleMode { get; set; }

    /// <summary>
    /// Minimal pan distance required before actually applying pan
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Threshold { get; set; }
}

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