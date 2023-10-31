#region Using directives
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

        if ( Column.Data is not null )
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

        base.OnInitialized();
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

    #endregion
}