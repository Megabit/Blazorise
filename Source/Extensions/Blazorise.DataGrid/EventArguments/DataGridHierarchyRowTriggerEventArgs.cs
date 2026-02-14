#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Holds hierarchy row trigger context configuration.
/// </summary>
/// <typeparam name="TItem">Type of the data model.</typeparam>
public class DataGridHierarchyRowTriggerEventArgs<TItem> : EventArgs
{
    /// <summary>
    /// Initializes a new instance of hierarchy-row-trigger event argument.
    /// </summary>
    /// <param name="item">Row item.</param>
    public DataGridHierarchyRowTriggerEventArgs( TItem item )
    {
        Item = item;
    }

    /// <summary>
    /// Gets the row item.
    /// </summary>
    public TItem Item { get; private set; }

    /// <summary>
    /// Gets or sets whether the row is expandable. Defaults to <c>true</c>.
    /// </summary>
    public bool Expandable { get; set; } = true;
}