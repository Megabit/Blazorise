#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Point elements are used to represent the points in a line, radar or bubble chart.
/// </summary>
public class ChartPointElements
{
    /// <summary>
    /// Point radius.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Radius { get; set; }

    /// <summary>
    /// Point style.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Point rotation (in degrees).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Point fill color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Point stroke width.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Point stroke color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Extra radius added to point radius for hit detection.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? HitRadius { get; set; }

    /// <summary>
    /// Point radius when hovered.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? HoverRadius { get; set; }

    /// <summary>
    /// Stroke width when hovered.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? HoverBorderWidth { get; set; }
}