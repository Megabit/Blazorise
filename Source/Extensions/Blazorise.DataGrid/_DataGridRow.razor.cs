#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

#endregion

namespace Blazorise.DataGrid;

public abstract class _BaseDataGridRow<TItem> : BaseDataGridComponent
{
    #region Members

    protected DataGridBatchEditItem<TItem> BatchEditItem;

    protected bool mouseIsOver = false;

    /// <summary>
    /// List of columns used to build this row.
    /// </summary>
    protected IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// Holds the internal value for every cell in the row.
    /// </summary>
    protected Dictionary<string, CellEditContext<TItem>> cellValues = new Dictionary<string, CellEditContext<TItem>>();

    /// <summary>
    /// Holds the reference to the multiSelect cell.
    /// </summary>
    protected _DataGridRowMultiSelect<TItem> multiSelect;

    /// <summary>
    /// If click came propagated from MultiSelect Check
    /// Funnels the selection logic into HandleClick.
    /// </summary>
    protected bool clickFromMultiSelectCheck;

    /// <summary>
    /// Holds information about the current Row.
    /// </summary>
    protected DataGridRowInfo<TItem> RowInfo;

    /// <summary>
    /// The Table Row Reference
    /// </summary>
    protected TableRow TableRowRef;

    protected bool cellEditing;

    #endregion

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
                case nameof( ChildContent ):
                    ChildContent = (RenderFragment)parameter.Value;
                    break;
                case nameof( ParentDataGrid ):
                    ParentDataGrid = (DataGrid<TItem>)parameter.Value;
                    break;
                case nameof( SelectedRow ):
                    SelectedRow = (TItem)parameter.Value;
                    break;
                case nameof( SelectedRows ):
                    SelectedRows = (List<TItem>)parameter.Value;
                    break;
                default:
                    throw new ArgumentException( $"Unknown parameter: {parameter.Name}" );
            }
        }

        BatchEditItem = ParentDataGrid.GetBatchEditItemByOriginal( Item );

        return base.SetParametersAsync( ParameterView.Empty );
    }

    protected override async Task OnInitializedAsync()
    {
        Columns = ParentDataGrid.DisplayableColumns;
        RowInfo = new DataGridRowInfo<TItem>( Item, this.Columns );

        ParentDataGrid.AddRow( RowInfo );

        if ( ParentDataGrid.DetailRowStartsVisible )
            await ParentDataGrid.ToggleDetailRow( RowInfo, DetailRowTriggerType.Manual, false, true );

        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            RowInfo.SetTableRow( TableRowRef );

            // initialise all internal cell values
            foreach ( var column in Columns )
            {
                if ( column.ExcludeFromInit )
                    continue;

                cellValues.Add( column.ElementId, new CellEditContext<TItem>( Item, column.GetValue( Item ), ParentDataGrid.UpdateCellEditValue, ParentDataGrid.ReadCellEditValue ) );
            }
        }

        return base.OnAfterRenderAsync( firstRender );
    }

    protected internal async Task HandleKeyDown( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter" )
        {
            if ( !ParentDataGrid.SelectedRow.IsEqual( this.Item ) )
                await ParentDataGrid.Select( this.Item );
            return;
        }

        if ( eventArgs.Code == "ArrowUp" )
        {
            var idx = ParentDataGrid.DisplayData.Index( x => x.IsEqual( this.Item ) );
            if ( idx > 0 )
            {
                await ParentDataGrid.Select( ParentDataGrid.DisplayData.ElementAt( idx - 1 ) );
                return;
            }
        }

        if ( eventArgs.Code == "ArrowDown" )
        {
            var idx = ParentDataGrid.DisplayData.Index( x => x.IsEqual( this.Item ) );
            if ( idx < ParentDataGrid.DisplayData.Count() - 1 )
            {
                await ParentDataGrid.Select( ParentDataGrid.DisplayData.ElementAt( idx + 1 ) );
                return;
            }
        }
    }

    protected async Task HandleCellKeyDown( KeyboardEventArgs args, DataGridColumn<TItem> column )
    {
        if ( !ParentDataGrid.IsCellEdit )
            return;

        var isKeyboardKeyText = args.IsTextKey();
        if ( !args.IsModifierKey() )
        {
            if ( isKeyboardKeyText )
            {
                await ParentDataGrid.HandleCellEdit( column, GetCurrentItem(), args.Key );
            }
            else if ( args.Code == "Enter" || args.Code == "NumpadEnter" )
            {
                await ParentDataGrid.HandleCellEdit( column, GetCurrentItem(), null );
            }
            else if ( args.Code == "Backspace" )
            {
                await ParentDataGrid.HandleCellEdit( column, GetCurrentItem(), string.Empty );
            }
        }
    }

    protected async Task HandleCellFocus( FocusEventArgs args, DataGridColumn<TItem> column )
    {
        if ( !ParentDataGrid.IsCellNavigable )
            return;

        await ParentDataGrid.HandleSelectedCell( Item, RowInfo, column );
    }

    protected bool BindMouseLeave()
        => ParentDataGrid.RowMouseLeave.HasDelegate || ParentDataGrid.RowOverlayTemplate is not null;

    protected bool BindMouseOver()
        => ParentDataGrid.RowMouseOver.HasDelegate || ParentDataGrid.RowOverlayTemplate is not null;

    protected internal async Task HandleMouseLeave( BLMouseEventArgs eventArgs )
    {
        mouseIsOver = false;
        await ParentDataGrid.OnRowMouseLeaveCommand( new( Item, eventArgs ) );
    }
    protected internal async Task HandleMouseOver( BLMouseEventArgs eventArgs )
    {
        mouseIsOver = true;
        await ParentDataGrid.OnRowMouseOverCommand( new( Item, eventArgs ) );
    }

    protected internal async Task HandleClick( BLMouseEventArgs eventArgs )
    {
        var multiSelectPreventRowClick = clickFromMultiSelectCheck && ( ParentDataGrid.MultiSelectColumn?.PreventRowClick ?? false );
        if ( !clickFromMultiSelectCheck )
            await ParentDataGrid.OnRowClickedCommand( new( Item, eventArgs ) );

        var selectable = ParentDataGrid.RowSelectable?.Invoke( new( Item, clickFromMultiSelectCheck ? DataGridSelectReason.MultiSelectClick : DataGridSelectReason.RowClick ) ) ?? true;

        if ( selectable )
        {
            if ( !ParentDataGrid.MultiSelect )
                await HandleSingleSelectClick( eventArgs );
            else
                await HandleMultiSelectClick( eventArgs );
        }

        if ( !multiSelectPreventRowClick )
        {
            await ParentDataGrid.ToggleDetailRow( Item, DetailRowTriggerType.RowClick );
        }

        clickFromMultiSelectCheck = false;
    }

    private async Task HandleMultiSelectClick( BLMouseEventArgs eventArgs )
    {
        if ( ParentDataGrid.MultiSelect )
        {
            var isSelected = ( ParentDataGrid.SelectedRows == null || ( ParentDataGrid.SelectedRows != null && !ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) ) );
            var shiftClick = ( eventArgs.ShiftKey && eventArgs.Button == MouseButton.Left );

            await OnMultiSelectCommand( isSelected || shiftClick, shiftClick );
        }
    }

    private bool IsCtrlClick( BLMouseEventArgs eventArgs )
    {
        var isMacOsCtrl = ParentDataGrid.IsClientMacintoshOS && eventArgs.MetaKey;
        return ( eventArgs.CtrlKey || isMacOsCtrl ) && eventArgs.Button == MouseButton.Left;
    }

    private async Task HandleSingleSelectClick( BLMouseEventArgs eventArgs )
    {
        // Un-select row if the user is holding the ctrl key on already selected row.
        if ( ParentDataGrid.SingleSelect && IsCtrlClick( eventArgs )
                                         && ParentDataGrid.SelectedRow != null
                                         && Item.IsEqual( ParentDataGrid.SelectedRow ) )
        {
            await ParentDataGrid.Select( default );
        }
        else if ( !eventArgs.ShiftKey
                  && ParentDataGrid.MultiSelect
                  && ParentDataGrid.SelectedRows != null
                  && ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) )
        {
            // If the user selects an already selected multiselect row, seems like it should be more transparent,
            // to just de-select both normal and multi selection
            // Remove this, if that is not the case!!
            await ParentDataGrid.Select( default );
        }
        else
        {
            await ParentDataGrid.Select( Item );
        }
    }

    protected internal Task HandleDoubleClick( BLMouseEventArgs eventArgs )
    {
        return ParentDataGrid.OnRowDoubleClickedCommand( new( Item, eventArgs ) );
    }

    protected internal Task HandleContextMenu( BLMouseEventArgs eventArgs )
    {
        return ParentDataGrid.OnRowContextMenuCommand( new( Item, eventArgs ) );
    }

    protected internal Task OnMultiSelectCommand( bool selected, bool shiftClick )
    {
        return ParentDataGrid.OnMultiSelectCommand( new( Item, selected, shiftClick ) );
    }


    protected async Task OnMultiSelectCheckedChanged( (bool isChecked, bool shift ) args )
    {
        await OnMultiSelectCommand( args.isChecked, args.shift );
    }

    protected Cursor GetHoverCursor()
        => ParentDataGrid.RowHoverCursor == null ? Cursor.Pointer : ParentDataGrid.RowHoverCursor( Item );

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            ParentDataGrid.RemoveRow( RowInfo );
        }

        return base.DisposeAsync( disposing );
    }

    protected TItem GetCurrentItem()
    {
        return BatchEditItem is null
            ? Item
            : BatchEditItem.NewItem;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the row is selected.
    /// </summary>
    protected bool IsSelected =>
        ( ( ParentDataGrid.EditState == DataGridEditState.None || ParentDataGrid.SelectionMode == DataGridSelectionMode.Single ) && ParentDataGrid.SelectedRow.IsEqual( Item ) )
        ||
        ( ParentDataGrid.SelectionMode == DataGridSelectionMode.Multiple && ParentDataGrid.SelectedRows != null && ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) );

    /// <summary>
    /// Gets the row background color.
    /// </summary>
    protected Background GetBackground( DataGridRowStyling styling, DataGridRowStyling selectedStyling, DataGridRowStyling batchEditSelectedStyling ) => ( IsSelected
        ? selectedStyling?.Background
        : batchEditSelectedStyling?.Background
          ?? styling?.Background ) ?? Blazorise.Background.Default;

    /// <summary>
    /// Gets the row color.
    /// </summary>
    protected Color GetColor( DataGridRowStyling styling, DataGridRowStyling selectedStyling, DataGridRowStyling batchEditSelectedStyling ) => ( IsSelected
        ? selectedStyling?.Color
        : batchEditSelectedStyling?.Color
          ?? styling?.Color ) ?? Blazorise.Color.Default;

    /// <summary>
    /// Gets the row class names.
    /// </summary>
    protected string GetClass( DataGridRowStyling styling, DataGridRowStyling selectedStyling, DataGridRowStyling batchEditSelectedStyling ) => IsSelected
        ? selectedStyling?.Class
        : batchEditSelectedStyling?.Class
          ?? styling?.Class;

    /// <summary>
    /// Gets the row styles.
    /// </summary>
    protected string GetStyle( DataGridRowStyling styling, DataGridRowStyling selectedStyling, DataGridRowStyling batchEditSelectedStyling ) => IsSelected
        ? selectedStyling?.Style
        : batchEditSelectedStyling?.Style
          ?? styling?.Style;

    /// <summary>
    /// Gets the cell background color.
    /// </summary>
    protected string GetCellStyle( DataGridColumn<TItem> column, DataGridCellStyling styling, DataGridCellStyling selectedStyling, DataGridCellStyling batchEditStyling )
    {
        var cellStyle = column.BuildCellStyle( GetCurrentItem() );

        var styleFromStyling = selectedStyling?.Style ?? batchEditStyling?.Style ?? styling.Style;

        if ( !string.IsNullOrEmpty( styleFromStyling ) )
        {
            cellStyle += $";{styleFromStyling}";
        }

        return cellStyle;
    }

    /// <summary>
    /// Gets the cell background color.
    /// </summary>
    protected string GetCellClass( DataGridColumn<TItem> column, DataGridCellStyling styling, DataGridCellStyling selectedStyling, DataGridCellStyling batchEditStyling )
    {
        var cellClass = column.CellClass?.Invoke( GetCurrentItem() ) ?? string.Empty;

        var classFromStyling = selectedStyling?.Class ?? batchEditStyling?.Class ?? styling.Class;

        if ( !string.IsNullOrEmpty( classFromStyling ) )
        {
            cellClass += $" {classFromStyling}";
        }

        return cellClass;
    }

    /// <summary>
    /// Gets the cell background color.
    /// </summary>
    protected Background GetCellBackground( DataGridCellStyling styling, DataGridCellStyling selectedStyling, DataGridCellStyling batchEditStyling )
        => selectedStyling?.Background ?? batchEditStyling?.Background ?? styling.Background ?? Blazorise.Background.Default;

    /// <summary>
    /// Gets the cell color.
    /// </summary>
    protected Color GetCellColor( DataGridCellStyling styling, DataGridCellStyling selectedStyling, DataGridCellStyling batchEditStyling )
        => selectedStyling?.Color ?? batchEditStyling?.Color ?? styling.Color ?? Blazorise.Color.Default;

    /// <summary>
    /// Gets the cell text color.
    /// </summary>
    protected TextColor GetCellTextColor( DataGridCellStyling styling, DataGridCellStyling selectedStyling, DataGridCellStyling batchEditStyling )
        => selectedStyling?.TextColor ?? batchEditStyling?.TextColor ?? styling.TextColor ?? Blazorise.TextColor.Default;

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Item associated with the data set.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets currently selected row.
    /// </summary>
    [Parameter] public TItem SelectedRow { get; set; }

    /// <summary>
    /// Gets or sets currently selected rows.
    /// </summary>
    [Parameter] public List<TItem> SelectedRows { get; set; }

    #endregion
}