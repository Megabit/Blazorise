#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides all the information about the <see cref="DataGridMultiSelectColumn{TItem}.SelectionChanged"/> event.
/// </summary>
/// <typeparam name="TItem">Type of the item associated with the row.</typeparam>
public class DataGridMultiSelectionChangedEventArgs<TItem> : EventArgs
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridMultiSelectionChangedEventArgs{TItem}"/> class.
    /// </summary>
    /// <param name="item">The model associated with the selected or deselected row.</param>
    /// <param name="selected">Indicates whether the row is selected or deselected.</param>
    public DataGridMultiSelectionChangedEventArgs( TItem item, bool selected )
    {
        Item = item;
        Selected = selected;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the model associated with the row.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets a value indicating whether the row is selected.
    /// </summary>
    public bool Selected { get; }

    #endregion
}