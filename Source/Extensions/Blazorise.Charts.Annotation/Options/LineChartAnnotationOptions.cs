#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Line annotation specific options.
/// </summary>
public class LineChartAnnotationOptions : ChartAnnotationOptions
{
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? BorderWidth { get; set; }

    [JsonPropertyName( "controlPoint" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ControlPoint { get; set; }

    [JsonPropertyName( "curve" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Curve { get; set; }

    [JsonPropertyName( "endValue" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? EndValue { get; set; }

    [JsonPropertyName( "label" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAnnotationLabelOptions Label { get; set; }

    [JsonPropertyName( "scaleID" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string ScaleID { get; set; }

    [JsonPropertyName( "value" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Value { get; set; }
}
