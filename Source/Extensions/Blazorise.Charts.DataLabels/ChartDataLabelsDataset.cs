namespace Blazorise.Charts.DataLabels;

/// <summary>
/// Supplies information about a datalabels for each dataset.
/// </summary>
public class ChartDataLabelsDataset
{
    /// <summary>
    /// Creates an unassigned dataset-label configuration.
    /// </summary>
    public ChartDataLabelsDataset()
    {
    }

    /// <summary>
    /// Associates label styling with a chart dataset.
    /// </summary>
    /// <param name="datasetIndex">Zero-based dataset position.</param>
    /// <param name="options">Label presentation for that dataset.</param>
    public ChartDataLabelsDataset( int datasetIndex, ChartDataLabelsOptions options )
    {
        DatasetIndex = datasetIndex;
        Options = options;
    }

    /// <summary>
    /// Gets the current dataset index.
    /// </summary>
    public int DatasetIndex { get; set; }

    /// <summary>
    /// Gets the data labels that will be applied to the dataset for the <see cref="DatasetIndex"/>.
    /// </summary>
    public ChartDataLabelsOptions Options { get; set; }
}