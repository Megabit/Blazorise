#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts
{
    public class ChartOptions
    {
        public Scales Scales { get; set; }

        public Legend Legend { get; set; }

        public Tooltips Tooltips { get; set; }
    }

    #region Specifics

    public class LineChartOptions : ChartOptions
    {
        /// <summary>
        /// If false, the lines between points are not drawn.
        /// </summary>
        public bool ShowLines { get; set; } = true;

        /// <summary>
        /// If false, NaN data causes a break in the line.
        /// </summary>
        public bool SpanGaps { get; set; }
    }

    public class BarChartOptions : ChartOptions
    {
        /// <summary>
        /// Percent (0-1) of the available width each bar should be within the category width. 1.0 will take the whole category width and put the bars right next to each other.
        /// </summary>
        public float BarPercentage { get; set; } = 0.9f;

        /// <summary>
        /// Percent (0-1) of the available width each category should be within the sample width. 
        /// </summary>
        public float CategoryPercentage { get; set; } = 0.8f;

        /// <summary>
        /// Manually set width of each bar in pixels. If not set, the base sample widths are calculated automatically so that they take the full available widths without overlap. Then, the bars are sized using barPercentage and categoryPercentage.
        /// </summary>
        public int BarThickness { get; set; }

        /// <summary>
        /// Set this to ensure that bars are not sized thicker than this.
        /// </summary>
        public int MaxBarThickness { get; set; }
    }

    public class PieChartOptions : ChartOptions
    {
        /// <summary>
        /// The percentage of the chart that is cut out of the middle.
        /// </summary>
        public int CutoutPercentage { get; set; } = 0;

        /// <summary>
        /// Starting angle to draw arcs from.
        /// </summary>
        public double Rotation { get; set; } = -0.5 * Math.PI;

        /// <summary>
        /// Sweep to allow arcs to cover.
        /// </summary>
        public double Circumference { get; set; } = 2 * Math.PI;
    }

    public class DoughnutChartOptions : PieChartOptions
    {
        /// <summary>
        /// The percentage of the chart that is cut out of the middle.
        /// </summary>
        public new int CutoutPercentage { get; set; } = 50;
    }

    public class PolarAreaChartOptions : ChartOptions
    {
        /// <summary>
        /// Starting angle to draw arcs for the first item in a dataset.
        /// </summary>
        public double StartAngle { get; set; } = -0.5 * Math.PI;
    }

    public class RadarChartOptions : ChartOptions
    {
        // Unlike other charts, the radar chart has no chart specific options.
    }

    #endregion

    public class Scales
    {
        public List<Axe> XAxes { get; set; }

        public List<Axe> YAxes { get; set; }
    }

    public class Legend
    {
        /// <summary>
        /// Is the legend shown.
        /// </summary>
        public bool Display { get; set; } = true;

        /// <summary>
        /// Marks that this box should take the full width of the canvas (pushing down other boxes). This is unlikely to need to be changed in day-to-day use.
        /// </summary>
        public bool FullWidth { get; set; } = true;

        /// <summary>
        /// Legend will show datasets in reverse order.
        /// </summary>
        public bool Reverse { get; set; } = false;
    }

    public class Tooltips
    {
        /// <summary>
        /// Are on-canvas tooltips enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

    public class Axe
    {
        public bool Display { get; set; } = true;

        public AxeTicks Ticks { get; set; }
    }

    public class AxeTicks
    {
        public bool BeginAtZero { get; set; }
    }
}
