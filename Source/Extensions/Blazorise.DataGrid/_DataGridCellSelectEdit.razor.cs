#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Internal component for editing the row item cell value.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class _DataGridCellSelectEdit<TItem> : ComponentBase
{
    /// <summary>
    /// Updated the internal cell values.
    /// </summary>
    /// <param name="value">Value that is updating.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnEditValueChanged( object value )
    {
        CellEditContext.CellValue = value;

        if ( ValidationItem != null )
            Column.SetValue( ValidationItem, value );

        return CellValueChanged.InvokeAsync( value );
    }

    /// <summary>
    /// Column that this cell belongs to.
    /// </summary>
    [Parameter] public DataGridSelectColumn<TItem> Column { get; set; }

    /// <summary>
    /// Instance of the currently editing row item.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Instance of the currently validating row item.
    /// </summary>
    [Parameter] public TItem ValidationItem { get; set; }

    /// <summary>
    /// Value data type.
    /// </summary>
    [Parameter] public Type ValueType { get; set; }

    /// <summary>
    /// Currently editing cell value.
    /// </summary>
    [Parameter] public CellEditContext<TItem> CellEditContext { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }
}