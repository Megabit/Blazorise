#region Using directives

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

#endregion

namespace Blazorise.DataGrid;

public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
{
    #region Members

    protected bool ShiftKeyPressed;

    #endregion

    #region Methods

    internal async Task OnCheckedChanged( bool @checked )
    {
        var selectable = ParentDataGrid.RowSelectable?.Invoke( new(Item, DataGridSelectReason.MultiSelectClick)) ?? true;
        if ( selectable && (ParentDataGrid.MultiSelectColumn?.PreventRowClick==false ))
             await CheckedChanged.InvokeAsync( ( @checked, ShiftKeyPressed ) );
    }

    protected string BuildCellStyle()
    {
        var style = Column.BuildCellStyle( Item );

        var sb = new StringBuilder();

        if ( !string.IsNullOrEmpty( style ) )
            sb.Append( style );

        if ( Column.Width is not null && Column.FixedPosition == TableColumnFixedPosition.None )
            sb.Append( $"; width: {Column.Width}" );

        return sb.ToString().TrimStart( ' ', ';' );
    }

    #endregion

    #region Properties

    [Inject] protected IBehaviourProvider BehaviourProvider { get; set; }

    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    [Parameter] public DataGridColumn<TItem> Column { get; set; }

    [Parameter] public bool Checked { get; set; }

    [Parameter] public EventCallback<(bool Checked, bool ShiftKey)> CheckedChanged { get; set; }

    #endregion
}