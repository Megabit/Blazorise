#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the Batch Change Event Arguments.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class BatchChangeEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the batch change event argument.
    /// </summary>
    /// <param name="batchChange"></param>
    public BatchChangeEventArgs( BatchEditItem<TItem> batchChange )
    {
        Item = batchChange;
    }

    public BatchEditItem<TItem> Item { get; private set; }

}