#region Using directives
#endregion

using System.Collections.Generic;
using Blazorise.DataGrid.EventArguments;
using Microsoft.AspNetCore.Components;

namespace Blazorise.DataGrid;

public class ColumnChooserContext<TItem>
{
    public IEnumerable<DataGridColumn<TItem>> Columns { get; private set; }
    public EventCallback<ColumnDisplayChangedEventArgs<TItem>> ColumnDisplayChanged { get; private set; }


    public ColumnChooserContext( IEnumerable<DataGridColumn<TItem>> columns, EventCallback<ColumnDisplayChangedEventArgs<TItem>> columnDisplayChanged )
    {
        Columns = columns;
        ColumnDisplayChanged = columnDisplayChanged;
    }
}