#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the Batch Change Event Arguments.
/// </summary>
/// <typeparam name="TItem">Model type param.</typeparam>
public class DataGridBatchChangeEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the batch change event argument.
    /// </summary>
    /// <param name="batchChange"></param>
    public DataGridBatchChangeEventArgs( DataGridBatchEditItem<TItem> batchChange )
    {
        Item = batchChange;
    }

    public DataGridBatchEditItem<TItem> Item { get; private set; }
}