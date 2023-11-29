#region Using directives
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

public abstract class _BaseDataGridRowMultiSelect<TItem> : ComponentBase
{
    #region Methods

    public override Task SetParametersAsync( ParameterView parameters )
    {
        foreach ( var parameter in parameters )
        {
            switch ( parameter.Name )
            {
                case nameof( Item ):
                    Item = (TItem)parameter.Value;
                    break;
                case nameof( Column ):
                    Column = (DataGridColumn<TItem>)parameter.Value;
                    break;
                case nameof( Checked ):
                    Checked = (bool)parameter.Value;
                    break;
                case nameof( CheckedChanged ):
                    CheckedChanged = (EventCallback<bool>)parameter.Value;
                    break;
                case nameof( CheckedClicked ):
                    CheckedClicked = (EventCallback)parameter.Value;
                    break;
                case nameof( ParentDataGrid ):
                    ParentDataGrid = (DataGrid<TItem>)parameter.Value;
                    break;
                default:
                    throw new ArgumentException( $"Unknown parameter: {parameter.Name}" );
            }
        }
        return base.SetParametersAsync( ParameterView.Empty );
    }

    internal Task OnCheckedChanged( bool @checked )
    {
        //Multi Select Checked State is bound to the Row Selected State
        return CheckedChanged.InvokeAsync( Checked );
    }

    internal Task OnCheckedClicked()
    {
        return CheckedClicked.InvokeAsync();
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

    [Parameter] public EventCallback<bool> CheckedChanged { get; set; }

    [Parameter] public EventCallback CheckedClicked { get; set; }

    #endregion
}