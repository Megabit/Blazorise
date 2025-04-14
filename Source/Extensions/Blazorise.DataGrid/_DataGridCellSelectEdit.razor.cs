#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Internal component for editing the row item cell value.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public partial class _DataGridCellSelectEdit<TItem> : ComponentBase
{
    #region Members

    private System.Collections.Generic.List<SelectItem> selectItems;

    private class SelectItem
    {
        public string Text { get; private set; }
        public object Value { get; private set; }
        public bool Disabled { get; private set; }

        public SelectItem( string text, object value, bool disabled )
        {
            Text = text;
            Value = value;
            Disabled = disabled;
        }
    }

    protected string elementId;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        elementId = IdGenerator.Generate;
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if ( Column?.Data is not null && selectItems?.Count != Column.Data.Count() )
        {
            selectItems = new();
            foreach ( var item in Column.Data )
            {
                var text = Column.TextField?.Invoke( item );
                var value = Column.ValueField != null ? Column.ValueField.Invoke( item ) : default;
                var disabled = Column.ItemDisabled != null && Column.ItemDisabled.Invoke( item );
                selectItems.Add( new( text, value, disabled ) );
            }
        }
        base.OnParametersSet();
    }

    private void OnSelectedValueChanged( object value )
    {
        if ( selectItems is not null )
        {
            //Using a Select<object> makes it so the internal value representation is always a string instead of the actual Value type from ValueField.
            //This is a problem, because when the value is passed back to the DataGrid internal item setter, it is passed as a string instead of the actual Value type.
            //This workaround, resolves the string value back to the actual Value type by matching the string representation back to the pre-generated list of select items which contain the actual Value type.
            var selectItem = selectItems
                .FirstOrDefault( x => ( x.Value?.ToString() ?? string.Empty ).Equals( value?.ToString() ?? string.Empty, System.StringComparison.Ordinal ) );

            CellValueChanged.InvokeAsync( selectItem?.Value );
        }
        else
        {
            CellValueChanged.InvokeAsync( value );
        }
    }

    private async Task OnSelectedValuesChanged( IReadOnlyList<object> values )
    {
        var columnType = Column.GetValueType( default );

        if ( columnType.IsArray )
        {
            var valueType = columnType.GetElementType();
            if ( valueType == typeof( int ) )
            {
                await CellValueChanged.InvokeAsync( values?.Select( x => int.Parse( x.ToString() ) )?.ToArray() );
            }
            else if ( valueType == typeof( short ) )
            {
                await CellValueChanged.InvokeAsync( values?.Select( x => short.Parse( x.ToString() ) )?.ToArray() );
            }
            else if ( valueType == typeof( decimal ) )
            {
                await CellValueChanged.InvokeAsync( values?.Select( x => decimal.Parse( x.ToString() ) )?.ToArray() );
            }
            else if ( valueType == typeof( double ) )
            {
                await CellValueChanged.InvokeAsync( values?.Select( x => double.Parse( x.ToString() ) )?.ToArray() );
            }
            else if ( valueType == typeof( float ) )
            {
                await CellValueChanged.InvokeAsync( values?.Select( x => float.Parse( x.ToString() ) )?.ToArray() );
            }
            else
            {
                await CellValueChanged.InvokeAsync( values?.Select( x => x.ToString() )?.ToArray() );
            }
            return;
        }

        await CellValueChanged.InvokeAsync( values?.Select( x => x.ToString() )?.ToArray() );
    }

    public object[] GetSelectedValues()
    {
        var columnType = Column.GetValueType( default );

        if ( CellValue is not null && columnType.IsArray )
        {
            var valueType = columnType.GetElementType();
            if ( valueType == typeof( int ) )
            {
                return ( CellValue as int[] )?.Select( x => (object)x )?.ToArray();
            }
            else if ( valueType == typeof( short ) )
            {
                return ( CellValue as short[] )?.Select( x => (object)x )?.ToArray();
            }
            else if ( valueType == typeof( decimal ) )
            {
                return ( CellValue as decimal[] )?.Select( x => (object)x )?.ToArray();
            }
            else if ( valueType == typeof( double ) )
            {
                return ( CellValue as double[] )?.Select( x => (object)x )?.ToArray();
            }
            else if ( valueType == typeof( float ) )
            {
                return ( CellValue as float[] )?.Select( x => (object)x )?.ToArray();
            }
        }
        return CellValue as object[];
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

    #endregion

    #region Properties

    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Inject] public IIdGenerator IdGenerator { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
    /// </summary>
    [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

    /// <summary>
    /// Column that this cell belongs to.
    /// </summary>
    [Parameter] public DataGridSelectColumn<TItem> Column { get; set; }

    /// <summary>
    /// Currently editing cell value.
    /// </summary>
    [Parameter] public object CellValue { get; set; }

    /// <summary>
    /// Raises when cell value changes.
    /// </summary>
    [Parameter] public EventCallback<object> CellValueChanged { get; set; }

    [Parameter] public bool ShowValidationFeedback { get; set; }
    
    /// <summary>
    /// Determines if the cell is currently in filter row
    /// </summary>
    [Parameter] public bool FilterDisplay { get; set; } 

    #endregion
}