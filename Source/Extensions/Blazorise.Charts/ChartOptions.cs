#region Using directives
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class ChartOptions
    {
        [DataMember( EmitDefaultValue = false )]
        public Scales Scales { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public Legend Legend { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public Tooltips Tooltips { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public Animation Animation { get; set; }

        /// <summary>
        /// Resizes the chart canvas when its container does.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public bool? Responsive { get; set; } = true;

        /// <summary>
        /// Maintain the original canvas aspect ratio (width / height) when resizing.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public bool? MaintainAspectRatio { get; set; } = true;

        /// <summary>
        /// Duration in milliseconds it takes to animate to new size after a resize event.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public double? ResponsiveAnimationDuration { get; set; } = 0;

        /// <summary>
        /// Canvas aspect ratio (i.e. width / height, a value of 1 representing a square canvas).
        /// Note that this option is ignored if the height is explicitly defined either as attribute or via the style.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public double? AspectRatio { get; set; } = 2;
    }

    #region Specifics

    [DataContract]
    public class LineChartOptions : ChartOptions
    {
        /// <summary>
        /// If false, the lines between points are not drawn.
        /// </summary>
        [DataMember]
        public bool ShowLines { get; set; } = true;

        /// <summary>
        /// If false, NaN data causes a break in the line.
        /// </summary>
        [DataMember]
        public bool SpanGaps { get; set; }
    }

    [DataContract]
    public class BarChartOptions : ChartOptions
    {
        /// <summary>
        /// Percent (0-1) of the available width each bar should be within the category width. 1.0 will take the whole category width and put the bars right next to each other.
        /// </summary>
        [DataMember]
        public float BarPercentage { get; set; } = 0.9f;

        /// <summary>
        /// Percent (0-1) of the available width each category should be within the sample width. 
        /// </summary>
        [DataMember]
        public float CategoryPercentage { get; set; } = 0.8f;

        /// <summary>
        /// Manually set width of each bar in pixels. If not set, the base sample widths are calculated automatically so that they take the full available widths without overlap. Then, the bars are sized using barPercentage and categoryPercentage.
        /// </summary>
        [DataMember]
        public int BarThickness { get; set; }

        /// <summary>
        /// Set this to ensure that bars are not sized thicker than this.
        /// </summary>
        [DataMember]
        public int MaxBarThickness { get; set; }
    }

    [DataContract]
    public class PieChartOptions : ChartOptions
    {
        /// <summary>
        /// The percentage of the chart that is cut out of the middle.
        /// </summary>
        [DataMember]
        public int CutoutPercentage { get; set; } = 0;

        /// <summary>
        /// Starting angle to draw arcs from.
        /// </summary>
        [DataMember]
        public double Rotation { get; set; } = -0.5 * Math.PI;

        /// <summary>
        /// Sweep to allow arcs to cover.
        /// </summary>
        [DataMember]
        public double Circumference { get; set; } = 2 * Math.PI;
    }

    [DataContract]
    public class DoughnutChartOptions : PieChartOptions
    {
        /// <summary>
        /// The percentage of the chart that is cut out of the middle.
        /// </summary>
        [DataMember]
        public new int CutoutPercentage { get; set; } = 50;
    }

    [DataContract]
    public class PolarAreaChartOptions : ChartOptions
    {
        /// <summary>
        /// Starting angle to draw arcs for the first item in a dataset.
        /// </summary>
        [DataMember]
        public double StartAngle { get; set; } = -0.5 * Math.PI;
    }

    [DataContract]
    public class RadarChartOptions : ChartOptions
    {
        // Unlike other charts, the radar chart has no chart specific options.
    }

    #endregion

    [DataContract]
    public class Scales
    {
        [DataMember( EmitDefaultValue = false )]
        public List<Axis> XAxes { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public List<Axis> YAxes { get; set; }
    }

    [DataContract]
    public class Legend
    {
        /// <summary>
        /// Is the legend shown.
        /// </summary>
        [DataMember]
        public bool Display { get; set; } = true;

        /// <summary>
        /// Marks that this box should take the full width of the canvas (pushing down other boxes). This is unlikely to need to be changed in day-to-day use.
        /// </summary>
        [DataMember]
        public bool FullWidth { get; set; } = true;

        /// <summary>
        /// Legend will show datasets in reverse order.
        /// </summary>
        [DataMember]
        public bool Reverse { get; set; } = false;
    }

    [DataContract]
    public class Tooltips
    {
        /// <summary>
        /// Are on-canvas tooltips enabled.
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; } = true;
    }

    [DataContract]
    public class Axis
    {
        [DataMember( EmitDefaultValue = false )]
        public string Type { get; set; }

        [DataMember]
        public bool Display { get; set; } = true;

        [DataMember( EmitDefaultValue = false )]
        public AxisTicks Ticks { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public AxisGridLines GridLines { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public AxisScaleLabel ScaleLabel { get; set; }

        [DataMember( EmitDefaultValue = false )]
        public bool Stacked { get; set; }
    }

    /// <summary>
    /// The tick configuration is nested under the scale configuration in the ticks key. It defines options for the tick marks that are generated by the axis.
    /// </summary>
    [DataContract]
    public class AxisTicks
    {
        /// <summary>
        /// If true, show tick marks.
        /// </summary>
        [DataMember]
        public bool Display { get; set; } = true;

        /// <summary>
        /// Font color for tick labels.
        /// </summary>
        [DataMember]
        public string FontColor { get; set; } = "#666";

        /// <summary>
        /// Font family for the tick labels, follows CSS font-family options.
        /// </summary>
        [DataMember]
        public string FontFamily { get; set; } = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

        /// <summary>
        /// Font size for the tick labels.
        /// </summary>
        [DataMember]
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// Font style for the tick labels, follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
        /// </summary>
        [DataMember]
        public string FontStyle { get; set; } = "normal";

        /// <summary>
        /// Height of an individual line of text.
        /// </summary>
        [DataMember]
        public decimal LineHeight { get; set; } = 1.2m;

        /// <summary>
        /// Reverses order of tick labels.
        /// </summary>
        [DataMember]
        public bool Reverse { get; set; }

        /// <summary>
        /// Minor ticks configuration. Omitted options are inherited from options above.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public AxisMinorTick Minor { get; set; }

        /// <summary>
        /// Major ticks configuration. Omitted options are inherited from options above.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public AxisMajorTick Major { get; set; }

        /// <summary>
        /// Sets the offset of the tick labels from the axis.
        /// </summary>
        [DataMember]
        public int Padding { get; set; }

        /// <summary>
        /// if true, scale will include 0 if it is not already included.
        /// </summary>
        [DataMember]
        public bool BeginAtZero { get; set; }
    }

    /// <summary>
    /// The minorTick configuration is nested under the ticks configuration in the minor key. It defines options for the minor tick marks that are generated by the axis. Omitted options are inherited from ticks configuration.
    /// </summary>
    [DataContract]
    public class AxisMinorTick
    {
        /// <summary>
        /// Font color for tick labels.
        /// </summary>
        [DataMember]
        public string FontColor { get; set; } = "#666";

        /// <summary>
        /// Font family for the tick labels, follows CSS font-family options.
        /// </summary>
        [DataMember]
        public string FontFamily { get; set; } = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

        /// <summary>
        /// Font size for the tick labels.
        /// </summary>
        [DataMember]
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// Font style for the tick labels, follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
        /// </summary>
        [DataMember]
        public string FontStyle { get; set; } = "normal";

        /// <summary>
        /// Height of an individual line of text.
        /// </summary>
        [DataMember]
        public decimal LineHeight { get; set; } = 1.2m;
    }

    /// <summary>
    /// The majorTick configuration is nested under the ticks configuration in the major key. It defines options for the major tick marks that are generated by the axis. Omitted options are inherited from ticks configuration. These options are disabled by default.
    /// </summary>
    [DataContract]
    public class AxisMajorTick
    {
        /// <summary>
        /// If true, major tick options are used to show major ticks.
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }

        /// <summary>
        /// Font color for tick labels.
        /// </summary>
        [DataMember]
        public string FontColor { get; set; } = "#666";

        /// <summary>
        /// Font family for the tick labels, follows CSS font-family options.
        /// </summary>
        [DataMember]
        public string FontFamily { get; set; } = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

        /// <summary>
        /// Font size for the tick labels.
        /// </summary>
        [DataMember]
        public int FontSize { get; set; } = 12;

        /// <summary>
        /// Font style for the tick labels, follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
        /// </summary>
        [DataMember]
        public string FontStyle { get; set; } = "normal";

        /// <summary>
        /// Height of an individual line of text.
        /// </summary>
        [DataMember]
        public decimal LineHeight { get; set; } = 1.2m;
    }

    /// <summary>
    /// The grid line configuration is nested under the scale configuration in the gridLines key. It defines options for the grid lines that run perpendicular to the axis.
    /// </summary>
    [DataContract]
    public class AxisGridLines
    {
        /// <summary>
        /// If false, do not display grid lines for this axis.
        /// </summary>
        [DataMember]
        public bool Display { get; set; } = true;

        /// <summary>
        /// If true, gridlines are circular (on radar chart only).
        /// </summary>
        [DataMember]
        public bool Circular { get; set; }

        /// <summary>
        /// The color of the grid lines.
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>
        /// Length and spacing of dashes on grid lines
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<int> BorderDash { get; set; }

        /// <summary>
        /// Offset for line dashes.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public decimal BorderDashOffset { get; set; } = 0m;

        /// <summary>
        /// Stroke width of grid lines.
        /// </summary>
        [DataMember]
        public int LineWidth { get; set; } = 1;

        /// <summary>
        /// If true, draw border at the edge between the axis and the chart area.
        /// </summary>
        [DataMember]
        public bool DrawBorder { get; set; } = true;

        /// <summary>
        /// If true, draw lines on the chart area inside the axis lines. This is useful when there are multiple axes and you need to control which grid lines are drawn.
        /// </summary>
        [DataMember]
        public bool DrawOnChartArea { get; set; } = true;

        /// <summary>
        /// If true, draw lines beside the ticks in the axis area beside the chart.
        /// </summary>
        [DataMember]
        public bool DrawTicks { get; set; } = true;

        /// <summary>
        /// Length in pixels that the grid lines will draw into the axis area.
        /// </summary>
        [DataMember]
        public int TickMarkLength { get; set; } = 10;

        /// <summary>
        /// Stroke width of the grid line for the first index (index 0).
        /// </summary>
        [DataMember]
        public int ZeroLineWidth { get; set; } = 1;

        /// <summary>
        /// Stroke color of the grid line for the first index (index 0).
        /// </summary>
        [DataMember]
        public string ZeroLineColor { get; set; }

        /// <summary>
        /// Length and spacing of dashes of the grid line for the first index (index 0).
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<int> ZeroLineBorderDash { get; set; }

        /// <summary>
        /// Offset for line dashes of the grid line for the first index (index 0).
        /// </summary>
        [DataMember]
        public decimal ZeroLineBorderDashOffset { get; set; } = 0m;

        /// <summary>
        /// If true, grid lines will be shifted to be between labels. This is set to true for a category scale in a bar chart by default.
        /// </summary>
        [DataMember]
        public bool OffsetGridLines { get; set; }
    }

    /// <summary>
    /// Defines options for the scale title.
    /// </summary>
    public class AxisScaleLabel
    {
        /// <summary>
        /// If true, display the axis title.
        /// </summary>
        [DataMember]
        public bool Display { get; set; }

        /// <summary>
        /// The text for the title. (i.e. "# of People" or "Response Choices").
        /// </summary>
        [DataMember]
        public string LabelString { get; set; } = "";

        /// <summary>
        /// Height of an individual line of text (<see cref="https://developer.mozilla.org/en-US/docs/Web/CSS/line-height"/>).
        /// </summary>
        [DataMember]
        public double LineHeight { get; set; } = 1.2d;

        /// <summary>
        /// Font color for scale title.
        /// </summary>
        [DataMember]
        public string FontColor { get; set; } = "#666";

        /// <summary>
        /// Font family for the scale title, follows CSS font-family options.
        /// </summary>
        [DataMember]
        public string FontFamily { get; set; } = "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif";

        /// <summary>
        /// Font size for scale title.
        /// </summary>
        [DataMember]
        public double FontSize { get; set; } = 12;

        /// <summary>
        /// Font style for the scale title, follows CSS font-style options (i.e. normal, italic, oblique, initial, inherit).
        /// </summary>
        [DataMember]
        public string FontStyle { get; set; } = "normal";

        /// <summary>
        /// Padding to apply around scale labels. Only top and bottom are implemented.
        /// </summary>
        [DataMember]
        public object Padding { get; set; } = 4;
    }

    /// <summary>
    /// Defines the chart animation options.
    /// </summary>
    [DataContract]
    public class Animation
    {
        /// <summary>
        /// The number of milliseconds an animation takes.
        /// </summary>
        [DataMember]
        public int Duration { get; set; } = 1000;

        /// <summary>
        /// Easing function to use. <seealso cref="https://www.chartjs.org/docs/latest/configuration/animations.html#easing"/>
        /// </summary>
        [DataMember]
        public string Easing { get; set; } = "easeOutQuart";
    }
}
