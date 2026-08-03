#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.DataGrid.Internal;

/// <summary>
/// Supports base data grid row edit rendering and interaction in a DataGrid.
/// </summary>
public abstract class _BaseDataGridRowEdit<TItem> : ComponentBase, IDisposable
{
    #region Members

    /// <summary>
    /// Creates the edit model used by the base data grid row edit.
    /// </summary>
    protected EventCallbackFactory callbackFactory = new();

    /// <summary>
    /// Validation component for the inline edit row.
    /// </summary>
    protected Validations validations;

    /// <summary>
    /// Indicates whether the base data grid row edit is invalid.
    /// </summary>
    protected bool isInvalid;

    /// <summary>
    /// Callback invoked for member.
    /// </summary>
    protected EventCallback Cancel
        => EventCallback.Factory.Create( this, ParentDataGrid.CancelInternal );

    /// <summary>
    /// Default flex behavior for command-cell content.
    /// </summary>
    protected static readonly IFluentFlex DefaultFlex = Constants.FlexInlineFlex;

    /// <summary>
    /// Default spacing between command-cell controls.
    /// </summary>
    protected static readonly IFluentGap DefaultGap = Constants.GapIs2;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <summary>
    /// Releases resources held by the base data grid row edit.
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
    /// Responds to validation status changes in the edit form.
    /// </summary>
    protected void ValidationsStatusChanged( ValidationsStatusChangedEventArgs args )
    {
        isInvalid = args.Status == ValidationStatus.Error;

        InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Validates every field in the inline edit row.
    /// </summary>
    internal protected Task<bool> ValidateAll()
    {
        return validations.ValidateAll();
    }

    /// <summary>
    /// Submits the current inline row edits.
    /// </summary>
    internal protected async Task Save()
    {
        await ParentDataGrid.SaveInternal();
    }

    /// <summary>
    /// Controls member behavior for the base data grid row edit.
    /// </summary>
    protected bool CanSaveFromOnScreenKeyboard
        => ( ParentDataGrid.CommandColumn is null || ParentDataGrid.CommandColumn.SaveCommandAllowed )
            && ParentDataGrid.SubmitFormOnEnter
            && !ParentDataGrid.IsCellEdit;

    /// <summary>
    /// Overrides Enter-key behavior while an on-screen keyboard is active.
    /// </summary>
    protected OnScreenKeyboardEnterKeyBehavior? DataGridOnScreenKeyboardEnterKeyBehaviorOverride
        => ParentDataGrid.IsCellEdit
            ? OnScreenKeyboardEnterKeyBehavior.KeyDown
            : CascadedOnScreenKeyboardEnterKeyBehaviorOverride;

    /// <summary>
    /// Handles cell click.
    /// </summary>
    protected async Task HandleCellClick( DataGridColumn<TItem> column )
    {
        await ParentDataGrid.HandleCellEdit( column, Item );
    }

    /// <summary>
    /// Handles cell key down.
    /// </summary>
    protected async Task HandleCellKeyDown( KeyboardEventArgs args, DataGridColumn<TItem> column )
    {
        var isCellEdit = ParentDataGrid.IsCellEdit && column.CellEditing;
        if ( !isCellEdit )
            return;

        //most of the keydown operations (arrows,focus) are handled in datagrid.js 
        if ( args.Code == "Escape" )
        {
            await Cancel.InvokeAsync();
            return;
        }

        if ( args.Code == "Enter" || args.Code == "NumpadEnter" )
        {
            var batchEditItem = ParentDataGrid.BatchEdit
                ? ParentDataGrid.GetBatchEditItem( Item )
                : null;

            await Save();

            if ( ParentDataGrid.EditState == DataGridEditState.Edit )
                return;

            if ( args.ShiftKey )
            {
                await HandleCellEditSelectPreviousRow( column, batchEditItem );
            }
            else
            {
                await HandleCellEditSelectNextRow( column, batchEditItem );
            }

            return;
        }

        if ( args.Code == "Tab" )
        {
            var batchEditItem = ParentDataGrid.BatchEdit
                ? ParentDataGrid.GetBatchEditItem( Item )
                : null;

            await Save();

            if ( ParentDataGrid.EditState == DataGridEditState.Edit )
                return;

            if ( args.ShiftKey )
            {
                await HandleCellEditSelectPreviousColumn( column, batchEditItem );
            }
            else
            {
                await HandleCellEditSelectNextColumn( column, batchEditItem );
            }
        }
    }

    private async Task HandleCellEditSelectNextColumn( DataGridColumn<TItem> currentColumn, DataGridBatchEditItem<TItem> batchEditItem )
    {
        var currentIdx = OrderedColumnsForEditing?.Index( x => x.IsEqual( currentColumn ) ) ?? -1;
        var nextColumn = OrderedColumnsForEditing.ElementAtOrDefault( currentIdx + 1 );

        if ( nextColumn is not null )
        {
            await ParentDataGrid.HandleCellEdit( nextColumn, GetEditingItem( batchEditItem ) );
        }
        else
        {
            var nextRowFirstColumn = OrderedColumnsForEditing.FirstOrDefault();
            var nextVisibleRow = GetVisibleRowByOffset( batchEditItem, 1 );

            if ( nextVisibleRow is not null && nextRowFirstColumn is not null )
            {
                await ParentDataGrid.HandleCellEdit( nextRowFirstColumn, nextVisibleRow );
            }
        }
    }

    private async Task HandleCellEditSelectPreviousColumn( DataGridColumn<TItem> currentColumn, DataGridBatchEditItem<TItem> batchEditItem )
    {
        var currentIdx = OrderedColumnsForEditing?.Index( x => x.IsEqual( currentColumn ) ) ?? -1;
        var previousColumn = OrderedColumnsForEditing?.ElementAtOrDefault( currentIdx - 1 );

        if ( previousColumn is not null )
        {
            await ParentDataGrid.HandleCellEdit( previousColumn, GetEditingItem( batchEditItem ) );
        }
        else
        {
            var previousRowLastColumn = OrderedColumnsForEditing.LastOrDefault();
            var previousVisibleRow = GetVisibleRowByOffset( batchEditItem, -1 );

            if ( previousVisibleRow is not null && previousRowLastColumn is not null )
            {
                await ParentDataGrid.HandleCellEdit( previousRowLastColumn, previousVisibleRow );
            }
        }
    }

    private Task HandleCellEditSelectNextRow( DataGridColumn<TItem> currentColumn, DataGridBatchEditItem<TItem> batchEditItem )
        => HandleCellEditSelectRow( currentColumn, batchEditItem, 1 );

    private Task HandleCellEditSelectPreviousRow( DataGridColumn<TItem> currentColumn, DataGridBatchEditItem<TItem> batchEditItem )
        => HandleCellEditSelectRow( currentColumn, batchEditItem, -1 );

    private async Task HandleCellEditSelectRow( DataGridColumn<TItem> currentColumn, DataGridBatchEditItem<TItem> batchEditItem, int rowOffset )
    {
        var visibleRow = GetVisibleRowByOffset( batchEditItem, rowOffset );

        if ( visibleRow is not null )
        {
            await ParentDataGrid.HandleCellEdit( currentColumn, visibleRow );
        }
    }

    private TItem GetEditingItem( DataGridBatchEditItem<TItem> batchEditItem )
        => batchEditItem is null
            ? Item
            : batchEditItem.NewItem;

    private TItem GetVisibleRowItem( DataGridBatchEditItem<TItem> batchEditItem )
        => batchEditItem is null
            ? Item
            : batchEditItem.OldItem;

    private TItem GetVisibleRowByOffset( DataGridBatchEditItem<TItem> batchEditItem, int rowOffset )
    {
        if ( ParentDataGrid.DisplayData.IsNullOrEmpty() )
            return default;

        var currentEditRowIdx = ParentDataGrid.DisplayData.Index( x => x.IsEqual( GetVisibleRowItem( batchEditItem ) ) );

        return currentEditRowIdx < 0
            ? default
            : ParentDataGrid.DisplayData.ElementAtOrDefault( currentEditRowIdx + rowOffset );
    }

    /// <summary>
    /// Builds cell display context for the current row.
    /// </summary>
    protected CellDisplayContext<TItem> BuildCellDisplayContext( DataGridColumn<TItem> column, TItem item, object cellValue = null )
    {
        object resolvedValue = cellValue ?? column.GetValue( item );
        string displayValue = column.FormatDisplayValue( resolvedValue );
        DataGridRowInfo<TItem> rowInfo = ParentDataGrid.GetRowInfo( item );
        int rowIndex = rowInfo is null ? -1 : ParentDataGrid.ResolveItemIndex( rowInfo.Item );

        return new CellDisplayContext<TItem>( item, column, rowInfo, rowIndex, resolvedValue, displayValue, ParentDataGrid );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Supplies localized text for inline edit commands.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Supplies translated labels for inline editing controls.
    /// </summary>
    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Identifies the item being edited inline.
    /// </summary>
    [Parameter] public TItem Item { get; set; }

    /// <summary>
    /// Holds the model used to validate inline changes.
    /// </summary>
    [Parameter] public TItem ValidationItem { get; set; }

    /// <summary>
    /// Provides the columns participating in inline editing.
    /// </summary>
    [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    /// <summary>
    /// Orders the columns eligible for inline editing.
    /// </summary>
    protected IEnumerable<DataGridColumn<TItem>> OrderedEditableColumns
    {
        get
        {
            return Columns
                .Where( column => !column.ExcludeFromEdit && column.CellValueIsEditable )
                .OrderBy( column => column.EditOrder ?? column.DisplayOrder );
        }
    }

    /// <summary>
    /// Editable columns arranged in their display order.
    /// </summary>
    protected IEnumerable<DataGridColumn<TItem>> OrderedColumnsForEditing
    {
        get
        {
            return ParentDataGrid
                .EditableColumns
                .OrderBy( column => column.EditOrder ?? column.DisplayOrder );
        }
    }

    /// <summary>
    /// Columns currently visible in the edit row.
    /// </summary>
    protected IEnumerable<DataGridColumn<TItem>> DisplayableColumns
    {
        get
        {
            return Columns
                .Where( column => column.IsDisplayable || column.Displaying )
                .OrderBy( column => column.DisplayOrder );
        }
    }

    /// <summary>
    /// Editable values keyed by column field name.
    /// </summary>
    [Parameter] public Dictionary<string, CellEditContext<TItem>> CellValues { get; set; }

    /// <summary>
    /// Selects the editing layout used for the row.
    /// </summary>
    [Parameter] public DataGridEditMode EditMode { get; set; }

    /// <summary>
    /// Specifies the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    /// <summary>
    /// Carries the surrounding Enter-key behavior into the inline editor.
    /// </summary>
    [CascadingParameter( Name = "OnScreenKeyboardEnterKeyBehaviorOverride" )] protected OnScreenKeyboardEnterKeyBehavior? CascadedOnScreenKeyboardEnterKeyBehaviorOverride { get; set; }

    #endregion
}