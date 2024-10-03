#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Defines the point style.
/// </summary>
public class ChartPointStyle
{
    /// <summary>
    /// Point radius.
    /// </summary>
    [JsonPropertyName( "radius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Radius { get; set; }

    /// <summary>
    /// Point style.
    /// </summary>
    [JsonPropertyName( "pointStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Point rotation (in degrees).
    /// </summary>
    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Point fill color.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Point stroke width.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Point stroke color.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Extra radius added to point radius for hit detection.
    /// </summary>
    [JsonPropertyName( "hitRadius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? HitRadius { get; set; }

    /// <summary>
    /// Point radius when hovered.
    /// </summary>
    [JsonPropertyName( "hoverRadius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? HoverRadius { get; set; }

    /// <summary>
    /// Stroke width when hovered.
    /// </summary>
    [JsonPropertyName( "hoverBorderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? HoverBorderWidth { get; set; }
}