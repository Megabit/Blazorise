#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides all the information about hierarchy row expand or collapse events.
/// </summary>
/// <typeparam name="TItem">Type of the data model.</typeparam>
public class DataGridHierarchyRowEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of hierarchy-row event argument.
    /// </summary>
    /// <param name="item">Row item.</param>
    public DataGridHierarchyRowEventArgs( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Gets the row item.
    /// </summary>
    public TItem Item { get; }
}