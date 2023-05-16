#region Using directives
using System;
using System.ComponentModel;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents the base class for items saved by the datagrid.
/// </summary>
/// <typeparam name="TItem">Model type param.</typeparam>
/// <typeparam name="TValues">Values type param.</typeparam>
public class SavedRowItem<TItem, TValues> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the event argument.
    /// </summary>
    /// <param name="oldItem">Old Saved item.</param>
    /// <param name="newItem">New Saved item.</param>
    /// <param name="values">Edited values.</param>
    public SavedRowItem( TItem oldItem, TItem newItem, TValues values )
    {
        OldItem = oldItem;
        NewItem = newItem;
        Values = values;
    }

    /// <summary>
    /// Gets the model that was saved.
    /// </summary>
    [Obsolete( "CancellableRowChange: The Item is deprecated, please use the OldItem instead." )]
    [EditorBrowsable( EditorBrowsableState.Never )]
    public TItem Item => OldItem;

    /// <summary>
    /// Gets the model with the values before being edited.
    /// </summary>
    public TItem OldItem { get; }

    /// Gets the model with the edited values.
    public TItem NewItem { get; }

    /// <summary>
    /// Values that are being edited by the datagrid.
    /// </summary>
    public TValues Values { get; }
}