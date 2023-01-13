namespace Blazorise.Charts.DataLabels;

/// <summary>
/// Supplies information about a datalabels for each dataset.
/// </summary>
public class ChartDataLabelsDataset
{
    public ChartDataLabelsDataset()
    {
    }

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