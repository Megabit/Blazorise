#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Box annotation specific options.
/// </summary>
public class BoxChartAnnotationOptions : ChartAnnotationOptions
{
    /// <summary>
    /// Shadow color behind the box fill.
    /// </summary>
    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    /// <summary>
    /// Canvas cap style applied to box border endpoints.
    /// </summary>
    [JsonPropertyName( "borderCapStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderCapStyle { get; set; }

    /// <summary>
    /// Canvas join style used where box border segments meet.
    /// </summary>
    [JsonPropertyName( "borderJoinStyle" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string BorderJoinStyle { get; set; }

    /// <summary>
    /// Radius used to round the box corners.
    /// </summary>
    [JsonPropertyName( "borderRadius" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderRadius { get; set; }

    /// <summary>
    /// Thickness of the box outline in pixels.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Caption rendered with the annotated box.
    /// </summary>
    [JsonPropertyName( "label" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAnnotationLabelOptions Label { get; set; }

    /// <summary>
    /// Clockwise rotation applied to the box geometry.
    /// </summary>
    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }
}