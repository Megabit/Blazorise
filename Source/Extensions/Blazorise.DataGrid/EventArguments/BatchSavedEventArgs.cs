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
public class BatchSavedEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the batch saved event argument.
    /// </summary>
    /// <param name="batchEditItems"></param>
    public BatchSavedEventArgs( IReadOnlyList<BatchEditItem<TItem>> batchEditItems )
    {
        Items = batchEditItems;
    }

    public IReadOnlyList<BatchEditItem<TItem>> Items { get; private set; }

}
