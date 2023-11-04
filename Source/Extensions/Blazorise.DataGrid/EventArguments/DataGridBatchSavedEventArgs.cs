#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the Batch Saved Event Arguments.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridBatchSavedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the batch saved event argument.
    /// </summary>
    /// <param name="batchEditItems"></param>
    public DataGridBatchSavedEventArgs( IReadOnlyList<DataGridBatchEditItem<TItem>> batchEditItems )
    {
        Items = batchEditItems;
    }

    public IReadOnlyList<DataGridBatchEditItem<TItem>> Items { get; private set; }

}
