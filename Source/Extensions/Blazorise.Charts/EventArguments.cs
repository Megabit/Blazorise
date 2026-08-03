#region Using directives
using System;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Supports chart mouse event args behavior in chart components.
/// </summary>
public class ChartMouseEventArgs : EventArgs
{
    /// <summary>
    /// Creates a chart mouse event args instance.
    /// </summary>
    public ChartMouseEventArgs( int datasetIndex, int index, object model )
    {
        DatasetIndex = datasetIndex;
        Index = index;
        Model = model;
    }

    /// <summary>
    /// Gets the dataset index in which the data point is resides.
    /// </summary>
    public int DatasetIndex { get; }

    /// <summary>
    /// Gets the data point index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the full data point info.
    /// </summary>
    /// <remarks>
    /// TODO: use ChartModel instead of object type once the System.Text.Json serializer is fixed!
    /// </remarks>
    public object Model { get; }
}