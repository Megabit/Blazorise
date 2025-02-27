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

namespace Blazorise.DataGrid;

public abstract class _BaseDataGridRowEdit<TItem> : ComponentBase, IDisposable
{
    #region Members

    protected EventCallbackFactory callbackFactory = new();

    protected Validations validations;

    protected bool isInvalid;

    protected EventCallback Cancel
        => EventCallback.Factory.Create( this, ParentDataGrid.CancelInternal );

    protected static readonly IFluentFlex DefaultFlex = Flex.InlineFlex;

    protected static readonly IFluentGap DefaultGap = Gap.Is2;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    public void Dispose()
    {
        LocalizerService.LocalizationChanged -= OnLocalizationChanged;
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    protected void ValidationsStatusChanged( ValidationsStatusChangedEventArgs args )
    {
        isInvalid = args.Status == ValidationStatus.Error;

        InvokeAsync( StateHasChanged );
    }

    internal protected Task<bool> ValidateAll()
    {
        return validations.ValidateAll();
    }

    internal protected async Task Save()
    {
        await ParentDataGrid.SaveInternal();
    }

    protected async Task HandleCellClick( DataGridColumn<TItem> column )
    {
        await ParentDataGrid.HandleCellEdit( column, Item );
    }

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
            await Save();
            return;
        }

        if ( args.Code == "Tab" )
        {
            var batchEditItem = ParentDataGrid.BatchEdit
                ? ParentDataGrid.GetBatchEditItemByLastEditItem( Item ) ?? ParentDataGrid.GetBatchEditItemByOriginal( Item )
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
            var item = batchEditItem is null
                ? Item
                : batchEditItem.NewItem;
            await ParentDataGrid.HandleCellEdit( nextColumn, item );
        }
        else
        {
            if ( !ParentDataGrid.DisplayData.IsNullOrEmpty() )
            {
                var item = batchEditItem is null
                    ? Item
                    : batchEditItem.OldItem;

                var currentEditRowIdx = ParentDataGrid.DisplayData.Index( x => x.IsEqual( item ) );
                var nextVisibleRow = ParentDataGrid.DisplayData.ElementAtOrDefault( currentEditRowIdx + 1 );
                var nextRowFirstColumn = OrderedColumnsForEditing.FirstOrDefault();
                if ( nextVisibleRow is not null && nextRowFirstColumn is not null )
                {
                    await ParentDataGrid.HandleCellEdit( nextRowFirstColumn, nextVisibleRow );
                }
            }
        }
    }

    private async Task HandleCellEditSelectPreviousColumn( DataGridColumn<TItem> currentColumn, DataGridBatchEditItem<TItem> batchEditItem )
    {
        var currentIdx = OrderedColumnsForEditing?.Index( x => x.IsEqual( currentColumn ) ) ?? -1;
        var previousColumn = OrderedColumnsForEditing?.ElementAtOrDefault( currentIdx - 1 );

        if ( previousColumn is not null )
        {
            var item = batchEditItem is null
                ? Item
                : batchEditItem.NewItem;

            await ParentDataGrid.HandleCellEdit( previousColumn, item );
        }
        else
        {
            if ( !ParentDataGrid.DisplayData.IsNullOrEmpty() )
            {
                var item = batchEditItem is null
                    ? Item
                    : batchEditItem.OldItem;

                var currentEditRowIdx = ParentDataGrid.DisplayData.Index( x => x.IsEqual( item ) );
                var previousVisibleRow = ParentDataGrid.DisplayData.ElementAtOrDefault( currentEditRowIdx - 1 );
                var previousRowLastColumn = OrderedColumnsForEditing.LastOrDefault();
                if ( previousVisibleRow is not null && previousRowLastColumn is not null )
                {
                    await ParentDataGrid.HandleCellEdit( previousRowLastColumn, previousVisibleRow );
                }
            }

        }
    }

    #endregion

    #region Properties

    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    [Parameter] public TItem Item { get; set; }

    [Parameter] public TItem ValidationItem { get; set; }

    [Parameter] public IEnumerable<DataGridColumn<TItem>> Columns { get; set; }

    protected IEnumerable<DataGridColumn<TItem>> OrderedEditableColumns
    {
        get
        {
            return Columns
                .Where( column => !column.ExcludeFromEdit && column.CellValueIsEditable )
                .OrderBy( column => column.EditOrder ?? column.DisplayOrder );
        }
    }

    protected IEnumerable<DataGridColumn<TItem>> OrderedColumnsForEditing
    {
        get
        {
            return ParentDataGrid
                .EditableColumns
                .OrderBy( column => column.EditOrder ?? column.DisplayOrder );
        }
    }

    protected IEnumerable<DataGridColumn<TItem>> DisplayableColumns
    {
        get
        {
            return Columns
                .Where( column => column.IsDisplayable || column.Displaying )
                .OrderBy( column => column.DisplayOrder );
        }
    }

    [Parameter] public Dictionary<string, CellEditContext<TItem>> CellValues { get; set; }

    [Parameter] public DataGridEditMode EditMode { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}