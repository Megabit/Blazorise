#region Using directives
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid row multi select rendering and interaction in a DataGrid.
/// </summary>
public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
{
    #region Members

    /// <summary>
    /// Controls shift key pressed behavior for the base data grid row multi select.
    /// </summary>
    protected bool ShiftKeyPressed;

    #endregion

    #region Methods

    internal async Task OnCheckedChanged( bool @checked )
    {
        var selectable = ParentDataGrid.RowSelectable?.Invoke( new( Item, DataGridSelectReason.MultiSelectClick ) ) ?? true;

        if ( selectable )
        {
            await CheckedChanged.InvokeAsync( new( Item, @checked, ShiftKeyPressed ) );
        }
    }

    /// <summary>
    /// Builds cell style for the current row.
    /// </summary>
    protected string BuildCellStyle()
    {
        var style = Column.BuildCellStyle( Item );

        var sb = new StringBuilder();

        if ( !string.IsNullOrEmpty( style ) )
            sb.Append( style );

        return sb.ToString().TrimStart( ' ', ';' );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider that supplies selection-cell behavior and styling.
    /// </summary>
    [Inject] protected IBehaviourProvider BehaviourProvider { get; set; }

    /// <summary>
    /// Identifies the row represented by this selection cell.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Supplies styling metadata for the selection column.
    /// </summary>
    [Parameter] public DataGridColumn<TItem> Column { get; set; }

    /// <summary>
    /// Indicates whether the row is part of the current selection.
    /// </summary>
    [Parameter] public bool Checked { get; set; }

    /// <summary>
    /// Reports changes to the row's selection state.
    /// </summary>
    [Parameter] public EventCallback<DataGridMultiSelectionChangedEventArgs<TItem>> CheckedChanged { get; set; }

    #endregion
}