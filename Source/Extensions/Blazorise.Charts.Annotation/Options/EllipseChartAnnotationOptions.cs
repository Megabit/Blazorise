#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Ellipse annotation specific options.
/// </summary>
public class EllipseChartAnnotationOptions : ChartAnnotationOptions
{
    /// <summary>
    /// Shadow color visible behind the ellipse fill.
    /// </summary>
    [JsonPropertyName( "backgroundShadowColor" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( ChartColorConverter ) )]
    public ChartColor? BackgroundShadowColor { get; set; }

    /// <summary>
    /// Width of the ellipse outline in pixels.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? BorderWidth { get; set; }

    /// <summary>
    /// Label configuration attached to the ellipse.
    /// </summary>
    [JsonPropertyName( "label" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Label { get; set; }

    /// <summary>
    /// Angle used to rotate the ellipse around its center.
    /// </summary>
    [JsonPropertyName( "rotation" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public double? Rotation { get; set; }
}