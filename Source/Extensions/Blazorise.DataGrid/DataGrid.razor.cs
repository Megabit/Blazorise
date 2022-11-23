#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.DataGrid.Enums;
using Blazorise.DataGrid.Models;
using Blazorise.DataGrid.Utils;
using Blazorise.Extensions;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// The DataGrid component llows you to display and manage data in a tabular (rows/columns) format.
    /// </summary>
    /// <typeparam name="TItem">Type parameter for the model displayed in the <see cref="DataGrid{TItem}"/>.</typeparam>
    [CascadingTypeParameter( nameof( TItem ) )]
    public partial class DataGrid<TItem> : BaseDataGridComponent
    {
        #region Members

        /// <summary>
        /// Keeps track of Virtualize State.
        /// </summary>
        private VirtualizeState virtualizeState;

        /// <summary>
        /// Element reference to the DataGrid's inner virtualize.
        /// </summary>
        private Virtualize<TItem> virtualizeRef;

        /// <summary>
        /// Gets or sets current selection mode.
        /// </summary>
        private DataGridSelectionMode selectionMode;

        /// <summary>
        /// Element reference to the DataGrid's inner table.
        /// </summary>
        private Table tableRef;

        /// <summary>
        /// Original data-source.
        /// </summary>
        private IEnumerable<TItem> data;

        /// <summary>
        /// Optional aggregate data.
        /// </summary>
        private IEnumerable<TItem> aggregateData;

        /// <summary>
        /// Holds the filtered data based on the filter.
        /// </summary>
        private List<TItem> filteredData = new();

        /// <summary>
        /// Holds the filtered data to display based on the current page.
        /// </summary>
        private IEnumerable<TItem> viewData;

        /// <summary>
        /// Marks the grid to reload entire data source based on the current filter settings.
        /// </summary>
        private bool dirtyFilter;

        /// <summary>
        /// Marks the grid to refresh currently visible page.
        /// </summary>
        private bool dirtyView = true;

        /// <summary>
        /// Keeps track whether the user has changed the filter for Virtualize purposes.
        /// </summary>
        private bool virtualizeFilterChanged;

        /// <summary>
        /// Holds the state of sorted columns grouped by the sort-mode.
        /// </summary>
        protected Dictionary<DataGridSortMode, List<DataGridColumn<TItem>>> sortByColumnsDictionary = new()
        {
            { DataGridSortMode.Single, new() },
            { DataGridSortMode.Multiple, new() },
        };

        private readonly Lazy<Func<TItem>> newItemCreator;

        /// <summary>
        /// Currently editing item.
        /// </summary>
        protected TItem editItem;

        /// <summary>
        /// Copy of the <see cref="editItem"/> that is used only as temporary object for data-annotation validation.
        /// </summary>
        protected internal TItem validationItem;

        /// <summary>
        /// State of the currently editing item.
        /// </summary>
        protected DataGridEditState editState = DataGridEditState.None;

        /// <summary>
        /// Holds the values for the editing fields.
        /// </summary>
        protected internal Dictionary<string, CellEditContext<TItem>> editItemCellValues;

        /// <summary>
        /// Holds the values for the filter fields.
        /// </summary>
        protected Dictionary<string, CellEditContext<TItem>> filterCellValues;

        /// <summary>
        /// Holds the pagination templates
        /// </summary>
        protected PaginationTemplates<TItem> paginationTemplates;

        /// <summary>
        /// Holds the pagination context
        /// </summary>
        protected PaginationContext<TItem> paginationContext;

        /// <summary>
        /// Holds the last known selected row index.
        /// </summary>
        protected internal int lastSelectedRowIndex;

        #endregion

        #region Constructors

        public DataGrid()
        {
            newItemCreator = new( () => FunctionCompiler.CreateNewItem<TItem>() );

            paginationTemplates = new();
            paginationContext = new( this );
        }

        #endregion

        #region Methods

        #region Setup

        /// <summary>
        /// Inspects User Agent for a client using a Macintosh Operating System.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsUserAgentMacintoshOS()
            => ( await JSUtilitiesModule.GetUserAgent() ).Contains( "Mac", StringComparison.InvariantCultureIgnoreCase );

        /// <summary>
        /// Sets the height for the FixedHeader table feature.
        /// </summary>
        /// <returns></returns>
        private string GetFixedTableHeaderHeight()
        {
            if ( Virtualize )
                return VirtualizeOptions?.DataGridHeight ?? "500px";
            else
                return FixedHeaderDataGridHeight;
        }

        /// <summary>
        /// Sets the max height for the FixedHeader table feature.
        /// </summary>
        /// <returns></returns>
        private string GetFixedTableHeaderMaxHeight()
        {
            if ( Virtualize )
                return VirtualizeOptions?.DataGridMaxHeight ?? "500px";
            else
                return FixedHeaderDataGridMaxHeight;
        }

        /// <summary>
        /// Links the child column with this datagrid.
        /// </summary>
        /// <param name="column">Column to link with this datagrid.</param>
        public void AddColumn( DataGridColumn<TItem> column )
        {
            Columns.Add( column );

            if ( column.CurrentSortDirection != SortDirection.Default )
                HandleSortColumn( column, false );

            // save command column reference for later
            if ( CommandColumn == null && column is DataGridCommandColumn<TItem> commandColumn )
            {
                CommandColumn = commandColumn;
            }

            if ( MultiSelectColumn == null && column is DataGridMultiSelectColumn<TItem> multiSelectColumn )
            {
                MultiSelectColumn = multiSelectColumn;
            }
        }

        /// <summary>
        /// Links the child row with this datagrid.
        /// </summary>
        /// <param name="row">Row to add.</param>
        public void AddRow( DataGridRowInfo<TItem> row )
        {
            Rows.Add( row );
        }

        /// <summary>
        /// Removes an existing link of a child column with this datagrid.
        /// <para>Returns:
        ///     true if item is successfully removed; otherwise, false. 
        /// </para>
        /// </summary>
        /// <param name="column">Column to link with this datagrid.</param>
        public bool RemoveColumn( DataGridColumn<TItem> column )
            => Columns.Remove( column );

        /// <summary>
        /// Links the child row with this datagrid.
        /// </summary>
        /// <param name="row">Row to remove.</param>
        public bool RemoveRow( DataGridRowInfo<TItem> row )
            => Rows.Remove( row );

        /// <summary>
        /// Links the child column with this datagrid.
        /// </summary>
        /// <param name="aggregate">Aggregate column to link with this datagrid.</param>
        public void AddAggregate( DataGridAggregate<TItem> aggregate )
        {
            Aggregates.Add( aggregate );
        }

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await CheckMultipleSelectionSetEmpty( parameters );

            await base.SetParametersAsync( parameters );
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                IsClientMacintoshOS = await IsUserAgentMacintoshOS();
                await JSModule.Initialize( tableRef.ElementRef, ElementId );
                paginationContext.SubscribeOnPageSizeChanged( OnPageSizeChanged );
                paginationContext.SubscribeOnPageChanged( OnPageChanged );
                
                if ( Theme is not null )
                {
                    Theme.Changed += OnThemeChanged;
                }

                if ( ManualReadMode || VirtualizeManualReadMode )
                    await Reload();

                return;
            }

            await HandleVirtualize();

            await base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                if ( paginationContext is not null )
                {
                    paginationContext.UnsubscribeOnPageSizeChanged( OnPageSizeChanged );
                    paginationContext.UnsubscribeOnPageChanged( OnPageChanged );
                    paginationContext.CancellationTokenSource?.Dispose();
                    paginationContext.CancellationTokenSource = null;
                }

                if ( Theme is not null )
                {
                    Theme.Changed -= OnThemeChanged;
                }
            }

            return base.DisposeAsync( disposing );
        }

        /// <summary>
        /// Tracks whether the user explicitly set SelectedRows to null or empty and makes sure SelectedRow is synced.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private async Task CheckMultipleSelectionSetEmpty( ParameterView parameters )
        {
            if ( SelectionMode == DataGridSelectionMode.Multiple )
            {
                if ( parameters.TryGetValue<List<TItem>>( nameof( SelectedRows ), out var changedSelectedRows ) )
                {
                    //If we note SelectedRows is empty. Let's make sure SelectedRow is syncronized.
                    if ( changedSelectedRows.IsNullOrEmpty() && !( SelectedRow?.Equals( default ) ?? true ) )
                    {
                        SelectedRow = default;
                        await SelectedRowChanged.InvokeAsync( default );
                    }
                }
            }
        }

        private async Task HandleSelectionModeChanged()
        {
            if ( selectionMode == DataGridSelectionMode.Multiple && SelectedRow != null )
            {
                SelectedRows ??= new();

                if ( !SelectedRows.Contains( SelectedRow ) && Data.Contains( SelectedRow ) )
                {
                    SelectedRows.Add( SelectedRow );

                    await SelectedRowsChanged.InvokeAsync( SelectedRows );
                }
            }
            else if ( selectionMode == DataGridSelectionMode.Single && SelectedRows != null )
            {
                SelectedRows = null;

                await SelectedRowsChanged.InvokeAsync( SelectedRows );
            }

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Handles Datagrid's <see cref="Virtualize"/>.
        /// </summary>
        /// <returns></returns>
        private async ValueTask HandleVirtualize()
        {
            if ( Virtualize )
            {
                VirtualizeOptions ??= new();

                if ( editState == DataGridEditState.Edit && EditMode != DataGridEditMode.Popup && VirtualizeOptions.ScrollRowOnEdit )
                    virtualizeState.EditLastKnownScroll = await JSModule.ScrollTo( tableRef.ElementRef, ClassProvider.TableRowHoverCursor() );
            }
            else
            {
                if ( virtualizeState.WasActive )
                {
                    virtualizeState.WasActive = false;
                    await Reload();
                }
            }

            virtualizeState.WasActive = Virtualize;
        }

        private ValueTask VirtualizeScrollToTop()
            => tableRef.ScrollToPixels( 0 );

        private async ValueTask VirtualizeOnEditCompleteScroll()
        {
            if ( virtualizeState.EditLastKnownScroll.HasValue )
            {
                await tableRef.ScrollToPixels( virtualizeState.EditLastKnownScroll.Value );
                virtualizeState.EditLastKnownScroll = null;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// An event raised when theme settings changes.
        /// </summary>
        /// <param name="sender">An object that raised the event.</param>
        /// <param name="eventArgs"></param>
        private void OnThemeChanged( object sender, EventArgs eventArgs )
        {
            InvokeAsync( StateHasChanged );
        }

        private async void OnPageSizeChanged( int pageSize )
        {
            paginationContext.CancellationTokenSource?.Cancel();
            paginationContext.CancellationTokenSource = new();

            await InvokeAsync( () => PageSizeChanged.InvokeAsync( pageSize ) );

            await Reload( paginationContext.CancellationTokenSource.Token );
        }

        private async void OnPageChanged( int currentPage )
        {
            paginationContext.CancellationTokenSource?.Cancel();
            paginationContext.CancellationTokenSource = new();

            await InvokeAsync( () => PageChanged.InvokeAsync( new( currentPage, PageSize ) ) );

            await Reload( paginationContext.CancellationTokenSource.Token );
        }

        #endregion

        #region Commands

        /// <summary>
        /// Sets the DataGrid into the New state mode.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task New()
        {
            if ( Virtualize && EditMode != DataGridEditMode.Popup )
            {
                VirtualizeScrollToTop();
            }

            TItem newItem = NewItemCreator != null ? NewItemCreator.Invoke() : CreateNewItem();

            NewItemDefaultSetter?.Invoke( newItem );

            InitEditItem( newItem );

            editState = DataGridEditState.New;

            return InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Sets the DataGrid into the Edit state mode for the specified item.
        /// </summary>
        /// <param name="item">Item for which to set the edit mode.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Edit( TItem item )
        {
            TItem editingItem = EditItemCreator != null ? EditItemCreator.Invoke( item ) : item;

            InitEditItem( editingItem );

            editState = DataGridEditState.Edit;

            return InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Deletes the specified item from the <see cref="Data"/> source.
        /// </summary>
        /// <param name="item">Item to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Delete( TItem item )
        {
            if ( Data is ICollection<TItem> data )
            {
                if ( await IsSafeToProceed( RowRemoving, item ) )
                {
                    var itemIsSelected = SelectedRow.IsEqual( item );
                    if ( UseInternalEditing )
                    {
                        if ( data.Contains( item ) )
                            data.Remove( item );

                        if ( itemIsSelected )
                        {
                            SelectedRow = default;
                            await SelectedRowChanged.InvokeAsync( SelectedRow );
                        }
                    }

                    if ( editState == DataGridEditState.Edit && itemIsSelected )
                        editState = DataGridEditState.None;

                    await RowRemoved.InvokeAsync( item );

                    SetDirty();
                }
            }

            // When deleting and the page becomes empty and we aren't the first page:
            // go to the previous page
            if ( ManualReadMode && ShowPager && CurrentPage > paginationContext.FirstVisiblePage && !Data.Any() )
            {
                await Paginate( ( CurrentPage - 1 ).ToString() );
            }

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Save the internal state of the editing items.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Save()
        {
            if ( Data == null )
                return;

            var editedCellValues = EditableColumns
                .Where( x => !string.IsNullOrEmpty( x.Field ) )
                .Select( c => new { c.Field, editItemCellValues[c.ElementId].CellValue } ).ToDictionary( x => x.Field, x => x.CellValue );

            var rowSavingHandler = editState == DataGridEditState.New ? RowInserting : RowUpdating;

            if ( await IsSafeToProceed( rowSavingHandler, editItem, editedCellValues ) )
            {
                if ( UseInternalEditing && editState == DataGridEditState.New && CanInsertNewItem && Data is ICollection<TItem> data )
                {
                    data.Add( editItem );
                }

                if ( UseInternalEditing || editState == DataGridEditState.New )
                {
                    // apply edited cell values to the item
                    // for new items it must be always be set, while for editing items it can be set only if it's enabled
                    foreach ( var column in EditableColumns )
                    {
                        column.SetValue( editItem, editItemCellValues[column.ElementId].CellValue );
                    }
                }

                if ( editState == DataGridEditState.New )
                {
                    await RowInserted.InvokeAsync( new( editItem, editedCellValues ) );
                    SetDirty();

                    // If a new item is added, the data should be refreshed
                    // to account for paging, sorting, and filtering
                    if ( ManualReadMode )
                        await HandleReadData( CancellationToken.None );
                }
                else
                    await RowUpdated.InvokeAsync( new( editItem, editedCellValues ) );

                editState = DataGridEditState.None;
                await VirtualizeOnEditCompleteScroll().AsTask();
            }

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Cancels the editing of DataGrid item.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Cancel()
        {
            editState = DataGridEditState.None;

            await VirtualizeOnEditCompleteScroll().AsTask();

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Selects the specified item.
        /// </summary>
        /// <param name="item">Item to select.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Select( TItem item )
        {
            await SelectRow( item );

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Sorts the Data for the specified column.
        /// Note that <see cref="DataGridColumn{TItem}.Sortable"/> must be enabled to be able to sort!
        /// </summary>
        /// <param name="fieldName">Field name of the column to sort.</param>
        /// <param name="sortDirection">Sort direction of the specified column, or if null it will be handled automatically.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Sort( string fieldName, SortDirection? sortDirection = null )
        {
            var column = Columns.FirstOrDefault( x => x.Field == fieldName );

            if ( column != null )
            {
                return Sort( column, sortDirection );
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sorts the Data for the specified column.
        /// Note that <see cref="DataGridColumn{TItem}.Sortable"/> must be enabled to be able to sort!
        /// </summary>
        /// <param name="column">Column to sort.</param>
        /// <param name="sortDirection">Sort direction of the specified column, or if null it will be handled automatically.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Sort( DataGridColumn<TItem> column, SortDirection? sortDirection = null )
        {
            if ( Sortable && column.Sortable )
            {
                HandleSortColumn( column, true, sortDirection );

                return Reload();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggers the DataGrid to change data source page.
        /// </summary>
        /// <remarks>
        /// Valid <paramref name="paginationCommandOrNumber"/> values are:
        /// 1-n:    Number of the page.
        /// prev:   Go to first page.
        /// next:   Go to next page.
        /// first:  Go to first page.
        /// last:   Go to last page.
        /// </remarks>
        /// <param name="paginationCommandOrNumber">Pagination command name or number(1 indexed) of the page.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Paginate( string paginationCommandOrNumber )
        {
            if ( int.TryParse( paginationCommandOrNumber, out var pageNumber ) )
            {
                CurrentPage = pageNumber;
            }
            else
            {
                if ( paginationCommandOrNumber == "prev" )
                {
                    CurrentPage--;

                    if ( CurrentPage < 1 )
                        CurrentPage = 1;
                }
                else if ( paginationCommandOrNumber == "next" )
                {
                    CurrentPage++;

                    if ( CurrentPage > paginationContext.LastPage )
                        CurrentPage = paginationContext.LastPage;
                }
                else if ( paginationCommandOrNumber == "first" )
                {
                    CurrentPage = 1;
                }
                else if ( paginationCommandOrNumber == "last" )
                {
                    CurrentPage = paginationContext.LastPage;
                }
            }

            return InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Clears all filters from the grid.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task ClearFilter()
        {
            foreach ( var column in Columns )
            {
                column.Filter.SearchValue = null;
            }

            return Reload();
        }

        /// <summary>
        /// Forces the internal DataGrid data to be filtered.
        /// </summary>
        /// <remarks>
        /// Keep in mind that this command will always trigger <see cref="FilteredDataChanged"/> even
        /// though not any data is actually changed.
        /// </remarks>
        public void FilterData()
        {
            FilterData( Data?.AsQueryable() );

            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Updates the cell of the current editing item that matches the <paramref name="fieldName"/>.
        /// </summary>
        /// <param name="fieldName">Cell field name.</param>
        /// <param name="value">New cell value.</param>
        public void UpdateCellEditValue( string fieldName, object value )
        {
            if ( editState == DataGridEditState.None )
                return;

            var column = Columns.FirstOrDefault( x => x.Field == fieldName );

            if ( column != null && editItemCellValues.TryGetValue( column.ElementId, out var cellEditContext ) )
            {
                cellEditContext.CellValue = value;
            }

            InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Reads the cell value of the current editing item that matches the <paramref name="fieldName"/>.
        /// </summary>
        /// <param name="fieldName">Cell field name.</param>
        /// <returns>Cell value.</returns>
        public object ReadCellEditValue( string fieldName )
        {
            if ( editState == DataGridEditState.None )
                return null;

            var column = Columns.FirstOrDefault( x => x.Field == fieldName );

            if ( column != null && editItemCellValues.TryGetValue( column.ElementId, out var cellEditContext ) )
            {
                return cellEditContext.CellValue;
            }

            return null;
        }

        /// <summary>
        /// Toggles DetailRow while evaluating the <see cref="DetailRowTrigger"/> if provided.
        /// Use <paramref name="forceDetailRow"/> to ignore <see cref="DetailRowTrigger"/> and toggle the DetailRow.
        /// </summary>
        /// <param name="item">Row item.</param>
        /// <param name="forceDetailRow">Ignores DetailRowTrigger and toggles the DetailRow.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task ToggleDetailRow( TItem item, bool forceDetailRow = false )
            => ToggleDetailRow( item, DetailRowTriggerType.Manual, forceDetailRow, true );

        protected internal Task ToggleDetailRow( TItem item, DetailRowTriggerType detailRowTriggerType, bool forceDetailRow = false, bool skipDetailRowTriggerType = false )
            => ToggleDetailRow( GetRowInfo( item ), detailRowTriggerType, forceDetailRow, skipDetailRowTriggerType );

        protected internal Task ToggleDetailRow( DataGridRowInfo<TItem> rowInfo, DetailRowTriggerType detailRowTriggerType, bool forceDetailRow = false, bool skipDetailRowTriggerType = false )
        {
            if ( rowInfo is not null )
            {
                if ( forceDetailRow )
                {
                    rowInfo.ToggleDetailRow();
                }
                else if ( DetailRowTrigger is not null )
                {
                    var detailRowTriggerContext = new DetailRowTriggerEventArgs<TItem>( rowInfo.Item );
                    var detailRowTriggerResult = DetailRowTrigger( detailRowTriggerContext );

                    if ( !skipDetailRowTriggerType && detailRowTriggerType != detailRowTriggerContext.DetailRowTriggerType )
                        return Task.CompletedTask;

                    rowInfo.SetRowDetail( detailRowTriggerResult, detailRowTriggerContext.Toggleable );

                    if ( rowInfo.HasDetailRow && detailRowTriggerContext.Single )
                    {
                        foreach ( var row in Rows.Where( x => !x.IsEqual( rowInfo ) ) )
                        {
                            row.SetRowDetail( false, false );
                        }
                    }
                }
                else
                {
                    rowInfo.ToggleDetailRow();
                }

                return InvokeAsync( StateHasChanged );
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// If <see cref="FixedHeader"/> or <see cref="Virtualize"/> is enabled, it will scroll position to the provided pixels.
        /// </summary>
        /// <param name="pixels">Offset in pixels from the top of the DataGrid.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public ValueTask ScrollToPixels( int pixels )
            => tableRef.ScrollToPixels( pixels );

        /// <summary>
        /// If <see cref="FixedHeader"/> or <see cref="Virtualize"/> is enabled, it will scroll position to the provided row.
        /// </summary>
        /// <param name="row">Zero-based index of DataGrid row to scroll to.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public ValueTask ScrollToRow( int row )
            => tableRef.ScrollToRow( row );

        #endregion

        #region Editing

        /// <summary>
        /// Create new empty instance of TItem.
        /// </summary>
        /// <returns>Return new instance of TItem.</returns>
        private TItem CreateNewItem()
            => newItemCreator.Value();

        /// <summary>
        /// Prepares edit item and it's cell values for editing.
        /// </summary>
        /// <param name="item">Item to set.</param>
        private void InitEditItem( TItem item )
        {
            editItem = item;
            editItemCellValues = new();

            validationItem = UseValidation
                ? ValidationItemCreator is null ? RecursiveObjectActivator.CreateInstance<TItem>() : ValidationItemCreator()
                : default;

            foreach ( var column in EditableColumns )
            {
                var cellValue = column.GetValue( editItem );
                editItemCellValues.Add( column.ElementId, new CellEditContext<TItem>( item, UpdateCellEditValue, ReadCellEditValue )
                {
                    CellValue = cellValue,
                } );

                if ( validationItem is not null )
                    column.SetValue( validationItem, cellValue );
            }
        }

        internal Task OnRowOverCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowOver.InvokeAsync( eventArgs );
        }
        internal Task OnRowLeaveCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowLeave.InvokeAsync( eventArgs );
        }

        internal Task OnRowClickedCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowClicked.InvokeAsync( eventArgs );
        }

        internal Task OnRowDoubleClickedCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowDoubleClicked.InvokeAsync( eventArgs );
        }

        internal Task OnRowContextMenuCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowContextMenu.InvokeAsync( eventArgs );
        }

        protected internal int ResolveItemIndex( TItem item )
        {
            short index = 0;
            foreach ( var displayItem in DisplayData )
            {
                if ( item.IsEqual( displayItem ) )
                    break;
                index++;
            }
            return index;
        }

        internal async Task OnMultiSelectCommand( MultiSelectEventArgs<TItem> eventArgs )
        {
            SelectedAllRows = false;
            UnSelectAllRows = false;

            SelectedRows ??= new();

            await HandleShiftClick( eventArgs );

            if ( eventArgs.Selected && !SelectedRows.Contains( eventArgs.Item ) && !eventArgs.ShiftKey )
            {
                SelectedRows.Add( eventArgs.Item );
            }
            else if ( !eventArgs.Selected && SelectedRows.Contains( eventArgs.Item ) && !eventArgs.ShiftKey )
            {
                if ( SelectedRows.Contains( eventArgs.Item ) )
                {
                    SelectedRows.Remove( eventArgs.Item );

                    if ( SelectedRow.IsEqual( eventArgs.Item ) )
                    {
                        await SelectedRowChanged.InvokeAsync( default( TItem ) );
                    }
                }
            }

            await SelectedRowsChanged.InvokeAsync( SelectedRows );

            await InvokeAsync( StateHasChanged );
        }

        private async Task HandleShiftClick( MultiSelectEventArgs<TItem> eventArgs )
        {
            if ( eventArgs.ShiftKey )
            {
                SelectedRows.Clear();

                var currIndex = ResolveItemIndex( eventArgs.Item );

                if ( currIndex >= lastSelectedRowIndex )
                {
                    foreach ( var item in DisplayData.Skip( lastSelectedRowIndex ).Take( currIndex - lastSelectedRowIndex + 1 ) )
                    {
                        SelectedRows.Add( item );
                    }
                }
                else
                {
                    foreach ( var item in DisplayData.Skip( currIndex ).Take( lastSelectedRowIndex - currIndex + 1 ) )
                    {
                        SelectedRows.Add( item );
                    }
                }

                if ( !SelectedRows.Contains( SelectedRow ) )
                {
                    await SelectedRowChanged.InvokeAsync( default( TItem ) );
                }
            }
            else
                lastSelectedRowIndex = ResolveItemIndex( eventArgs.Item );
        }

        protected internal async Task OnMultiSelectAll( bool selectAll )
        {
            SelectedRows ??= new();

            if ( selectAll )
            {
                SelectedRows.Clear();

                if ( RowSelectable is not null )
                {
                    foreach ( var item in DisplayData )
                    {
                        if ( RowSelectable.Invoke( new( item, DataGridSelectReason.MultiSelectAll ) ) )
                        {
                            SelectedRows.Add( item );
                        }
                    }
                }
                else
                {
                    SelectedRows.AddRange( DisplayData );
                }
            }
            else
            {
                SelectedRows.Clear();

                await SelectedRowChanged.InvokeAsync( default( TItem ) );
            }

            SelectedAllRows = selectAll;
            UnSelectAllRows = !selectAll;

            await SelectedRowsChanged.InvokeAsync( SelectedRows );

            await InvokeAsync( StateHasChanged );
        }

        // this is to give user a way to stop save if necessary
        internal async Task<bool> IsSafeToProceed<TValues>( EventCallback<CancellableRowChange<TItem, TValues>> handler, TItem item, TValues editedCellValues )
        {
            if ( handler.HasDelegate )
            {
                var args = new CancellableRowChange<TItem, TValues>( item, editedCellValues );

                await handler.InvokeAsync( args );

                if ( args.Cancel )
                {
                    return false;
                }
            }

            return true;
        }

        internal async Task<bool> IsSafeToProceed( EventCallback<CancellableRowChange<TItem>> handler, TItem item )
        {
            if ( handler.HasDelegate )
            {
                var args = new CancellableRowChange<TItem>( item );

                await handler.InvokeAsync( args );

                if ( args.Cancel )
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Filtering

        private void SetDirty()
        {
            dirtyFilter = dirtyView = true;
        }

        /// <summary>
        /// Triggers the reload of the <see cref="DataGrid{TItem}"/> data.
        /// Makes sure not to reload if the DataGrid is in a loading state.
        /// </summary>
        /// <returns>Returns the awaitable task.</returns>
        public async Task Reload( CancellationToken cancellationToken = default )
        {
            if ( IsLoading )
                return;

            SetDirty();

            if ( ManualReadMode )
            {
                await InvokeAsync( () => HandleReadData( cancellationToken ) );
            }
            else if ( VirtualizeManualReadMode )
            {
                if ( virtualizeFilterChanged )
                {
                    virtualizeFilterChanged = false;
                    await VirtualizeScrollToTop();
                }

                if ( virtualizeRef is null )
                    await InvokeAsync( () => HandleVirtualizeReadData( 0, PageSize, cancellationToken ) );
                else
                    await virtualizeRef.RefreshDataAsync();
                await InvokeAsync( StateHasChanged );
            }
            else
            {
                await InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Notifies the <see cref="DataGrid{TItem}"/> to refresh.
        /// </summary>
        /// <returns></returns>
        public async virtual Task Refresh()
            => await InvokeAsync( StateHasChanged );

        protected async Task HandleReadData( CancellationToken cancellationToken )
        {
            try
            {
                IsLoading = true;

                if ( !cancellationToken.IsCancellationRequested )
                    await ReadData.InvokeAsync( new DataGridReadDataEventArgs<TItem>( DataGridReadDataMode.Paging, Columns, SortByColumns, CurrentPage, PageSize, 0, 0, cancellationToken ) );
            }
            finally
            {
                IsLoading = false;

                await InvokeAsync( StateHasChanged );
            }
        }

        protected async Task HandleVirtualizeReadData( int startIdx, int count, CancellationToken cancellationToken )
        {
            try
            {
                IsLoading = true;

                if ( !cancellationToken.IsCancellationRequested )
                    await ReadData.InvokeAsync( new DataGridReadDataEventArgs<TItem>( DataGridReadDataMode.Virtualize, Columns, SortByColumns, 0, 0, virtualizeOffset: startIdx, virtualizeCount: count, cancellationToken ) );
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected async ValueTask<ItemsProviderResult<TItem>> VirtualizeItemsProviderHandler( ItemsProviderRequest request )
        {
            var requestCount = request.StartIndex > 0
                ? Math.Min( request.Count, TotalItems.Value - request.StartIndex )
                : request.Count;

            await HandleVirtualizeReadData( request.StartIndex, requestCount, request.CancellationToken );
            await Task.Yield(); // This line makes sure SetParametersAsync catches up, since we depend upon Data Parameter.

            if ( request.CancellationToken.IsCancellationRequested )
                return new();
            else
                return new( Data.ToList(), TotalItems.Value );
        }

        protected void HandleSortColumn( DataGridColumn<TItem> column, bool changeSortDirection, SortDirection? sortDirection = null )
        {
            if ( Sortable && column.CanSort() )
            {
                if ( SortMode == DataGridSortMode.Single )
                {
                    // in single-mode we need to reset all other columns to default state
                    foreach ( var c in Columns.Where( x => x.GetFieldToSort() != column.GetFieldToSort() ) )
                    {
                        c.CurrentSortDirection = SortDirection.Default;
                    }

                    // and also remove any column sort info except for current one
                    SortByColumns.RemoveAll( x => x.GetFieldToSort() != column.GetFieldToSort() );
                }

                if ( changeSortDirection )
                    column.CurrentSortDirection = sortDirection ?? column.CurrentSortDirection.NextDirection();

                if ( !SortByColumns.Any( c => c.GetFieldToSort() == column.GetFieldToSort() ) )
                {
                    var nextOrderToSort = SortByColumns.Count == 0 ? 0 : SortByColumns.Max( x => x.SortOrder ) + 1;
                    column.SetSortOrder( nextOrderToSort );
                    SortByColumns.Add( column );
                }
                else if ( column.CurrentSortDirection == SortDirection.Default )
                {
                    SortByColumns.Remove( column );
                    column.ResetSortOrder();
                }

                if ( changeSortDirection )
                    InvokeAsync( () => SortChanged.InvokeAsync( new DataGridSortChangedEventArgs( column.GetFieldToSort(), column.CurrentSortDirection ) ) );
            }
        }

        protected Task OnClearFilterCommand()
        {
            foreach ( var column in Columns )
            {
                column.Filter.SearchValue = null;
            }
            return Reload();
        }

        protected Task OnPaginationItemClick( string pageName )
        {
            if ( int.TryParse( pageName, out var pageNumber ) )
            {
                CurrentPage = pageNumber;
            }
            else
            {
                if ( pageName == "prev" )
                {
                    CurrentPage--;

                    if ( CurrentPage < 1 )
                        CurrentPage = 1;
                }
                else if ( pageName == "next" )
                {
                    CurrentPage++;

                    if ( CurrentPage > paginationContext.LastPage )
                        CurrentPage = paginationContext.LastPage;
                }
                else if ( pageName == "first" )
                {
                    CurrentPage = 1;
                }
                else if ( pageName == "last" )
                {
                    CurrentPage = paginationContext.LastPage;
                }
            }

            return Task.CompletedTask;
        }

        private void FilterData( IQueryable<TItem> query )
        {
            dirtyFilter = false;

            if ( query == null )
            {
                filteredData.Clear();
                FilteredDataChanged?.Invoke( new( filteredData, 0, 0 ) );

                return;
            }

            // only use internal filtering if we're not using custom data loading
            if ( !ManualReadMode )
            {
                var firstSort = true;

                foreach ( var sortByColumn in SortByColumns.OrderBy( x => x.SortOrder ) )
                {
                    Func<TItem, object> sortFunction = sortByColumn.GetValueForSort;

                    if ( firstSort )
                    {
                        if ( sortByColumn.CurrentSortDirection == SortDirection.Ascending )
                            query = query.OrderBy( x => sortFunction( x ) );
                        else
                            query = query.OrderByDescending( x => sortFunction( x ) );

                        firstSort = false;
                    }
                    else
                    {
                        if ( sortByColumn.CurrentSortDirection == SortDirection.Ascending )
                            query = ( query as IOrderedQueryable<TItem> ).ThenBy( x => sortFunction( x ) );
                        else
                            query = ( query as IOrderedQueryable<TItem> ).ThenByDescending( x => sortFunction( x ) );
                    }
                }

                if ( CustomFilter != null )
                {
                    query = from item in query
                            where item != null
                            where CustomFilter( item )
                            select item;
                }

                foreach ( var column in Columns )
                {
                    if ( column.ExcludeFromFilter )
                        continue;

                    if ( column.CustomFilter != null )
                    {
                        query = from item in query
                                let cellRealValue = column.GetValue( item )
                                where column.CustomFilter( cellRealValue, column.Filter.SearchValue )
                                select item;
                    }
                    else
                    {
                        var stringSearchValue = column.Filter.SearchValue?.ToString();

                        if ( string.IsNullOrEmpty( stringSearchValue ) )
                            continue;

                        query = from item in query
                                let cellRealValue = column.GetValue( item )
                                let cellStringValue = cellRealValue == null ? string.Empty : cellRealValue.ToString()
                                where CompareFilterValues( cellStringValue, stringSearchValue )
                                select item;
                    }
                }
            }

            filteredData = query.ToList();

            FilteredDataChanged?.Invoke( new(
                filteredData,
                filteredData.Count,
                ( ManualReadMode ? TotalItems : Data?.Count() ) ?? 0 ) );
        }

        protected internal Task OnFilterChanged( DataGridColumn<TItem> column, object value )
        {
            virtualizeFilterChanged = true;
            column.Filter.SearchValue = value;
            return Reload();
        }

        private bool CompareFilterValues( string searchValue, string compareTo )
        {
            switch ( FilterMethod )
            {
                case DataGridFilterMethod.StartsWith:
                    return searchValue.StartsWith( compareTo, StringComparison.OrdinalIgnoreCase );
                case DataGridFilterMethod.EndsWith:
                    return searchValue.EndsWith( compareTo, StringComparison.OrdinalIgnoreCase );
                case DataGridFilterMethod.Equals:
                    return searchValue.Equals( compareTo, StringComparison.OrdinalIgnoreCase );
                case DataGridFilterMethod.NotEquals:
                    return !searchValue.Equals( compareTo, StringComparison.OrdinalIgnoreCase );
                case DataGridFilterMethod.Contains:
                default:
                    return searchValue.IndexOf( compareTo, StringComparison.OrdinalIgnoreCase ) >= 0;
            }
        }

        private IEnumerable<TItem> FilterViewData()
        {
            if ( dirtyFilter )
                FilterData();

            // only use pagination if the custom data loading is not used
            if ( !ManualReadMode && !Virtualize )
            {
                var skipElements = ( CurrentPage - 1 ) * PageSize;
                if ( skipElements > filteredData.Count )
                {
                    CurrentPage = paginationContext.LastPage;
                    skipElements = ( CurrentPage - 1 ) * PageSize;
                }
                return filteredData.Skip( skipElements ).Take( PageSize );
            }

            return filteredData;
        }

        private Task SelectRow( TItem item )
        {
            if ( editState != DataGridEditState.None )
                return Task.CompletedTask;

            SelectedRow = item;

            return SelectedRowChanged.InvokeAsync( SelectedRow );
        }

        private DataGridRowInfo<TItem> GetRowInfo( TItem item )
            => Rows.LastOrDefault( x => x.Item.IsEqual( item ) );

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Template for mouse hover overlay display formatting.
        /// </summary>
        [Parameter] public RenderFragment<TItem> RowOverlayTemplate { get; set; }

        [Parameter] public RowOverlayPosition RowOverlayPosition { get; set; } = RowOverlayPosition.End;


        /// <summary>
        /// Cascaded theme settings.
        /// </summary>
        [CascadingParameter] public Theme Theme { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IJSUtilitiesModule"/> instance.
        /// </summary>
        [Inject] public IJSUtilitiesModule JSUtilitiesModule { get; set; }

        internal bool IsFixedHeader
            => Virtualize || FixedHeader;

        /// <summary>
        /// Gets the DataGrid standard class and other existing Class
        /// </summary>
        protected string ClassNames
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append( "b-datagrid" );

                if ( Class != null )
                    sb.Append( $" {Class}" );

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the data to show on grid based on the filter and current page.
        /// </summary>
        protected List<DataGridRowInfo<TItem>> Rows { get; } = new();

        /// <summary>
        /// List of all the columns associated with this datagrid.
        /// </summary>
        protected List<DataGridColumn<TItem>> Columns { get; } = new();

        /// <summary>
        /// List of all the aggregate columns associated with this datagrid.
        /// </summary>
        protected List<DataGridAggregate<TItem>> Aggregates { get; } = new();

        /// <summary>
        /// Gets only columns that are available for editing.
        /// </summary>
        protected IEnumerable<DataGridColumn<TItem>> EditableColumns => Columns.Where( x => !x.ExcludeFromEdit && x.Editable );

        /// <summary>
        /// Gets only columns that are available for display in the grid.
        /// </summary>
        internal IEnumerable<DataGridColumn<TItem>> DisplayableColumns
        {
            get
            {
                return Columns
                    .Where( x => x.IsDisplayable || x.Displayable )
                    .OrderBy( x => x.DisplayOrder );
            }
        }

        /// <summary>
        /// Returns true if <see cref="Data"/> is safe to modify.
        /// </summary>
        protected bool CanInsertNewItem => Editable && Data is ICollection<TItem>;

        /// <summary>
        /// Returns true if any aggregate is defines on columns.
        /// </summary>
        protected bool HasAggregates => Aggregates.Count > 0;

        /// <summary>
        /// If true, aggregates will be shown on top of the table.
        /// </summary>
        protected bool ShowAggregatesOnTop => AggregateRowPosition == DataGridAggregateRowPosition.Top || AggregateRowPosition == DataGridAggregateRowPosition.TopAndBottom;

        /// <summary>
        /// If true, aggregates will be shown on bottom of the table.
        /// </summary>
        protected bool ShowAggregatesOnBottom => AggregateRowPosition == DataGridAggregateRowPosition.Bottom || AggregateRowPosition == DataGridAggregateRowPosition.TopAndBottom;

        /// <summary>
        /// Returns true if data is not empty, data is not loaded, empty and loading template is not set.
        /// </summary>
        protected bool IsDisplayDataVisible => !IsLoadingTemplateVisible && !IsEmptyTemplateVisible;

        /// <summary>
        /// Returns true if LoadingTemplate is set and IsLoading is true.
        /// </summary>
        protected bool IsLoadingTemplateVisible => !IsNewItemInGrid && LoadingTemplate != null && IsLoading && !Virtualize;

        /// <summary>
        /// Returns true if ReadData will be invoked.
        /// </summary>
        protected bool IsLoading { get; set; }

        /// <summary>
        /// Returns true if EmptyTemplate is set and Data is null or empty.
        /// </summary>
        protected bool IsEmptyTemplateVisible
            => !IsLoading && !IsNewItemInGrid && EmptyTemplate != null && Data.IsNullOrEmpty() && Rendered;

        /// <summary>
        /// Returns true if EmptyFilterTemplate is set and FilteredData is null or empty.
        /// </summary>
        protected bool IsEmptyFilterTemplateVisible
            => !IsLoading && !IsNewItemInGrid && EmptyFilterTemplate != null && ( !data.IsNullOrEmpty() && FilteredData.IsNullOrEmpty() ) && Rendered;

        /// <summary>
        /// Returns true if ShowPager is true and grid is not empty or loading.
        /// </summary>
        protected bool IsPagerVisible
            => ShowPager && !IsLoadingTemplateVisible && ( ( IsButtonRowVisible && ButtonRowTemplate != null ) || !IsEmptyTemplateVisible );

        /// <summary>
        /// Returns true if current state is for new item and editing fields are shown on datagrid.
        /// </summary>
        protected bool IsNewItemInGrid
            => Editable && editState == DataGridEditState.New && EditMode != DataGridEditMode.Popup;

        /// <summary>
        /// Returns true if the datagrid is in edit mode and the item is the currently selected edititem
        /// </summary>
        protected bool IsEditItemInGrid( TItem item ) => Editable && editState == DataGridEditState.Edit && EditMode != DataGridEditMode.Popup && item.IsEqual( editItem );

        /// <summary>
        /// True if user is using <see cref="ReadData"/> for loading the data.
        /// </summary>
        public bool ManualReadMode => ReadData.HasDelegate && !Virtualize;

        public bool VirtualizeManualReadMode => ReadData.HasDelegate && Virtualize;

        /// <summary>
        /// Gets the current datagrid editing state.
        /// </summary>
        public DataGridEditState EditState => editState;

        /// <summary>
        /// Gets the sort column info for current SortMode.
        /// </summary>
        protected List<DataGridColumn<TItem>> SortByColumns => sortByColumnsDictionary[SortMode];

        /// <summary>
        /// True if button row should be rendered.
        /// </summary>
        public bool IsButtonRowVisible => CommandMode is DataGridCommandMode.Default or DataGridCommandMode.ButtonRow;

        /// <summary>
        /// True if command buttons should be rendered.
        /// </summary>
        public bool IsCommandVisible => Editable && CommandMode is DataGridCommandMode.Default or DataGridCommandMode.Commands;

        /// <summary>
        /// Trigger to unselect all rows.
        /// Set it back to false.
        /// </summary>
        internal bool UnSelectAllRows { get; set; }

        /// <summary>
        /// Trigger to select all rows.
        /// </summary>
        internal bool SelectedAllRows { get; set; }

        /// <summary>
        /// Checks if the DataGrid is currently on single selection mode.
        /// </summary>
        internal bool SingleSelect
            => ( SelectionMode == DataGridSelectionMode.Single );

        /// <summary>
        /// Checks if the DataGrid is currently on multiple selection mode.
        /// </summary>
        internal bool MultiSelect
            => ( SelectionMode == DataGridSelectionMode.Multiple );

        /// <summary>
        /// Tracks whether the current client is a Macintosh Operating System.
        /// </summary>
        internal bool IsClientMacintoshOS { get; private set; }

        /// <summary>
        /// Gets template for title of popup modal.
        /// </summary>
        [Parameter]
        public RenderFragment<PopupTitleContext<TItem>> PopupTitleTemplate { get; set; } = context =>
        {
            return builder =>
            {
                builder.AddContent( 0, context.EditState == DataGridEditState.Edit ? "Row Edit" : "Row Create" );
            };
        };

        /// <summary>
        /// Gets the flag which indicates if popup editor is visible.
        /// </summary>
        protected bool PopupVisible => EditMode == DataGridEditMode.Popup && EditState != DataGridEditState.None;

        /// <summary>
        /// Defines the size of popup dialog.
        /// </summary>
        [Parameter] public ModalSize PopupSize { get; set; } = ModalSize.Default;

        /// <summary>
        /// Occurs before the popup dialog is closed.
        /// </summary>
        [Parameter] public Func<ModalClosingEventArgs, Task> PopupClosing { get; set; }

        /// <summary>
        /// Gets the reference to the associated command column.
        /// </summary>
        public DataGridCommandColumn<TItem> CommandColumn { get; private set; }

        /// <summary>
        /// Gets the reference to the associated multiselect column.
        /// </summary>
        public DataGridMultiSelectColumn<TItem> MultiSelectColumn { get; private set; }

        /// <summary>
        /// Checks if the MultiSelectAll is checked, meaning that all of the current view rows are selected.
        /// </summary>
        private bool IsMultiSelectAllChecked
            => ( !SelectedRows.IsNullOrEmpty() )
                && DisplayData.Any()
                && !DisplayData.Except( SelectedRows ).Any();

        /// <summary>
        /// Checks if the MultiSelectAll is indeterminate, meaning that only some of the current view rows are selected.
        /// </summary>
        private bool IsMultiSelectAllIndeterminate
        {
            get
            {
                var hasSelectedRows = SelectedRows?.Any() ?? false;

                if ( hasSelectedRows )
                {
                    var unselectedRows = DisplayData.Except( SelectedRows ).Count();

                    return MultiSelect && hasSelectedRows && unselectedRows > 0 && unselectedRows < DisplayData.Count();
                }

                return false;
            }
        }

        /// <summary>
        /// Gets true if <see cref="ShowValidationsSummary"/> is enabled, and there are validation error messages <seealso cref="ValidationsSummaryErrors"/>.
        /// </summary>
        internal bool HasValidationsSummary => ShowValidationsSummary && ValidationsSummaryErrors?.Length > 0;

        /// <summary>
        /// Gets or sets the datagrid data-source.
        /// </summary>
        [Parameter]
        public IEnumerable<TItem> Data
        {
            get { return data; }
            set
            {
                SetDirty();
                data = value;
            }
        }

        /// <summary>
        /// Gets or sets the calculated aggregate data.
        /// </summary>
        /// <remarks>
        /// Used only in manual read mode along with the <see cref="ReadData"/> handler.
        /// </remarks>
        [Parameter]
        public IEnumerable<TItem> AggregateData
        {
            get { return aggregateData; }
            set { aggregateData = value; }
        }

        /// <summary>
        /// Gets or sets the total number of items. Used only when <see cref="ReadData"/> is used to load the data.
        /// </summary>
        /// <remarks>
        /// This field must be set only when <see cref="ReadData"/> is used to load the data.
        /// </remarks>
        [Parameter] public int? TotalItems { get => paginationContext.TotalItems; set => paginationContext.TotalItems = value; }

        /// <summary>
        /// Gets the data after all of the filters have being applied.
        /// </summary>
        public IEnumerable<TItem> FilteredData
        {
            get
            {
                if ( dirtyFilter )
                    FilterData();

                return filteredData;
            }
        }

        /// <summary>
        /// Raises an event every time that filtered data is refreshed.
        /// </summary>
        [Parameter] public Action<DataGridFilteredDataEventArgs<TItem>> FilteredDataChanged { get; set; }

        /// <summary>
        /// Gets the data to show on grid based on the filter and current page.
        /// </summary>
        public IEnumerable<TItem> DisplayData
        {
            get
            {
                if ( dirtyView )
                    viewData = FilterViewData();

                dirtyView = false;

                return viewData ?? Enumerable.Empty<TItem>();
            }
        }

        /// <summary>
        /// Specifies the behaviour of datagrid editing.
        /// </summary>
        /// <remarks>
        /// Disabling this option will send all changes to the RowInserted and RowUpdated but nothing will be saved unless the user manually update the item values.
        /// </remarks>
        [Parameter] public bool UseInternalEditing { get; set; } = true;

        /// <summary>
        /// Gets or sets whether users can edit datagrid rows.
        /// </summary>
        [Parameter] public bool Editable { get; set; }

        /// <summary>
        /// Gets or sets whether the datagrid will use the Virtualize functionality.
        /// </summary>
        [Parameter] public bool Virtualize { get; set; }

        /// <summary>
        /// Gets or sets Virtualize options when using the Virtualize functionality.
        /// </summary>
        [Parameter] public VirtualizeOptions VirtualizeOptions { get; set; }

        /// <summary>
        /// Gets or sets Pager options.
        /// </summary>
        [Parameter] public DataGridPagerOptions PagerOptions { get; set; }

        /// <summary>
        /// Gets or sets whether users can resize datagrid columns.
        /// </summary>
        [Parameter] public bool Resizable { get; set; }

        /// <summary>
        /// Gets or sets whether the user can resize on header or columns.
        /// </summary>
        [Parameter] public TableResizeMode ResizeMode { get; set; }

        /// <summary>
        /// Gets or sets whether end-users can sort data by the column's values.
        /// </summary>
        [Parameter] public bool Sortable { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the user can sort only by one column or by multiple.
        /// </summary>
        [Parameter] public DataGridSortMode SortMode { get; set; } = DataGridSortMode.Multiple;

        /// <summary>
        /// Gets or sets whether users can filter rows by its cell values.
        /// </summary>
        [Parameter] public bool Filterable { get; set; }

        /// <summary>
        /// Gets or sets whether user can see a column captions.
        /// </summary>
        [Parameter] public bool ShowCaptions { get; set; } = true;

        /// <summary>
        /// Gets or sets whether users can navigate datagrid by using pagination controls.
        /// </summary>
        [Parameter] public bool ShowPager { get; set; }

        /// <summary>
        /// Gets or sets the position of the pager.
        /// </summary>
        [Parameter] public DataGridPagerPosition PagerPosition { get; set; } = DataGridPagerPosition.Bottom;

        /// <summary>
        /// Gets or sets the position of the aggregate row.
        /// </summary>
        [Parameter] public DataGridAggregateRowPosition AggregateRowPosition { get; set; } = DataGridAggregateRowPosition.Bottom;

        /// <summary>
        /// Gets or sets whether users can adjust the page size of the datagrid.
        /// </summary>
        [Parameter] public bool ShowPageSizes { get => paginationContext.ShowPageSizes; set => paginationContext.ShowPageSizes = value; }

        /// <summary>
        /// Gets or sets the chooseable page sizes of the datagrid.
        /// </summary>
        [Parameter] public IEnumerable<int> PageSizes { get => paginationContext.PageSizes; set => paginationContext.PageSizes = value; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        [Parameter] public int CurrentPage { get => paginationContext.CurrentPage; set => paginationContext.CurrentPage = value; }

        protected PaginationContext<TItem> PaginationContext => paginationContext;

        protected PaginationTemplates<TItem> PaginationTemplates => paginationTemplates;

        /// <summary>
        /// Gets or sets content of table body for empty DisplayData.
        /// </summary>
        [Parameter] public RenderFragment EmptyTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of table body for the empty filter DisplayData.
        /// </summary>
        [Parameter] public RenderFragment EmptyFilterTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of cell body for empty DisplayData.
        /// </summary>
        [Parameter] public RenderFragment<TItem> EmptyCellTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of table body for handle ReadData.
        /// </summary>
        [Parameter] public RenderFragment LoadingTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of button row of pager.
        /// </summary>
        [Parameter] public RenderFragment<ButtonRowContext<TItem>> ButtonRowTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of first button of pager.
        /// </summary>
        [Parameter] public RenderFragment FirstPageButtonTemplate { get => paginationTemplates.FirstPageButtonTemplate; set => paginationTemplates.FirstPageButtonTemplate = value; }

        /// <summary>
        /// Gets or sets content of last button of pager.
        /// </summary>
        [Parameter] public RenderFragment LastPageButtonTemplate { get => paginationTemplates.LastPageButtonTemplate; set => paginationTemplates.LastPageButtonTemplate = value; }

        /// <summary>
        /// Gets or sets content of previous button of pager.
        /// </summary>
        [Parameter] public RenderFragment PreviousPageButtonTemplate { get => paginationTemplates.PreviousPageButtonTemplate; set => paginationTemplates.PreviousPageButtonTemplate = value; }

        /// <summary>
        /// Gets or sets content of next button of pager.
        /// </summary>
        [Parameter] public RenderFragment NextPageButtonTemplate { get => paginationTemplates.NextPageButtonTemplate; set => paginationTemplates.NextPageButtonTemplate = value; }

        /// <summary>
        /// Gets or sets content of page buttons of pager.
        /// </summary>
        [Parameter] public RenderFragment<PageButtonContext> PageButtonTemplate { get => paginationTemplates.PageButtonTemplate; set => paginationTemplates.PageButtonTemplate = value; }

        /// <summary>
        /// Gets or sets content of items per page of grid.
        /// </summary>
        [Parameter] public RenderFragment ItemsPerPageTemplate { get => paginationTemplates.ItemsPerPageTemplate; set => paginationTemplates.ItemsPerPageTemplate = value; }

        /// <summary>
        /// Gets or sets content of total items grid for small devices.
        /// </summary>
        [Parameter] public RenderFragment<PaginationContext<TItem>> TotalItemsShortTemplate { get => paginationTemplates.TotalItemsShortTemplate; set => paginationTemplates.TotalItemsShortTemplate = value; }

        /// <summary>
        /// Gets or sets content of total items grid.
        /// </summary>
        [Parameter] public RenderFragment<PaginationContext<TItem>> TotalItemsTemplate { get => paginationTemplates.TotalItemsTemplate; set => paginationTemplates.TotalItemsTemplate = value; }

        /// <summary>
        /// Gets or sets the maximum number of items for each page.
        /// </summary>
        [Parameter] public int PageSize { get => paginationContext.CurrentPageSize; set => paginationContext.CurrentPageSize = value; }

        /// <summary>
        /// Occurs after the <see cref="PageSize"/> has changed.
        /// </summary>
        [Parameter] public EventCallback<int> PageSizeChanged { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of visible pagination links. It has to be odd for well look.
        /// </summary>
        [Parameter] public int MaxPaginationLinks { get => paginationContext.MaxPaginationLinks; set => paginationContext.MaxPaginationLinks = value; }

        /// <summary>
        /// Defines the filter method when searching the cell values.
        /// </summary>
        [Parameter] public DataGridFilterMethod FilterMethod { get; set; } = DataGridFilterMethod.Contains;

        /// <summary>
        /// Gets or sets currently selected row.
        /// </summary>
        [Parameter] public TItem SelectedRow { get; set; }

        /// <summary>
        /// Gets or sets currently selected rows.
        /// </summary>
        [Parameter] public List<TItem> SelectedRows { get; set; }

        /// <summary>
        /// Gets or sets current selection mode.
        /// </summary>
        [Parameter] public DataGridSelectionMode SelectionMode { get { return selectionMode; } set { selectionMode = value; ExecuteAfterRender( HandleSelectionModeChanged ); } }

        /// <summary>
        /// Occurs after the selected row has changed.
        /// </summary>
        [Parameter] public EventCallback<TItem> SelectedRowChanged { get; set; }

        /// <summary>
        /// Occurs after multi selection has changed.
        /// </summary>
        [Parameter] public EventCallback<List<TItem>> SelectedRowsChanged { get; set; }

        /// <summary>
        /// Cancelable event called before the row is inserted.
        /// </summary>
        [Parameter] public EventCallback<CancellableRowChange<TItem, Dictionary<string, object>>> RowInserting { get; set; }

        /// <summary>
        /// Cancelable event called before the row is updated.
        /// </summary>
        [Parameter] public EventCallback<CancellableRowChange<TItem, Dictionary<string, object>>> RowUpdating { get; set; }

        /// <summary>
        /// Cancelable event called before the row is removed.
        /// </summary>
        [Parameter] public EventCallback<CancellableRowChange<TItem>> RowRemoving { get; set; }

        /// <summary>
        /// Event called after the row is inserted.
        /// </summary>
        [Parameter] public EventCallback<SavedRowItem<TItem, Dictionary<string, object>>> RowInserted { get; set; }

        /// <summary>
        /// Event called after the row is updated.
        /// </summary>
        [Parameter] public EventCallback<SavedRowItem<TItem, Dictionary<string, object>>> RowUpdated { get; set; }

        /// <summary>
        /// Event called after the row is removed.
        /// </summary>
        [Parameter] public EventCallback<TItem> RowRemoved { get; set; }


        /// <summary>
        /// Event called after the mouse leaves the row.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowLeave { get; set; }

        /// <summary>
        /// Event called after the mouse is over the row.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowOver { get; set; }
        /// <summary>
        /// Event called after the row is clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowClicked { get; set; }

        /// <summary>
        /// Event called after the row is double clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowDoubleClicked { get; set; }

        /// <summary>
        /// Event called after the row has requested a context menu.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowContextMenu { get; set; }

        /// <summary>
        /// Used to prevent the default action for an <see cref="RowContextMenu"/> event.
        /// </summary>
        [Parameter] public bool RowContextMenuPreventDefault { get; set; }

        /// <summary>
        /// Occurs after the selected page has changed.
        /// </summary>
        [Parameter] public EventCallback<DataGridPageChangedEventArgs> PageChanged { get; set; }

        /// <summary>
        /// Event handler used to load data manually based on the current page and filter data settings.
        /// </summary>
        [Parameter] public EventCallback<DataGridReadDataEventArgs<TItem>> ReadData { get; set; }

        /// <summary>
        /// Occurs after the column sort direction has changed.
        /// </summary>
        [Parameter] public EventCallback<DataGridSortChangedEventArgs> SortChanged { get; set; }

        /// <summary>
        /// Specifies the grid editing modes.
        /// </summary>
        [Parameter] public DataGridEditMode EditMode { get; set; } = DataGridEditMode.Form;

        /// <summary>
        /// Specifies the grid command mode.
        /// </summary>
        [Parameter] public DataGridCommandMode CommandMode { get; set; }

        /// <summary>
        /// A trigger function used to handle the visibility of detail row.
        /// </summary>
        [Parameter] public Func<DetailRowTriggerEventArgs<TItem>, bool> DetailRowTrigger { get; set; }

        /// <summary>
        /// Handles the selection of the DataGrid row.
        /// If not set it will default to always true.
        /// </summary>
        [Parameter] public Func<RowSelectableEventArgs<TItem>, bool> RowSelectable { get; set; }

        /// <summary>
        /// Handles the selection of the cursor for a hovered row.
        /// If not set, <see cref="Cursor.Default"/> will be used.
        /// </summary>
        [Parameter] public Func<TItem, Cursor> RowHoverCursor { get; set; }

        /// <summary>
        /// Template for displaying detail or nested row.
        /// </summary>
        [Parameter] public RenderFragment<TItem> DetailRowTemplate { get; set; }

        /// <summary>
        /// Function, that is called, when a new item is created for inserting new entry.
        /// </summary>
        [Parameter] public Action<TItem> NewItemDefaultSetter { get; set; }

        /// <summary>
        /// Function that, if set, is called to create new instance of an item. If left null a default constructor will be used.
        /// </summary>
        [Parameter] public Func<TItem> NewItemCreator { get; set; }

        /// <summary>
        /// Function that, if set, is called to create a validation instance of an item that it's used as a separate instance for Datagrid's internal processing of validation. If left null, Datagrid will try to use it's own implementation to instantiate.
        /// </summary>
        [Parameter] public Func<TItem> ValidationItemCreator { get; set; }

        /// <summary>
        /// Function that, if set, is called to create a instance of the selected item to edit. If left null the selected item will be used.
        /// </summary>
        [Parameter] public Func<TItem, TItem> EditItemCreator { get; set; }

        /// <summary>
        /// Adds stripes to the table.
        /// </summary>
        [Parameter] public bool Striped { get; set; }

        /// <summary>
        /// Adds borders to all the cells.
        /// </summary>
        [Parameter] public bool Bordered { get; set; }

        /// <summary>
        /// Makes the table without any borders.
        /// </summary>
        [Parameter] public bool Borderless { get; set; }

        /// <summary>
        /// Adds a hover effect when mousing over rows.
        /// </summary>
        [Parameter] public bool Hoverable { get; set; }

        /// <summary>
        /// Makes the table more compact by cutting cell padding in half.
        /// </summary>
        [Parameter] public bool Narrow { get; set; }

        /// <summary>
        /// Makes table responsive by adding the horizontal scroll bar.
        /// </summary>
        /// <remarks>
        /// In some cases <see cref="Dropdown"/> component placed inside of a table marked with <see cref="Responsive"/>
        /// flag might not show dropdown menu properly. To make it work you might need to add some
        /// <see href="https://stackoverflow.com/questions/49346755/bootstrap-4-drop-down-menu-in-table">additional CSS rules</see>.
        /// </remarks>
        [Parameter] public bool Responsive { get; set; }

        /// <summary>
        /// Custom css classname.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom html style.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Defines the element margin spacing.
        /// </summary>
        [Parameter] public IFluentSpacing Margin { get; set; }

        /// <summary>
        /// Defines the element padding spacing.
        /// </summary>
        [Parameter] public IFluentSpacing Padding { get; set; }

        /// <summary>
        /// Custom handler for each row in the datagrid.
        /// </summary>
        [Parameter] public Action<TItem, DataGridRowStyling> RowStyling { get; set; }

        /// <summary>
        /// Custom handler for currently selected row.
        /// </summary>
        [Parameter] public Action<TItem, DataGridRowStyling> SelectedRowStyling { get; set; }

        /// <summary>
        /// Handler for custom filtering on datagrid item.
        /// </summary>
        [Parameter] public DataGridCustomFilter<TItem> CustomFilter { get; set; }

        /// <summary>
        /// Custom styles for header row.
        /// </summary>
        [Parameter] public DataGridRowStyling HeaderRowStyling { get; set; }

        /// <summary>
        /// Custom styles for filter row.
        /// </summary>
        [Parameter] public DataGridRowStyling FilterRowStyling { get; set; }

        /// <summary>
        /// Custom styles for group row.
        /// </summary>
        [Parameter] public DataGridRowStyling GroupRowStyling { get; set; }

        /// <summary>
        /// Template for holding the datagrid columns.
        /// </summary>
        [Parameter] public RenderFragment DataGridColumns { get; set; }

        /// <summary>
        /// Template for holding the datagrid aggregate columns.
        /// </summary>
        [Parameter] public RenderFragment DataGridAggregates { get; set; }

        /// <summary>
        /// If true, DataGrid will use validation when editing the fields.
        /// </summary>
        [Parameter] public bool UseValidation { get; set; }

        /// <summary>
        /// If true, shows feedbacks for all validations.
        /// </summary>
        [Parameter] public bool ShowValidationFeedback { get; set; } = false;

        /// <summary>
        /// If true, shows summary for all validations.
        /// </summary>
        [Parameter] public bool ShowValidationsSummary { get; set; } = true;

        /// <summary>
        /// Label for validations summary.
        /// </summary>
        [Parameter] public string ValidationsSummaryLabel { get; set; }

        /// <summary>
        /// List of custom error messages for the validations summary.
        /// </summary>
        [Parameter] public string[] ValidationsSummaryErrors { get; set; }

        /// <summary>
        /// Defines the default handler type that will be used by the validation, unless it is overriden by <see cref="Validation.HandlerType"/> property.
        /// </summary>
        [Parameter] public Type ValidationsHandlerType { get; set; }

        /// <summary>
        /// Custom localizer handlers to override default <see cref="DataGrid{TItem}"/> localization.
        /// </summary>
        [Parameter] public DataGridLocalizers Localizers { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="DataGrid{TItem}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        ///  Makes Datagrid have a fixed header and enabling a scrollbar in the Datagrid body.
        /// </summary>
        [Parameter] public bool FixedHeader { get; set; }

        /// <summary>
        /// Sets the Datagrid height when <see cref="FixedHeader"/> feature is enabled (defaults to 500px).
        /// </summary>
        [Parameter] public string FixedHeaderDataGridHeight { get; set; } = "500px";

        /// <summary>
        /// Sets the Datagrid max height when <see cref="FixedHeader"/> feature is enabled (defaults to 500px).
        /// </summary>
        [Parameter] public string FixedHeaderDataGridMaxHeight { get; set; } = "500px";

        /// <summary>
        /// Sets the Datagrid's table header <see cref="ThemeContrast"/>.
        /// </summary>
        [Parameter] public ThemeContrast HeaderThemeContrast { get; set; }

        /// <summary>
        /// If true, the edit form will have the Save button as <c>type="submit"</c>, and it will react to Enter keys being pressed.
        /// </summary>
        [Parameter] public bool SubmitFormOnEnter { get; set; } = true;

        /// <summary>
        /// Controls whether DetailRow will start visible if <see cref="DetailRowTemplate"/> is set. <see cref="DetailRowTrigger"/> will be evaluated if set.
        /// </summary>
        [Parameter] public bool DetailRowStartsVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets whether default sort icon should display.
        /// </summary>
        [Parameter] public bool ShowDefaultSortIcon { get; set; }

        /// <summary>
        /// Captures all the custom attribute that are not part of Blazorise component.
        /// </summary>
        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Gets a zero-based index of the currently selected row if found; otherwise it'll return -1. Considers the current pagination.
        /// </summary>
        public int SelectedRowIndex
        {
            get
            {
                var selectedRowDataIdx = Data.Index( x => x.IsEqual( SelectedRow ) );

                return Virtualize
                        ? selectedRowDataIdx
                        : ( selectedRowDataIdx == -1 )
                            ? -1
                            : selectedRowDataIdx + ( CurrentPage - 1 ) * PageSize;
            }
        }

        #endregion
    }
}