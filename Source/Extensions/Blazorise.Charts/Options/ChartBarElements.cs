#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Bar elements are used to represent the bars in a bar chart.
/// </summary>
public class ChartBarElements
{
    /// <summary>
    /// Bar fill color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Bar stroke width.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Bar stroke color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Skipped (excluded) border: 'start', 'end', 'middle', 'bottom', 'left', 'top', 'right' or false.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderSkipped { get; set; }

    /// <summary>
    /// The bar border radius (in pixels).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderRadius { get; set; }

    /// <summary>
    /// The amount of pixels to inflate the bar rectangle(s) when drawing.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? InflateAmount { get; set; }

    /// <summary>
    /// Style of the point for legend.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string PointStyle { get; set; }
}