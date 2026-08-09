#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Stores values and styling for bubble chart data.
/// </summary>
public class BubbleChartDataset<T> : ChartDataset<T>
{
    /// <summary>
    /// Creates a bubble chart dataset.
    /// </summary>
    public BubbleChartDataset() : base(
        label: string.Empty,
        backgroundColor: "rgba(0, 0, 0, 0.1)",
        borderColor: "rgba(0, 0, 0, 0.1)",
        borderWidth: 3
    )
    {
        Type = "bubble";
    }

    /// <summary>
    /// Fill colors applied to hovered bubbles.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> HoverBackgroundColor { get; set; }

    /// <summary>
    /// Border colors applied to hovered bubbles.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> HoverBorderColor { get; set; }

    /// <summary>
    /// Hover Border Width used to size or locate content in the bubble chart dataset.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? HoverBorderWidth { get; set; }

    /// <summary>
    /// Additional radius applied to hovered bubbles.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? HoverRadius { get; set; }

    /// <summary>
    /// Extra radius used for pointer hit detection.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? HitRadius { get; set; }

    /// <summary>
    /// Style of the point.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Rotation angle for point-style graphics.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }

    /// <summary>
    /// Radius of the rendered point.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Radius { get; set; }
}

/// <summary>
/// Supports bubble chart point behavior in chart components.
/// </summary>
public struct BubbleChartPoint
{
    /// <summary>
    /// Creates a bubble chart point instance.
    /// </summary>
    public BubbleChartPoint( double? x, double? y, double? r )
    {
        X = x;
        Y = y;
        R = r;
    }

    /// <summary>
    /// Horizontal coordinate of the rendered element.
    /// </summary>
    [JsonPropertyName( "x" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? X { get; set; }

    /// <summary>
    /// Vertical coordinate of the rendered element.
    /// </summary>
    [JsonPropertyName( "y" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Y { get; set; }

    /// <summary>
    /// Radius of the bubble point.
    /// </summary>
    [JsonPropertyName( "r" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? R { get; set; }
}