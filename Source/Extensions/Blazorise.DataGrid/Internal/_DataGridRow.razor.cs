#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.DataGrid.Internal;

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
    /// Holds information about the current Row.
    /// </summary>
    protected DataGridRowInfo<TItem> RowInfo;

    /// <summary>
    /// The Table Row Reference
    /// </summary>
    protected TableRow TableRowRef;

    /// <summary>
    /// Indicates whether a cell is currently being edited.
    /// </summary>
    protected bool cellEditing;

    /// <summary>
    /// Indicates whether the row should focus itself after render.
    /// </summary>
    private bool shouldFocusSelf;

    /// <summary>
    /// Indicates whether the row was selected during the last render.
    /// </summary>
    private bool wasSelected;

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

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            RowInfo.SetTableRow( TableRowRef );

            // initialise all internal cell values
            foreach ( var column in Columns )
            {
                if ( column.ExcludeFromInit )
                    continue;

                cellValues.Add( column.ElementId, new CellEditContext<TItem>( Item, column.GetValue( Item ), ParentDataGrid.UpdateCellEditValue, ParentDataGrid.ReadCellEditValue, ParentDataGrid.EditState ) );
            }
        }

        if ( shouldFocusSelf && ParentDataGrid.IsRowNavigable && TableRowRef is not null )
        {
            shouldFocusSelf = false;
            await TableRowRef.ElementRef.FocusAsync();
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    protected override Task OnParametersSetAsync()
    {
        var isSelected = IsSelected;

        if ( ParentDataGrid.IsRowNavigable && isSelected && !wasSelected )
            shouldFocusSelf = true;

        wasSelected = isSelected;

        return base.OnParametersSetAsync();
    }

    protected internal async Task HandleKeyDown( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter" )
        {
            if ( !ParentDataGrid.SelectedRow.IsEqual( Item ) )
                await ParentDataGrid.Select( Item );
            return;
        }

        var navigationItem = ParentDataGrid.SelectedRow ?? Item;
        var displayData = ParentDataGrid.DisplayData;
        var displayList = displayData as IList<TItem> ?? displayData.ToList();

        if ( displayList.Count == 0 )
            return;

        var idx = displayList.Index( x => x.IsEqual( navigationItem ) );
        if ( idx < 0 )
            return;

        var lastIndex = displayList.Count - 1;
        var pageSize = ParentDataGrid.GetRowNavigationPageSize();
        var targetIndex = idx;

        switch ( eventArgs.Code )
        {
            case "ArrowUp" when idx > 0:
                targetIndex = idx - 1;
                break;
            case "ArrowDown" when idx < lastIndex:
                targetIndex = idx + 1;
                break;
            case "PageUp" when idx > 0:
                targetIndex = Math.Max( 0, idx - pageSize );
                break;
            case "PageDown" when idx < lastIndex:
                targetIndex = Math.Min( lastIndex, idx + pageSize );
                break;
            default:
                return;
        }

        if ( targetIndex == idx )
            return;

        await ParentDataGrid.Select( displayList[targetIndex] );
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

    protected internal async Task HandleMouseLeave( MouseEventArgs eventArgs )
    {
        mouseIsOver = false;
        await ParentDataGrid.OnRowMouseLeaveCommand( new( Item, eventArgs ) );
    }
    protected internal async Task HandleMouseOver( MouseEventArgs eventArgs )
    {
        mouseIsOver = true;
        await ParentDataGrid.OnRowMouseOverCommand( new( Item, eventArgs ) );
    }

    protected internal async Task HandleClick( MouseEventArgs eventArgs )
    {
        await ParentDataGrid.OnRowClickedCommand( new( Item, eventArgs ) );

        var selectable = ParentDataGrid.RowSelectable?.Invoke( new( Item, DataGridSelectReason.RowClick ) ) ?? true;

        if ( selectable )
        {
            if ( !ParentDataGrid.MultiSelect )
                await HandleSingleSelectClick( eventArgs );
            else
                await HandleMultiSelectClick( eventArgs );
        }

        if ( ParentDataGrid.IsHierarchyEnabled && ParentDataGrid.ExpandOnRowClick )
            await ParentDataGrid.ToggleRow( Item );

        await ParentDataGrid.ToggleDetailRow( Item, DetailRowTriggerType.RowClick );
    }

    private async Task HandleMultiSelectClick( MouseEventArgs eventArgs )
    {
        var isSelected = ( ParentDataGrid.SelectedRows == null || ( ParentDataGrid.SelectedRows != null && !ParentDataGrid.SelectedRows.Any( x => x.IsEqual( Item ) ) ) );
        var shiftClick = ( eventArgs.ShiftKey && eventArgs.ToMouseButton() == MouseButton.Left );

        await OnMultiSelectCommand( new( Item, isSelected || shiftClick, shiftClick ) );
    }

    private bool IsCtrlClick( MouseEventArgs eventArgs )
    {
        var isMacOsCtrl = ParentDataGrid.IsClientMacintoshOS && eventArgs.MetaKey;
        return ( eventArgs.CtrlKey || isMacOsCtrl ) && eventArgs.ToMouseButton() == MouseButton.Left;
    }

    private async Task HandleSingleSelectClick( MouseEventArgs eventArgs )
    {
        // Un-select row if the user is holding the ctrl key on already selected row.
        if ( ParentDataGrid.SingleSelect && IsCtrlClick( eventArgs )
                                         && ParentDataGrid.SelectedRow != null
                                         && Item.IsEqual( ParentDataGrid.SelectedRow ) )
        {
            await ParentDataGrid.Select( default );
        }
        else
        {
            await ParentDataGrid.Select( Item );
        }
    }

    protected internal Task HandleDoubleClick( MouseEventArgs eventArgs )
    {
        return ParentDataGrid.OnRowDoubleClickedCommand( new( Item, eventArgs ) );
    }

    protected internal Task HandleContextMenu( MouseEventArgs eventArgs )
    {
        return ParentDataGrid.OnRowContextMenuCommand( new( Item, eventArgs ) );
    }

    protected internal Task OnMultiSelectCommand( DataGridMultiSelectionChangedEventArgs<TItem> eventArgs )
    {
        return ParentDataGrid.OnMultiSelectCommand( eventArgs );
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

    protected CellDisplayContext<TItem> BuildCellDisplayContext( DataGridColumn<TItem> column, TItem item, object cellValue = null )
    {
        object resolvedValue = cellValue ?? column.GetValue( item );
        string displayValue = column.FormatDisplayValue( resolvedValue );
        int rowIndex = RowInfo is null ? -1 : ParentDataGrid.ResolveItemIndex( RowInfo.Item );

        return new CellDisplayContext<TItem>( item, column, RowInfo, rowIndex, resolvedValue, displayValue, ParentDataGrid );
    }

    protected Task ToggleRow()
        => ParentDataGrid.ToggleRow( Item );

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