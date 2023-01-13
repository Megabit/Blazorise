#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Line elements are used to represent the line in a line chart.
/// </summary>
public class ChartLineElements
{
    /// <summary>
    /// Bézier curve tension (0 for no Bézier curves).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Tension { get; set; }

    /// <summary>
    /// Line fill color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Line stroke width.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Line stroke color.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Line cap style. See <see href="https://developer.mozilla.org/en/docs/Web/API/CanvasRenderingContext2D/lineCap">MDN</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderCapStyle { get; set; }

    /// <summary>
    /// Line dash. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/setLineDash">MDN</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double?[] BorderDash { get; set; }

    /// <summary>
    /// Line dash offset. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineDashOffset">MDN</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderDashOffset { get; set; }

    /// <summary>
    /// Line join style. See <see href="https://developer.mozilla.org/en-US/docs/Web/API/CanvasRenderingContext2D/lineJoin">MDN</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderJoinStyle { get; set; }

    /// <summary>
    /// true to keep Bézier control inside the chart, false for no restriction.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? CapBezierPoints { get; set; }

    /// <summary>
    /// Interpolation mode to apply. <see href="https://www.chartjs.org/docs/latest/charts/line.html#cubicinterpolationmode">See more...</see>
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string CubicInterpolationMode { get; set; }

    /// <summary>
    /// How to fill the area under the line. See <see href="https://www.chartjs.org/docs/latest/charts/area.html#filling-modes">area charts</see>.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Fill { get; set; }

    /// <summary>
    /// true to show the line as a stepped line (tension will be ignored).
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Stepped { get; set; }
}