#region Using directives
#endregion

namespace Blazorise.Charts.Streaming;

/// <summary>
/// Supplies information about a dataset data point.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class ChartStreamingData<TItem>
{
    /// <summary>
    /// Creates a streaming data descriptor for the selected chart dataset.
    /// </summary>
    /// <param name="datasetLabel">Label of the target dataset.</param>
    /// <param name="datasetIndex">Position of the target dataset.</param>
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