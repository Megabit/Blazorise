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
    /// <param name="shiftKey">True if the user is holding shift key.</param>
    public DataGridMultiSelectionChangedEventArgs( TItem item, bool selected, bool shiftKey )
    {
        Item = item;
        Selected = selected;
        ShiftKey = shiftKey;
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

    /// <summary>
    /// Returns true if user has pressed shift key.
    /// </summary>
    public bool ShiftKey { get; }

    #endregion
}