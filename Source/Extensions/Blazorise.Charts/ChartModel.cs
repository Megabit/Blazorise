#region Using directives
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class ChartModel
    {
        [JsonPropertyName( "x" )]
        public double X { get; set; }

        [JsonPropertyName( "y" )]
        public double Y { get; set; }
    }

    /* ======== IMPORTANT ========
    * The reason why base ChartModel class is not used is becaoue the Blazor serializer does not support inheritance.
    * Until that is fixed we must write every model without inherit fields.
    * =========================== */

    [DataContract]
    public class LineChartModel : ChartModel
    {
        [JsonPropertyName( "label" )]
        public string Label { get; set; }

        [JsonPropertyName( "backgroundColor" )]
        public string BackgroundColor { get; set; }

        [JsonPropertyName( "borderColor" )]
        public string BorderColor { get; set; }

        [JsonPropertyName( "borderWidth" )]
        public double BorderWidth { get; set; }

        [JsonPropertyName( "controlPointNextX" )]
        public double ControlPointNextX { get; set; }

        [JsonPropertyName( "controlPointNextY" )]
        public double ControlPointNextY { get; set; }

        [JsonPropertyName( "controlPointPreviousX" )]
        public double ControlPointPreviousX { get; set; }

        [JsonPropertyName( "controlPointPreviousY" )]
        public double ControlPointPreviousY { get; set; }

        [JsonPropertyName( "hitRadius" )]
        public double HitRadius { get; set; }

        [JsonPropertyName( "pointStyle" )]
        public string PointStyle { get; set; }

        [JsonPropertyName( "radius" )]
        public double Radius { get; set; }

        [JsonPropertyName( "skip" )]
        public bool Skip { get; set; }

        [JsonPropertyName( "steppedLine" )]
        public bool SteppedLine { get; set; }

        [JsonPropertyName( "tension" )]
        public double Tension { get; set; }

        //[JsonPropertyName( "x" )]
        //public double X { get; set; }

        //[JsonPropertyName( "y" )]
        //public double Y { get; set; }
    }

    [DataContract]
    public class BarChartModel : ChartModel
    {
        [JsonPropertyName( "label" )]
        public string Label { get; set; }

        [JsonPropertyName( "datasetLabel" )]
        public string DatasetLabel { get; set; }

        [JsonPropertyName( "backgroundColor" )]
        public string BackgroundColor { get; set; }

        [JsonPropertyName( "borderColor" )]
        public string BorderColor { get; set; }

        [JsonPropertyName( "borderWidth" )]
        public double BorderWidth { get; set; }

        [JsonPropertyName( "borderSkipped" )]
        public string BorderSkipped { get; set; }

        [JsonPropertyName( "base" )]
        public double Base { get; set; }

        [JsonPropertyName( "horizontal" )]
        public bool Horizontal { get; set; }

        //[JsonPropertyName( "x" )]
        //public double X { get; set; }

        //[JsonPropertyName( "y" )]
        //public double Y { get; set; }

        [JsonPropertyName( "width" )]
        public double Width { get; set; }
    }

    [DataContract]
    public class DoughnutChartModel : ChartModel
    {
        [JsonPropertyName( "label" )]
        public string Label { get; set; }

        [JsonPropertyName( "backgroundColor" )]
        public string BackgroundColor { get; set; }

        [JsonPropertyName( "borderColor" )]
        public string BorderColor { get; set; }

        [JsonPropertyName( "borderWidth" )]
        public double BorderWidth { get; set; }

        [JsonPropertyName( "circumference" )]
        public double Circumference { get; set; }

        [JsonPropertyName( "startAngle" )]
        public double StartAngle { get; set; }

        [JsonPropertyName( "endAngle" )]
        public double EndAngle { get; set; }

        [JsonPropertyName( "outerRadius" )]
        public double OuterRadius { get; set; }

        [JsonPropertyName( "innerRadius" )]
        public double InnerRadius { get; set; }

        //[JsonPropertyName( "x" )]
        //public double X { get; set; }

        //[JsonPropertyName( "y" )]
        //public double Y { get; set; }
    }

    [DataContract]
    public class PieChartModel : ChartModel
    {
        [JsonPropertyName( "label" )]
        public string Label { get; set; }

        [JsonPropertyName( "backgroundColor" )]
        public string BackgroundColor { get; set; }

        [JsonPropertyName( "borderColor" )]
        public string BorderColor { get; set; }

        [JsonPropertyName( "borderWidth" )]
        public double BorderWidth { get; set; }

        [JsonPropertyName( "circumference" )]
        public double Circumference { get; set; }

        [JsonPropertyName( "startAngle" )]
        public double StartAngle { get; set; }

        [JsonPropertyName( "endAngle" )]
        public double EndAngle { get; set; }

        [JsonPropertyName( "outerRadius" )]
        public double OuterRadius { get; set; }

        [JsonPropertyName( "innerRadius" )]
        public double InnerRadius { get; set; }

        //[JsonPropertyName( "x" )]
        //public double X { get; set; }

        //[JsonPropertyName( "y" )]
        //public double Y { get; set; }
    }

    [DataContract]
    public class PolarChartModel : ChartModel
    {
        [JsonPropertyName( "label" )]
        public string Label { get; set; }

        [JsonPropertyName( "backgroundColor" )]
        public string BackgroundColor { get; set; }

        [JsonPropertyName( "borderColor" )]
        public string BorderColor { get; set; }

        [JsonPropertyName( "startAngle" )]
        public double StartAngle { get; set; }

        [JsonPropertyName( "endAngle" )]
        public double EndAngle { get; set; }

        [JsonPropertyName( "outerRadius" )]
        public double OuterRadius { get; set; }

        [JsonPropertyName( "innerRadius" )]
        public double InnerRadius { get; set; }

        //[JsonPropertyName( "x" )]
        //public double X { get; set; }

        //[JsonPropertyName( "y" )]
        //public double Y { get; set; }
    }

    [DataContract]
    public class RadarChartModel : ChartModel
    {
        [JsonPropertyName( "label" )]
        public string Label { get; set; }

        [JsonPropertyName( "backgroundColor" )]
        public string BackgroundColor { get; set; }

        [JsonPropertyName( "borderColor" )]
        public string BorderColor { get; set; }

        [JsonPropertyName( "borderWidth" )]
        public double BorderWidth { get; set; }

        [JsonPropertyName( "controlPointNextX" )]
        public double ControlPointNextX { get; set; }

        [JsonPropertyName( "controlPointNextY" )]
        public double ControlPointNextY { get; set; }

        [JsonPropertyName( "controlPointPreviousX" )]
        public double ControlPointPreviousX { get; set; }

        [JsonPropertyName( "controlPointPreviousY" )]
        public double ControlPointPreviousY { get; set; }

        [JsonPropertyName( "hitRadius" )]
        public double HitRadius { get; set; }

        [JsonPropertyName( "pointStyle" )]
        public string PointStyle { get; set; }

        [JsonPropertyName( "radius" )]
        public double Radius { get; set; }

        [JsonPropertyName( "skip" )]
        public bool Skip { get; set; }

        [JsonPropertyName( "tension" )]
        public double Tension { get; set; }

        //[JsonPropertyName( "x" )]
        //public double X { get; set; }

        //[JsonPropertyName( "y" )]
        //public double Y { get; set; }
    }
}
