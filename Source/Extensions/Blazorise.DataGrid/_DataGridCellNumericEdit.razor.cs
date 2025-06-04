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
public partial class _DataGridCellNumericEdit<TItem> : ComponentBase
{
    #region Members

    protected string elementId;

    /// <summary>
    /// Value data type.
    /// </summary>
    private Type valueType;

    #endregion

    #region Methods

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

    #endregion

    #region Properties

    private RenderFragment NumericPickerFragment => builder =>
    {
        var underlyingType = Nullable.GetUnderlyingType( valueType ) ?? valueType;
        var nullableType = typeof( Nullable<> ).MakeGenericType( underlyingType );
        var type = typeof( NumericPicker<> ).MakeGenericType( nullableType ); //"type" will always be nullable

        var cellValue = CellValue;

        builder.OpenComponent( 0, type );
        builder.AddAttribute( 1, nameof( NumericPicker<object>.Value ), cellValue );

        object valueChanged = underlyingType switch
        {
            not null when underlyingType == typeof( decimal ) => EventCallback.Factory.Create<decimal?>( this, OnCellValueChanged ),
            not null when underlyingType == typeof( double ) => EventCallback.Factory.Create<double?>( this, OnCellValueChanged ),
            not null when underlyingType == typeof( float ) => EventCallback.Factory.Create<float?>( this, OnCellValueChanged ),
            not null when underlyingType == typeof( int ) => EventCallback.Factory.Create<int?>( this, OnCellValueChanged ),
            not null when underlyingType == typeof( long ) => EventCallback.Factory.Create<long?>( this, OnCellValueChanged ),
            _ => throw new InvalidOperationException( $"Unsupported type {valueType}" )
        };

        builder.AddAttribute( 2, nameof( NumericPicker<object>.ValueChanged ), valueChanged );
        builder.AddAttribute( 3, nameof( BaseInputComponent<object>.ReadOnly ), Column.Readonly );
        builder.AddAttribute( 4, nameof( NumericPicker<object>.Step ), Column.Step );
        builder.AddAttribute( 5, nameof( NumericPicker<object>.Decimals ), Column.Decimals );
        builder.AddAttribute( 6, nameof( NumericPicker<object>.DecimalSeparator ), Column.DecimalSeparator );
        builder.AddAttribute( 6, nameof( NumericPicker<object>.GroupSeparator ), Column.GroupSeparator );
        builder.AddAttribute( 7, nameof( NumericPicker<object>.Culture ), Column.Culture );
        builder.AddAttribute( 8, nameof( NumericPicker<object>.ShowStepButtons ), Column.ShowStepButtons );
        builder.AddAttribute( 9, nameof( NumericPicker<object>.EnableStep ), Column.EnableStep );
        builder.AddAttribute( 10, nameof( NumericPicker<object>.ElementId ), elementId );
        builder.CloseComponent();
    };

    private RenderFragment NumericEditFragment => builder =>
    {
        var underlyingType = Nullable.GetUnderlyingType( valueType ) ?? valueType;
        var nullableType = typeof( Nullable<> ).MakeGenericType( underlyingType );
        var type = typeof( NumericEdit<> ).MakeGenericType( nullableType );

        var cellValue = CellValue;

        builder.OpenComponent( 0, type );
        builder.AddAttribute( 1, nameof( NumericEdit<object>.Value ), cellValue );

        object valueChanged = underlyingType switch
        {
            Type t when t == typeof( decimal ) => EventCallback.Factory.Create<decimal?>( this, OnCellValueChanged ),
            Type t when t == typeof( double ) => EventCallback.Factory.Create<double?>( this, OnCellValueChanged ),
            Type t when t == typeof( float ) => EventCallback.Factory.Create<float?>( this, OnCellValueChanged ),
            Type t when t == typeof( int ) => EventCallback.Factory.Create<int?>( this, OnCellValueChanged ),
            Type t when t == typeof( long ) => EventCallback.Factory.Create<long?>( this, OnCellValueChanged ),
            _ => throw new InvalidOperationException( $"Unsupported type {valueType}" )
        };

        builder.AddAttribute( 2, nameof( NumericEdit<object>.ValueChanged ), valueChanged );
        builder.AddAttribute( 3, nameof( BaseInputComponent<object>.ReadOnly ), Column.Readonly );
        builder.AddAttribute( 4, nameof( NumericEdit<object>.Step ), Column.Step );
        builder.AddAttribute( 5, nameof( NumericEdit<object>.Culture ), Column.Culture );
        builder.AddAttribute( 6, nameof( NumericEdit<object>.ElementId ), elementId );
        builder.CloseComponent();
    };

    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Inject] public IIdGenerator IdGenerator { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

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

    #endregion
}