#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Common options to all axes.
/// </summary>
public class ChartAxis
{
    /// <summary>
    /// Type of scale being employed. Custom scales can be created and registered with a string key. This allows changing the type of an axis for a chart.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Type { get; set; }

    /// <summary>
    /// Align pixel values to device pixels.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? AlignToPixels { get; set; }

    /// <summary>
    /// Controls the axis global visibility (visible when <c>true</c>, hidden when <c>false</c>). When display: <c>"auto"</c>, the axis is visible only if at least one associated dataset is visible.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Display { get; set; }

    /// <summary>
    /// Grid line configuration. <see href="https://www.chartjs.org/docs/latest/axes/styling.html#grid-line-configuration">more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxisGridLine Grid { get; set; }

    /// <summary>
    /// Border configuration. <see href="https://www.chartjs.org/docs/latest/axes/styling.html#border-configuration">more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxisBorder Border { get; set; }

    /// <summary>
    /// User defined minimum number for the scale, overrides minimum value from data. <see href="https://www.chartjs.org/docs/latest/axes/#axis-range-settings">more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Min { get; set; }

    /// <summary>
    /// User defined maximum number for the scale, overrides maximum value from data. <see href="https://www.chartjs.org/docs/latest/axes/#axis-range-settings">more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Max { get; set; }

    /// <summary>
    /// Reverse the scale.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Reverse { get; set; }

    /// <summary>
    /// Should the data be stacked.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Stacked { get; set; }

    /// <summary>
    /// Adjustment used when calculating the maximum data value. <see href="https://www.chartjs.org/docs/latest/axes/#axis-range-settings">more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? SuggestedMax { get; set; }

    /// <summary>
    /// Adjustment used when calculating the minimum data value. <see href="https://www.chartjs.org/docs/latest/axes/#axis-range-settings">more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? SuggestedMin { get; set; }

    /// <summary>
    /// Tick configuration.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxisTicks Ticks { get; set; }

    /// <summary>
    /// Time configuration.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAxisTime Time { get; set; }

    /// <summary>
    /// The weight used to sort the axis. Higher weights are further away from the chart area.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Weight { get; set; }

    /// <summary>
    /// Defines options for the scale title. Note that this only applies to cartesian axes.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartScaleTitle Title { get; set; }

    #region Linear Axis specific options

    /// <summary>
    /// If true, scale will include 0 if it is not already included.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? BeginAtZero { get; set; }

    /// <summary>
    /// Percentage (string ending with %) or amount (number) for added room in the scale range above and below data.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Grace { get; set; }

    #endregion
}