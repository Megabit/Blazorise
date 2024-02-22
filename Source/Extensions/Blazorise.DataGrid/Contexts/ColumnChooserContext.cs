#region Using directives

using System.Collections.Generic;
using Blazorise.DataGrid.EventArguments;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Context for the column chooser.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class ColumnChooserContext<TItem>
{
    /// <summary>
    /// The list of columns that can be displayed.
    /// </summary>
    public IEnumerable<DataGridColumn<TItem>> Columns { get; private set; }

    /// <summary>
    /// The event that is raised when the column display changes.
    /// </summary>
    public EventCallback<ColumnDisplayChangedEventArgs<TItem>> ColumnDisplayChanged { get; private set; }

    public ColumnChooserContext( IEnumerable<DataGridColumn<TItem>> columns, EventCallback<ColumnDisplayChangedEventArgs<TItem>> columnDisplayChanged )
    {
        Columns = columns;
        ColumnDisplayChanged = columnDisplayChanged;
    }
}