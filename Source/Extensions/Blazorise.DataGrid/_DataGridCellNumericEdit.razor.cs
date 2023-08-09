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
public partial class _DataGridCellNumericEdit<TItem> : ComponentBase
{
    /// <summary>
    /// Column that this cell belongs to.
    /// </summary>
    [Parameter] public DataGridNumericColumn<TItem> Column { get; set; }

    /// <summary>
    /// Currently editing cell value.
    /// </summary>
    [Parameter] public object CellValue { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

    /// <summary>
    /// Value data type.
    /// </summary>
    private Type valueType;

    public Task OnCellValueChanged<TValue>( TValue value )
        => CellValueChanged.InvokeAsync( value );

    protected override void OnInitialized()
    {
        valueType = Column.GetValueType();
        base.OnInitialized();
    }
}