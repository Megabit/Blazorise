#region Using directives
using System.Linq;
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

    private System.Collections.Generic.List<(string text, object value, bool disabled)> selectItems;
    protected override void OnInitialized()
    {
        if ( Column.Data is not null )
        {
            selectItems = new();
            foreach ( var item in Column.Data )
            {
                var text = Column.TextField?.Invoke( item );
                var value = Column.ValueField != null ? Column.ValueField.Invoke( item ) : default;
                var disabled = Column.ItemDisabled != null && Column.ItemDisabled.Invoke( item );
                selectItems.Add( (text, value, disabled) );
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
                .FirstOrDefault( x => ( x.value?.ToString() ?? string.Empty ).Equals( value?.ToString() ?? string.Empty, System.StringComparison.Ordinal ) );

            CellValueChanged.InvokeAsync( selectItem.value );
        }
        else
        {
            CellValueChanged.InvokeAsync( value );
        }
    }
}