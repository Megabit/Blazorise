#region Using directives
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the Batch Saving Event Arguments.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class DataGridBatchSavingEventArgs<TItem> : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the batch saving event argument.
    /// </summary>
    /// <param name="batchEditItems"></param>
    public DataGridBatchSavingEventArgs( IReadOnlyList<DataGridBatchEditItem<TItem>> batchEditItems  )
    {
        Items = batchEditItems;
    }

    public IReadOnlyList<DataGridBatchEditItem<TItem>> Items { get; private set; }

}
