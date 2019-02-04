#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts
{
    /* ======== IMPORTANT ========
     * Blazor serializer does not support DataMember attribute and because of that there is no way to omit null fields when serializing objects to json.
     * Hopefully this will change in the future.
     * =========================== */

    /// <summary>
    /// Base data object for all charts.
    /// </summary>
    /// <typeparam name="TItem">Type of value in the dataset.</typeparam>
    public class ChartData<TItem>
    {
        /// <summary>
        /// List of labels for the chart coordinates.
        /// </summary>
        public List<string> Labels { get; set; }

        /// <summary>
        /// List of datasets to be displayed in the chart.
        /// </summary>
        public List<ChartDataset<TItem>> Datasets { get; set; }
    }

    /// <summary>
    /// Base class for the chart dataset.
    /// </summary>
    public class ChartDataset<T>
    {
        /// <summary>
        /// Defines the dataset display name.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// List of data items.
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        ///List of background colors for each of the data items.
        /// </summary>
        public List<string> BackgroundColor { get; set; }

        /// <summary>
        /// List of border colors for each of the data items.
        /// </summary>
        public List<string> BorderColor { get; set; }

        /// <summary>
        /// Defines the border width.
        /// </summary>
        public int BorderWidth { get; set; } = 1;
    }

    public class LineChartDataset<T> : ChartDataset<T>
    {
        /// <summary>
        /// Length and spacing of dashes.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<int> BorderDash { get; set; }

        /// <summary>
        /// Offset for line dashes.
        /// </summary>
        public int BorderDashOffset { get; set; }

        /// <summary>
        /// Fill the area under the line.
        /// </summary>
        public bool Fill { get; set; } = true;

        /// <summary>
        /// Bezier curve tension of the line. Set to 0 to draw straightlines. This option is ignored if monotone cubic interpolation is used.
        /// </summary>
        public int LineTension { get; set; }

        /// <summary>
        /// The fill color for points.
        /// </summary>
        public List<string> PointBackgroundColor { get; set; }

        /// <summary>
        /// The border color for points.
        /// </summary>
        public List<string> PointBorderColor { get; set; }

        /// <summary>
        /// The width of the point border in pixels.
        /// </summary>
        public int PointBorderWidth { get; set; }

        /// <summary>
        /// The radius of the point shape. If set to 0, the point is not rendered.
        /// </summary>
        public float PointRadius { get; set; }

        /// <summary>
        /// If false, the line is not drawn for this dataset.
        /// </summary>
        public bool ShowLine { get; set; } = true;

        /// <summary>
        /// If true, lines will be drawn between points with no or null data. If false, points with NaN data will create a break in the line.
        /// </summary>
        public bool SpanGaps { get; set; }

        /// <summary>
        /// If the line is shown as a stepped line.
        /// </summary>
        public bool SteppedLine { get; set; }
    }

    public class BarChartDataset<T> : ChartDataset<T>
    {
        /// <summary>
        /// The fill colour of the bars when hovered.
        /// </summary>
        public List<string> HoverBackgroundColor { get; set; }

        /// <summary>
        /// The stroke colour of the bars when hovered.
        /// </summary>
        public List<string> HoverBorderColor { get; set; }

        /// <summary>
        /// The stroke width of the bars when hovered.
        /// </summary>
        public int HoverBorderWidth { get; set; }
    }

    public class PieChartDataset<T> : ChartDataset<T>
    {
        /// <summary>
        /// The fill colour of the arcs when hovered.
        /// </summary>
        public List<string> HoverBackgroundColor { get; set; }

        /// <summary>
        /// The stroke colour of the arcs when hovered.
        /// </summary>
        public List<string> HoverBorderColor { get; set; }

        /// <summary>
        /// The stroke width of the arcs when hovered.
        /// </summary>
        public int HoverBorderWidth { get; set; }
    }

    public class DoughnutChartDataset<T> : PieChartDataset<T>
    {
        // same as pie chart
    }

    public class PolarAreaChartDataset<T> : ChartDataset<T>
    {
        /// <summary>
        /// The fill colour of the arcs when hovered.
        /// </summary>
        public List<string> HoverBackgroundColor { get; set; }

        /// <summary>
        /// The stroke colour of the arcs when hovered.
        /// </summary>
        public List<string> HoverBorderColor { get; set; }

        /// <summary>
        /// The stroke width of the arcs when hovered.
        /// </summary>
        public int HoverBorderWidth { get; set; }
    }

    public class RadarChartDataset<T> : ChartDataset<T>
    {
        /// <summary>
        /// How to fill the area under the line.
        /// </summary>
        public bool Fill { get; set; }

        /// <summary>
        /// Bezier curve tension of the line. Set to 0 to draw straightlines.
        /// </summary>
        public float LineTension { get; set; }
    }

    public struct ChartColor
    {
        #region Constructors

        public ChartColor( byte red, byte green, byte blue )
            : this( red, green, blue, 1f )
        {
        }

        public ChartColor( byte red, byte green, byte blue, float alpha )
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public ChartColor( float red, float green, float blue )
            : this( red, green, blue, 1f )
        {
        }

        public ChartColor( float red, float green, float blue, float alpha )
        {
            R = (byte)( red * 255 );
            G = (byte)( green * 255 );
            B = (byte)( blue * 255 );
            A = alpha;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implicitly convert color to the string representation that is understood by the ChartJs.
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator string( ChartColor color ) => color.ToJsRgba();

        /// <summary>
        /// Creates the new color based on the supplied color component values.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static ChartColor FromRgba( byte red, byte green, byte blue, float alpha ) => new ChartColor( red, green, blue, alpha );

        /// <summary>
        /// Converts the color to the js function call.
        /// </summary>
        /// <returns></returns>
        public string ToJsRgba() => $"rgba({R},{G},{B},{A})";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the red component value of color structure.
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Gets or sets the green component value of color structure.
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Gets or sets the blue component value of color structure.
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// Gets or sets the alpha component value of color structure.
        /// </summary>
        public float A { get; set; }

        #endregion
    }
}
