#region Using directives
#endregion

namespace Blazorise.Charts.Streaming
{
    /// <summary>
    /// Supplies information about a dataset data point.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ChartStreamingData<TItem>
    {
        public ChartStreamingData( string datasetLabel, int datasetIndex )
        {
            DatasetLabel = datasetLabel;
            DatasetIndex = datasetIndex;
        }

        /// <summary>
        /// Gets the current dataset display name.
        /// </summary>
        public string DatasetLabel { get; }

        /// <summary>
        /// Gets the current dataset index.
        /// </summary>
        public int DatasetIndex { get; }

        /// <summary>
        /// Gets or sets the data point.
        /// </summary>
        public TItem Value { get; set; }
    }
}
