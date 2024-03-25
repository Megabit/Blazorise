#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides context for managing and interacting with the display state of columns in a data grid.
/// This includes maintaining a list of available columns and handling changes to their display states.
/// </summary>
/// <typeparam name="TItem">The type of the data items that the columns in the data grid are bound to.</typeparam>
public class ColumnChooserContext<TItem>
{
    /// <summary>
    /// Gets the list of columns that are available for display in the data grid.
    /// This collection includes both currently displayed columns and those that can be added to the grid.
    /// </summary>
    /// <value>The enumerable collection of <see cref="DataGridColumn{TItem}"/> objects representing the available columns.</value>
    public IEnumerable<DataGridColumn<TItem>> Columns { get; private set; }

    /// <summary>
    /// Gets the event callback that is invoked when the display state of any column changes.
    /// This event should be handled to update the UI accordingly, such as showing or hiding the column in the data grid.
    /// </summary>
    /// <value>The <see cref="EventCallback{TEvent}"/> where TEvent is <see cref="ColumnDisplayChangedEventArgs{TItem}"/>, representing the change event.</value>
    public EventCallback<ColumnDisplayChangedEventArgs<TItem>> ColumnDisplayChanged { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnChooserContext{TItem}"/> class with the specified columns and change event handler.
    /// </summary>
    /// <param name="columns">The initial list of columns that can be displayed in the data grid.</param>
    /// <param name="columnDisplayChanged">The event callback to be invoked when the display state of a column changes.</param>
    public ColumnChooserContext( IEnumerable<DataGridColumn<TItem>> columns, EventCallback<ColumnDisplayChangedEventArgs<TItem>> columnDisplayChanged )
    {
        Columns = columns;
        ColumnDisplayChanged = columnDisplayChanged;
    }
}