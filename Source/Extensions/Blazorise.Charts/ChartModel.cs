#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Describes a rendered chart element.
/// </summary>
public class ChartModel
{
    /// <summary>
    /// Horizontal coordinate of the rendered element.
    /// </summary>
    [JsonPropertyName( "x" )]
    public double X { get; set; }

    /// <summary>
    /// Vertical coordinate of the rendered element.
    /// </summary>
    [JsonPropertyName( "y" )]
    public double Y { get; set; }
}

/* ======== IMPORTANT ========
* The reason why base ChartModel class is not used is because the Blazor serializer does not support inheritance.
* Until that is fixed we must write every model without inherit fields.
* =========================== */

/// <summary>
/// Describes a rendered line chart element.
/// </summary>
public class LineChartModel : ChartModel
{
    /// <summary>
    /// Label displayed for the rendered value.
    /// </summary>
    [JsonPropertyName( "label" )]
    public string Label { get; set; }

    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Horizontal coordinate of the next Bézier control point.
    /// </summary>
    [JsonPropertyName( "controlPointNextX" )]
    public double ControlPointNextX { get; set; }

    /// <summary>
    /// Vertical coordinate of the next Bézier control point.
    /// </summary>
    [JsonPropertyName( "controlPointNextY" )]
    public double ControlPointNextY { get; set; }

    /// <summary>
    /// Horizontal coordinate of the previous Bézier control point.
    /// </summary>
    [JsonPropertyName( "controlPointPreviousX" )]
    public double ControlPointPreviousX { get; set; }

    /// <summary>
    /// Vertical coordinate of the previous Bézier control point.
    /// </summary>
    [JsonPropertyName( "controlPointPreviousY" )]
    public double ControlPointPreviousY { get; set; }

    /// <summary>
    /// Extra radius used for pointer hit detection.
    /// </summary>
    [JsonPropertyName( "hitRadius" )]
    public double HitRadius { get; set; }

    /// <summary>
    /// Visual shape used for the data point.
    /// </summary>
    [JsonPropertyName( "pointStyle" )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Radius of the rendered point.
    /// </summary>
    [JsonPropertyName( "radius" )]
    public double Radius { get; set; }

    /// <summary>
    /// Controls skip behavior for the line chart model.
    /// </summary>
    [JsonPropertyName( "skip" )]
    public bool Skip { get; set; }

    /// <summary>
    /// Controls stepped line behavior for the line chart model.
    /// </summary>
    [JsonPropertyName( "steppedLine" )]
    public bool SteppedLine { get; set; }

    /// <summary>
    /// Curve tension applied between data points.
    /// </summary>
    [JsonPropertyName( "tension" )]
    public double Tension { get; set; }

    //[JsonPropertyName( "x" )]
    //public double X { get; set; }

    //[JsonPropertyName( "y" )]
    //public double Y { get; set; }
}

/// <summary>
/// Describes a rendered bar chart element.
/// </summary>
public class BarChartModel : ChartModel
{
    /// <summary>
    /// Label displayed for the rendered value.
    /// </summary>
    [JsonPropertyName( "label" )]
    public string Label { get; set; }

    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Edge omitted when drawing the bar border.
    /// </summary>
    [JsonPropertyName( "borderSkipped" )]
    public string BorderSkipped { get; set; }

    /// <summary>
    /// Pixel coordinate of the bar baseline.
    /// </summary>
    [JsonPropertyName( "base" )]
    public double Base { get; set; }

    /// <summary>
    /// Controls horizontal behavior for the bar chart model.
    /// </summary>
    [JsonPropertyName( "horizontal" )]
    public bool Horizontal { get; set; }

    //[JsonPropertyName( "x" )]
    //public double X { get; set; }

    //[JsonPropertyName( "y" )]
    //public double Y { get; set; }

    /// <summary>
    /// Rendered width of the element.
    /// </summary>
    [JsonPropertyName( "width" )]
    public double Width { get; set; }
}

/// <summary>
/// Describes a rendered doughnut chart element.
/// </summary>
public class DoughnutChartModel : ChartModel
{
    /// <summary>
    /// Label displayed for the rendered value.
    /// </summary>
    [JsonPropertyName( "label" )]
    public string Label { get; set; }

    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Angular span of the rendered arc.
    /// </summary>
    [JsonPropertyName( "circumference" )]
    public double Circumference { get; set; }

    /// <summary>
    /// Angle where the rendered arc begins.
    /// </summary>
    [JsonPropertyName( "startAngle" )]
    public double StartAngle { get; set; }

    /// <summary>
    /// Angle where the rendered arc ends.
    /// </summary>
    [JsonPropertyName( "endAngle" )]
    public double EndAngle { get; set; }

    /// <summary>
    /// Outer radius of the rendered arc.
    /// </summary>
    [JsonPropertyName( "outerRadius" )]
    public double OuterRadius { get; set; }

    /// <summary>
    /// Inner radius of the rendered arc.
    /// </summary>
    [JsonPropertyName( "innerRadius" )]
    public double InnerRadius { get; set; }

    //[JsonPropertyName( "x" )]
    //public double X { get; set; }

    //[JsonPropertyName( "y" )]
    //public double Y { get; set; }
}

/// <summary>
/// Describes a rendered pie chart element.
/// </summary>
public class PieChartModel : ChartModel
{
    /// <summary>
    /// Label displayed for the rendered value.
    /// </summary>
    [JsonPropertyName( "label" )]
    public string Label { get; set; }

    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Angular span of the rendered arc.
    /// </summary>
    [JsonPropertyName( "circumference" )]
    public double Circumference { get; set; }

    /// <summary>
    /// Angle where the rendered arc begins.
    /// </summary>
    [JsonPropertyName( "startAngle" )]
    public double StartAngle { get; set; }

    /// <summary>
    /// Angle where the rendered arc ends.
    /// </summary>
    [JsonPropertyName( "endAngle" )]
    public double EndAngle { get; set; }

    /// <summary>
    /// Outer radius of the rendered arc.
    /// </summary>
    [JsonPropertyName( "outerRadius" )]
    public double OuterRadius { get; set; }

    /// <summary>
    /// Inner radius of the rendered arc.
    /// </summary>
    [JsonPropertyName( "innerRadius" )]
    public double InnerRadius { get; set; }

    //[JsonPropertyName( "x" )]
    //public double X { get; set; }

    //[JsonPropertyName( "y" )]
    //public double Y { get; set; }
}

/// <summary>
/// Describes a rendered polar chart element.
/// </summary>
public class PolarChartModel : ChartModel
{
    /// <summary>
    /// Label displayed for the rendered value.
    /// </summary>
    [JsonPropertyName( "label" )]
    public string Label { get; set; }

    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Angle where the rendered arc begins.
    /// </summary>
    [JsonPropertyName( "startAngle" )]
    public double StartAngle { get; set; }

    /// <summary>
    /// Angle where the rendered arc ends.
    /// </summary>
    [JsonPropertyName( "endAngle" )]
    public double EndAngle { get; set; }

    /// <summary>
    /// Outer radius of the rendered arc.
    /// </summary>
    [JsonPropertyName( "outerRadius" )]
    public double OuterRadius { get; set; }

    /// <summary>
    /// Inner radius of the rendered arc.
    /// </summary>
    [JsonPropertyName( "innerRadius" )]
    public double InnerRadius { get; set; }

    //[JsonPropertyName( "x" )]
    //public double X { get; set; }

    //[JsonPropertyName( "y" )]
    //public double Y { get; set; }
}

/// <summary>
/// Describes a rendered radar chart element.
/// </summary>
public class RadarChartModel : ChartModel
{
    /// <summary>
    /// Label displayed for the rendered value.
    /// </summary>
    [JsonPropertyName( "label" )]
    public string Label { get; set; }

    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Horizontal coordinate of the next radar-line control point.
    /// </summary>
    [JsonPropertyName( "controlPointNextX" )]
    public double ControlPointNextX { get; set; }

    /// <summary>
    /// Vertical coordinate of the next radar-line control point.
    /// </summary>
    [JsonPropertyName( "controlPointNextY" )]
    public double ControlPointNextY { get; set; }

    /// <summary>
    /// Horizontal coordinate of the previous radar-line control point.
    /// </summary>
    [JsonPropertyName( "controlPointPreviousX" )]
    public double ControlPointPreviousX { get; set; }

    /// <summary>
    /// Vertical coordinate of the previous radar-line control point.
    /// </summary>
    [JsonPropertyName( "controlPointPreviousY" )]
    public double ControlPointPreviousY { get; set; }

    /// <summary>
    /// Extra radius used for pointer hit detection.
    /// </summary>
    [JsonPropertyName( "hitRadius" )]
    public double HitRadius { get; set; }

    /// <summary>
    /// Visual shape used for the data point.
    /// </summary>
    [JsonPropertyName( "pointStyle" )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Radius of the rendered point.
    /// </summary>
    [JsonPropertyName( "radius" )]
    public double Radius { get; set; }

    /// <summary>
    /// Controls skip behavior for the radar chart model.
    /// </summary>
    [JsonPropertyName( "skip" )]
    public bool Skip { get; set; }

    /// <summary>
    /// Curve tension applied between data points.
    /// </summary>
    [JsonPropertyName( "tension" )]
    public double Tension { get; set; }

    //[JsonPropertyName( "x" )]
    //public double X { get; set; }

    //[JsonPropertyName( "y" )]
    //public double Y { get; set; }
}

/// <summary>
/// Describes a rendered scatter chart element.
/// </summary>
public class ScatterChartModel : ChartModel
{
    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Extra radius used for pointer hit detection.
    /// </summary>
    [JsonPropertyName( "hitRadius" )]
    public double HitRadius { get; set; }

    /// <summary>
    /// Hover Border Width used to size or locate content in the scatter chart model.
    /// </summary>
    [JsonPropertyName( "hoverBorderWidth" )]
    public double HoverBorderWidth { get; set; }

    /// <summary>
    /// Point radius applied while the scatter value is hovered.
    /// </summary>
    [JsonPropertyName( "hoverRadius" )]
    public double HoverRadius { get; set; }

    /// <summary>
    /// Visual shape used for the data point.
    /// </summary>
    [JsonPropertyName( "pointStyle" )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Radius of the rendered point.
    /// </summary>
    [JsonPropertyName( "radius" )]
    public double Radius { get; set; }

    /// <summary>
    /// Controls skip behavior for the scatter chart model.
    /// </summary>
    [JsonPropertyName( "skip" )]
    public bool Skip { get; set; }

    /// <summary>
    /// Controls stop behavior for the scatter chart model.
    /// </summary>
    [JsonPropertyName( "stop" )]
    public bool Stop { get; set; }
}

/// <summary>
/// Describes a rendered bubble chart element.
/// </summary>
public class BubbleChartModel : ChartModel
{
    /// <summary>
    /// Label of the owning dataset.
    /// </summary>
    [JsonPropertyName( "datasetLabel" )]
    public string DatasetLabel { get; set; }

    /// <summary>
    /// Fill color of the rendered element.
    /// </summary>
    [JsonPropertyName( "backgroundColor" )]
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Outline color of the rendered element.
    /// </summary>
    [JsonPropertyName( "borderColor" )]
    public string BorderColor { get; set; }

    /// <summary>
    /// Width of the rendered outline.
    /// </summary>
    [JsonPropertyName( "borderWidth" )]
    public double BorderWidth { get; set; }

    /// <summary>
    /// Extra radius used for pointer hit detection.
    /// </summary>
    [JsonPropertyName( "hitRadius" )]
    public double HitRadius { get; set; }

    /// <summary>
    /// Hover Border Width used to size or locate content in the bubble chart model.
    /// </summary>
    [JsonPropertyName( "hoverBorderWidth" )]
    public double HoverBorderWidth { get; set; }

    /// <summary>
    /// Bubble radius applied while the value is hovered.
    /// </summary>
    [JsonPropertyName( "hoverRadius" )]
    public double HoverRadius { get; set; }

    /// <summary>
    /// Visual shape used for the data point.
    /// </summary>
    [JsonPropertyName( "pointStyle" )]
    public string PointStyle { get; set; }

    /// <summary>
    /// Radius of the rendered point.
    /// </summary>
    [JsonPropertyName( "radius" )]
    public double Radius { get; set; }

    /// <summary>
    /// Controls skip behavior for the bubble chart model.
    /// </summary>
    [JsonPropertyName( "skip" )]
    public bool Skip { get; set; }

    /// <summary>
    /// Controls stop behavior for the bubble chart model.
    /// </summary>
    [JsonPropertyName( "stop" )]
    public bool Stop { get; set; }
}