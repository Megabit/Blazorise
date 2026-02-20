#region Using directives
using System;
using System.ComponentModel;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the base class for cancellable events when datagrid item is saving.
/// </summary>
/// <typeparam name="TItem">Model type param.</typeparam>
public class CancellableRowChange<TItem> : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the cancelable event argument.
    /// </summary>
    /// <param name="oldItem">Old Saved item.</param>
    /// <param name="newItem">New Saved item.</param>
    public CancellableRowChange( TItem oldItem, TItem newItem )
    {
        OldItem = oldItem;
        NewItem = newItem;
    }

    /// <summary>
    /// Gets the model with the values before being edited.
    /// </summary>
    public TItem OldItem { get; }

    /// <summary>
    /// Gets the model with the edited values.
    /// </summary>
    public TItem NewItem { get; }
}