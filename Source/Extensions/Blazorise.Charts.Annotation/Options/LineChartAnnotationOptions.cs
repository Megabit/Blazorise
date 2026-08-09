#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Line annotation specific options.
/// </summary>
public class LineChartAnnotationOptions : ChartAnnotationOptions
{
    /// <summary>
    /// Thickness of the annotated line in pixels.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BorderWidth { get; set; }

    /// <summary>
    /// Control-point offset used when drawing a curved line.
    /// </summary>
    [JsonPropertyName( "controlPoint" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ControlPoint { get; set; }

    /// <summary>
    /// Whether the line bends through its configured control point.
    /// </summary>
    [JsonPropertyName( "curve" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Curve { get; set; }

    /// <summary>
    /// Ending coordinate for a line spanning one scale.
    /// </summary>
    [JsonPropertyName( "endValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? EndValue { get; set; }

    /// <summary>
    /// Text and styling displayed alongside the line.
    /// </summary>
    [JsonPropertyName( "label" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAnnotationLabelOptions Label { get; set; }

    /// <summary>
    /// Scale that interprets <see cref="Value"/> and <see cref="EndValue"/>.
    /// </summary>
    [JsonPropertyName( "scaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ScaleID { get; set; }

    /// <summary>
    /// Starting coordinate for a line bound to one scale.
    /// </summary>
    [JsonPropertyName( "value" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Value { get; set; }
}