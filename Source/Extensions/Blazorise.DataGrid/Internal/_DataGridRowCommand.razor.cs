#region Using directives
using System;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid row command rendering and interaction in a DataGrid.
/// </summary>
public abstract class _BaseDataGridRowCommand<TItem> : ComponentBase, IDisposable
{
    /// <summary>
    /// Default flex behavior for command content.
    /// </summary>
    protected static readonly IFluentFlex DefaultFlex = Constants.FlexInlineFlex;

    /// <summary>
    /// Default spacing between row commands.
    /// </summary>
    protected static readonly IFluentGap DefaultGap = Constants.GapIs2;

    /// <inheritdoc />
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
                case nameof( Save ):
                    Save = (EventCallback)parameter.Value;
                    break;
                case nameof( EditState ):
                    EditState = (DataGridEditState)parameter.Value;
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

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <summary>
    /// Releases resources held by the base data grid row command.
    /// </summary>
    public void Dispose()
    {
        LocalizerService.LocalizationChanged -= OnLocalizationChanged;
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Builds cell style for the current row.
    /// </summary>
    protected string BuildCellStyle()
        => Column.BuildCellStyle( Item );

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected EventCallback Edit
        => EventCallback.Factory.Create( this, () => ParentDataGrid.Edit( Item ) );

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected EventCallback Delete
        => EventCallback.Factory.Create( this, () => ParentDataGrid.Delete( Item ) );

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected EventCallback Cancel
        => EventCallback.Factory.Create( this, ParentDataGrid.CancelInternal );

    /// <summary>
    /// Supplies localized text for row commands.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Supplies translated labels for row-level commands.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Indicates the operation currently active for the row.
    /// </summary>
    [Parameter] public DataGridEditState EditState { get; set; }

    /// <summary>
    /// Identifies the row targeted by the command.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Handles submission of the edited row.
    /// </summary>
    [Parameter] public EventCallback Save { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Supplies layout and styling for the command cell.
    /// </summary>
    [Parameter] public DataGridColumn<TItem> Column { get; set; }
}