namespace Blazorise.Charts.Trendline
{
    /// <summary>
    /// Supplies information about a trendline.
    /// </summary>
    public class ChartTrendlineData
    {
        /// <summary>
        /// The index of the dataset to add the trendline to. By default this is 0. If you have more than one line, increase the index by one for each dataset you have.
        /// </summary>
        public int DatasetIndex { get; set; } = 0;

        /// <summary>
        /// The colour of the trendline
        /// </summary>
        public ChartColor Color { get; set; } = ChartColor.FromRgba( 100, 0, 0, 1 );

        /// <summary>
        /// Can be "solid" or "dotted"
        /// </summary>
        public string LineStyle { get; set; } = "solid";

        /// <summary>
        /// Line width
        /// </summary>
        public int Width { get; set; } = 2;
    }
}
