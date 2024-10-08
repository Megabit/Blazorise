#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Internal component for editing the row item cell value.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class _DataGridCellDatePicker<TItem> : ComponentBase
{
    protected string elementId;

    /// <summary>
    /// Value data type.
    /// </summary>
    private Type valueType;

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        valueType = Column.GetValueType( default );
        elementId = IdGenerator.Generate;
        base.OnInitialized();
    }

    public Task OnCellValueChanged<TValue>( TValue value )
        => CellValueChanged.InvokeAsync( value );

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            if ( ParentDataGrid.IsCellEdit && Column.CellEditing )
            {
                await Task.Yield();
                await Focus();
            }
        }
        await base.OnAfterRenderAsync( firstRender );
    }

    public async Task Focus()
    {
        await JSUtilitiesModule.Focus( default, elementId, true );
    }


    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Inject] public IIdGenerator IdGenerator { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Column that this cell belongs to.
    /// </summary>
    [Parameter] public DataGridDateColumn<TItem> Column { get; set; }

    /// <summary>
    /// Currently editing cell value.
    /// </summary>
    [Parameter] public object CellValue { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

}