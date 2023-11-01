#region Using directives
using System.Collections.Generic;
using System.ComponentModel;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the Batch Saving Event Arguments.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class BatchSavingEventArgs<TItem> : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the batch saving event argument.
    /// </summary>
    /// <param name="batchEditItems"></param>
    public BatchSavingEventArgs( IReadOnlyList<BatchEditItem<TItem>> batchEditItems  )
    {
        Items = batchEditItems;
    }

    public IReadOnlyList<BatchEditItem<TItem>> Items { get; private set; }

}
