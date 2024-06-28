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
public partial class _DataGridCellEdit<TItem> : ComponentBase
{
    protected string elementId;

    protected override void OnInitialized()
    {
        elementId = IdGenerator.Generate;
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            if ( ParentDataGrid.IsCellEdit && Column.CellEditing )
            {
                var cellValue = ParentDataGrid.ReadCellEditValue( Column.Field )?.ToString();
                var columnValue = Column.GetValue( ParentDataGrid.editItem )?.ToString();
                var valueHasChanged = cellValue != columnValue;

                await Task.Yield();
                if ( ParentDataGrid.IsCellEditSelectTextOnEdit && !valueHasChanged )
                {
                    await Select();
                }
                else
                {
                    await Focus();
                }

            }
        }
        await base.OnAfterRenderAsync( firstRender );
    }

    public async Task Focus()
    {
        await JSUtilitiesModule.Focus( default, elementId, true );
    }

    public async Task Select()
    {
        await JSUtilitiesModule.Select( default, elementId, true );
    }

    /// <summary>
    /// Updated the internal cell values.
    /// </summary>
    /// <typeparam name="T">Type of the value being changed.</typeparam>
    /// <param name="value">Value that is updating.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnEditValueChanged<T>( T value )
    {
        CellEditContext.CellValue = value;

        if ( ValidationItem != null )
            Column.SetValue( ValidationItem, value );

        return CellValueChanged.InvokeAsync( value );
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
    [Parameter] public DataGridColumn<TItem> Column { get; set; }

    /// <summary>
    /// Field name that this cell belongs to.
    /// </summary>
    [Parameter] public string Field { get; set; }

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
    /// Prevents user from editing the cell value.
    /// </summary>
    [Parameter] public bool Readonly { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

    /// <summary>
    /// Specifies the interval between valid values.
    /// </summary>
    [Parameter] public decimal? Step { get; set; }

    /// <summary>
    /// Maximum number of decimal places after the decimal separator.
    /// </summary>
    [Parameter] public int Decimals { get; set; } = 2;

    /// <summary>
    /// String to use as the decimal separator in numeric values.
    /// </summary>
    [Parameter] public string DecimalSeparator { get; set; } = ".";

    /// <summary>
    /// Helps define the language of an element.
    /// </summary>
    /// <remarks>
    /// https://www.w3schools.com/tags/ref_language_codes.asp
    /// </remarks>
    [Parameter] public string Culture { get; set; }

    /// <summary>
    /// If true, step buttons will be visible.
    /// </summary>
    [Parameter] public bool? ShowStepButtons { get; set; }

    /// <summary>
    /// If true, enables change of numeric value by pressing on step buttons or by keyboard up/down keys.
    /// </summary>
    [Parameter] public bool? EnableStep { get; set; }

    /// <summary>
    /// Hints at the type of data that might be entered by the user while editing the element or its contents.
    /// </summary>
    [Parameter] public DateInputMode DateInputMode { get; set; }
}