namespace Blazorise.Charts.Streaming
{
    public class ChartStreamingOptions
    {
        /// <summary>
        /// Duration of the chart in milliseconds (how much time of data it will show).
        /// </summary>
        public int Duration { get; set; } = 10000;

        /// <summary>
        /// Refresh interval of data in milliseconds. onRefresh callback function will be called at this interval.
        /// </summary>
        public int Refresh { get; set; } = 1000;

        /// <summary>
        /// Delay added to the chart in milliseconds so that upcoming values are known before lines are plotted. This makes the chart look like a continual stream rather than very jumpy on the right hand side. Specify the maximum expected delay.
        /// </summary>
        public int Delay { get; set; } = 0;

        /// <summary>
        /// Frequency at which the chart is drawn on a display (frames per second). This option can be set at chart level but not at axis level. Decrease this value to save CPU power (https://github.com/nagix/chartjs-plugin-streaming#lowering-cpu-usage).
        /// </summary>
        public int FrameRate { get; set; } = 30;
    }
}
