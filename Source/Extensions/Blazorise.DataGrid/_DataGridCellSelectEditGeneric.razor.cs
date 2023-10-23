#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Internal component for editing the row item cell value.
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TValue"></typeparam>
public partial class _DataGridCellSelectEditGeneric<TItem, TValue> : ComponentBase
{
    /// <summary>
    /// Column that this cell belongs to.
    /// </summary>
    [Parameter] public DataGridSelectColumn<TItem> Column { get; set; }

    /// <summary>
    /// Currently editing cell value.
    /// </summary>
    [Parameter] public TValue CellValue { get; set; }

    [Parameter] public bool ShowValidationFeedback { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

    private void OnSelectValueChanged( TValue value )
    {
        CellValueChanged.InvokeAsync( value );
    }

}