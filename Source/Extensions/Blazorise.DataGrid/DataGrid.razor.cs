#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Blazorise.DataGrid.Utils;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.DataGrid
{
    public partial class DataGrid<TItem> : BaseDataGridComponent, IDisposable
    {
        #region Members

        /// <summary>
        /// Element reference to the DataGrid's inner table.
        /// </summary>
        private Table tableRef;

        /// <summary>
        /// Gets or sets whether users can resize datagrid columns.
        /// </summary>
        private bool resizable;

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
        private List<TItem> filteredData = new List<TItem>();

        /// <summary>
        /// Holds the filtered data to display based on the current page.
        /// </summary>
        private IEnumerable<TItem> viewData;

        /// <summary>
        /// Marks the grid to reload entire data source based on the current filter settings.
        /// </summary>
        private bool dirtyFilter = true;

        /// <summary>
        /// Marks the grid to refresh currently visible page.
        /// </summary>
        private bool dirtyView = true;

        /// <summary>
        /// Keeps track of whether the object has already been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Holds the state of sorted columns grouped by the sort-mode.
        /// </summary>
        protected Dictionary<DataGridSortMode, List<DataGridColumn<TItem>>> sortByColumnsDictionary = new Dictionary<DataGridSortMode, List<DataGridColumn<TItem>>>
        {
            { DataGridSortMode.Single, new List<DataGridColumn<TItem>>() },
            { DataGridSortMode.Multiple, new List<DataGridColumn<TItem>>() },
        };

        private readonly Lazy<Func<TItem>> newItemCreator;

        /// <summary>
        /// Currently editing item.
        /// </summary>
        protected TItem editItem;

        /// <summary>
        /// State of the currently editing item.
        /// </summary>
        protected DataGridEditState editState = DataGridEditState.None;

        protected Dictionary<string, CellEditContext<TItem>> editItemCellValues;

        protected Dictionary<string, CellEditContext<TItem>> filterCellValues;

        /// <summary>
        /// Holds the pagination templates
        /// </summary>
        protected PaginationTemplates<TItem> paginationTemplates;

        /// <summary>
        /// Holds the pagination context
        /// </summary>
        protected PaginationContext<TItem> paginationContext;

        #endregion

        #region Constructors

        public DataGrid()
        {
            newItemCreator = new Lazy<Func<TItem>>( () => FunctionCompiler.CreateNewItem<TItem>() );

            paginationTemplates = new PaginationTemplates<TItem>();
            paginationContext = new PaginationContext<TItem>( this );
        }

        #endregion

        #region Methods

        #region Setup

        /// <summary>
        /// Links the child column with this datagrid.
        /// </summary>
        /// <param name="column">Column to link with this datagrid.</param>
        internal void Hook( DataGridColumn<TItem> column )
        {
            Columns.Add( column );

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
        /// Links the child column with this datagrid.
        /// </summary>
        /// <param name="column">Column to link with this datagrid.</param>
        internal void Hook( DataGridAggregate<TItem> aggregate )
        {
            Aggregates.Add( aggregate );
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                paginationContext.SubscribeOnPageSizeChanged( OnPageSizeChanged );
                paginationContext.SubscribeOnPageChanged( OnPageChanged );

                if ( ManualReadMode )
                {
                    await HandleReadData( CancellationToken.None );

                    return;
                }

                // after all the columns have being "hooked" we need to resfresh the grid
                await InvokeAsync( StateHasChanged );
            }

            await RecalculateResize();

            await base.OnAfterRenderAsync( firstRender );
        }

        protected override void Dispose( bool disposing )
        {
            if ( !disposed )
            {
                disposed = true;

                paginationContext.UnsubscribeOnPageSizeChanged( OnPageSizeChanged );
                paginationContext.UnsubscribeOnPageChanged( OnPageChanged );

                base.Dispose();
            }
        }

        /// <summary>
        /// If DataGrid is resizable. 
        /// Resizable columns should be constantly recalculated to keep up with the current Datagrid's height dimensions.
        /// </summary>
        /// <returns></returns>
        private async ValueTask RecalculateResize()
        {
            if ( resizable )
            {
                await DestroyResizable();
                await InitResizable();
            }
        }

        private ValueTask InitResizable()
            => JSRuntime.InvokeVoidAsync( JSInteropFunction.INIT_RESIZABLE, tableRef.ElementRef, ResizeMode );

        private ValueTask DestroyResizable()
            => JSRuntime.InvokeVoidAsync( JSInteropFunction.DESTROY_RESIZABLE, tableRef.ElementRef );

        #endregion

        #region Events

        private async void OnPageSizeChanged( int pageSize )
        {
            paginationContext.CancellationTokenSource?.Cancel();
            paginationContext.CancellationTokenSource = new CancellationTokenSource();

            await InvokeAsync( () => PageSizeChanged.InvokeAsync( pageSize ) );

            if ( ManualReadMode )
            {
                await InvokeAsync( () => HandleReadData( paginationContext.CancellationTokenSource.Token ) );
            }
            else
            {
                await InvokeAsync( StateHasChanged );
            }
        }

        private async void OnPageChanged( int currentPage )
        {
            paginationContext.CancellationTokenSource?.Cancel();
            paginationContext.CancellationTokenSource = new CancellationTokenSource();

            await InvokeAsync( () => PageChanged.InvokeAsync( new DataGridPageChangedEventArgs( currentPage, PageSize ) ) );

            if ( ManualReadMode )
            {
                await InvokeAsync( () => HandleReadData( paginationContext.CancellationTokenSource.Token ) );
            }
            else
            {
                await InvokeAsync( StateHasChanged );
            }
        }

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
            editItemCellValues = new Dictionary<string, CellEditContext<TItem>>();

            foreach ( var column in EditableColumns )
            {
                editItemCellValues.Add( column.ElementId, new CellEditContext<TItem>( item )
                {
                    CellValue = column.GetValue( editItem ),
                } );
            }
        }

        protected Task OnSelectedCommand( TItem item )
        {
            return SelectRow( item );
        }

        protected Task OnRowClickedCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowClicked.InvokeAsync( eventArgs );
        }

        protected Task OnRowDoubleClickedCommand( DataGridRowMouseEventArgs<TItem> eventArgs )
        {
            return RowDoubleClicked.InvokeAsync( eventArgs );
        }

        protected void OnNewCommand()
        {
            TItem newItem = NewItemCreator != null ? NewItemCreator.Invoke() : CreateNewItem();

            NewItemDefaultSetter?.Invoke( newItem );

            InitEditItem( newItem );

            editState = DataGridEditState.New;

            if ( EditMode == DataGridEditMode.Popup )
                PopupVisible = true;
        }

        protected void OnEditCommand( TItem item )
        {
            InitEditItem( item );

            editState = DataGridEditState.Edit;

            if ( EditMode == DataGridEditMode.Popup )
                PopupVisible = true;
        }

        protected async Task OnDeleteCommand( TItem item )
        {
            if ( Data is ICollection<TItem> data )
            {
                if ( await IsSafeToProceed( RowRemoving, item ) )
                {
                    if ( UseInternalEditing )
                    {
                        if ( data.Contains( item ) )
                            data.Remove( item );
                    }

                    await RowRemoved.InvokeAsync( item );

                    dirtyFilter = dirtyView = true;
                }
            }

            // When deleting and the page becomes empty and we aren't the first page:
            // go to the previous page
            if ( ManualReadMode && ShowPager && CurrentPage > paginationContext.FirstVisiblePage && !Data.Any() )
            {
                await OnPaginationItemClick( ( CurrentPage - 1 ).ToString() );
            }
        }

        protected async Task OnSaveCommand()
        {
            if ( Data == null )
                return;

            // get the list of edited values
            var editedCellValues = EditableColumns
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
                    await RowInserted.InvokeAsync( new SavedRowItem<TItem, Dictionary<string, object>>( editItem, editedCellValues ) );
                    dirtyFilter = dirtyView = true;

                    // If a new item is added, the data should be refreshed
                    // to account for paging, sorting, and filtering
                    if ( ManualReadMode )
                        await HandleReadData( CancellationToken.None );
                }
                else
                    await RowUpdated.InvokeAsync( new SavedRowItem<TItem, Dictionary<string, object>>( editItem, editedCellValues ) );

                editState = DataGridEditState.None;

                if ( EditMode == DataGridEditMode.Popup )
                    PopupVisible = false;
            }
        }

        protected void OnCancelCommand()
        {
            editState = DataGridEditState.None;

            if ( EditMode == DataGridEditMode.Popup )
                PopupVisible = false;
        }

        protected async Task OnMultiSelectCommand( MultiSelectEventArgs<TItem> eventArgs )
        {
            SelectedAllRows = false;
            UnSelectAllRows = false;

            if ( SelectedRows is null )
            {
                SelectedRows = new List<TItem>();
            }

            if ( eventArgs.Selected && !SelectedRows.Contains( eventArgs.Item ) )
            {
                SelectedRows.Add( eventArgs.Item );
            }

            if ( !eventArgs.Selected && SelectedRows.Contains( eventArgs.Item ) )
            {
                SelectedRows.Remove( eventArgs.Item );

                if ( SelectedRow.IsEqual( eventArgs.Item ) )
                {
                    await SelectedRowChanged.InvokeAsync( default( TItem ) );
                }
            }

            await SelectedRowsChanged.InvokeAsync( SelectedRows );
        }

        protected async Task OnMultiSelectAll( bool selectAll )
        {
            if ( SelectedRows is null )
            {
                SelectedRows = new List<TItem>();
            }

            if ( selectAll )
            {
                SelectedRows.Clear();
                SelectedRows.AddRange( viewData );
            }
            else
            {
                SelectedRows.Clear();

                await SelectedRowChanged.InvokeAsync( default( TItem ) );
            }

            SelectedAllRows = selectAll;
            UnSelectAllRows = !selectAll;

            await SelectedRowsChanged.InvokeAsync( SelectedRows );
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

        /// <summary>
        /// Triggers the reload of the <see cref="DataGrid{TItem}"/> data.
        /// </summary>
        /// <returns>Returns the awaitable task.</returns>
        public Task Reload()
        {
            dirtyFilter = dirtyView = true;

            if ( ManualReadMode )
            {
                return InvokeAsync( () => HandleReadData( CancellationToken.None ) );
            }
            else
            {
                return InvokeAsync( StateHasChanged );
            }
        }

        protected async Task HandleReadData( CancellationToken cancellationToken )
        {
            try
            {
                IsLoading = true;
                if ( !cancellationToken.IsCancellationRequested )
                    await ReadData.InvokeAsync( new DataGridReadDataEventArgs<TItem>( CurrentPage, PageSize, Columns, cancellationToken ) );
            }
            finally
            {
                IsLoading = false;

                await InvokeAsync( StateHasChanged );
            }
        }

        protected Task OnSortClicked( DataGridColumn<TItem> column )
        {
            if ( Sortable && column.Sortable )
            {
                if ( SortMode == DataGridSortMode.Single )
                {
                    // in single-mode we need to reset all other columns to default state
                    foreach ( var c in Columns.Where( x => x.Field != column.Field ) )
                    {
                        c.CurrentDirection = SortDirection.None;
                    }

                    // and also remove any column sort info except for current one
                    SortByColumns.RemoveAll( x => x.Field != column.Field );
                }

                column.CurrentDirection = column.CurrentDirection.NextDirection();

                if ( !ManualReadMode )
                {
                    if ( !SortByColumns.Any( c => c.Field == column.Field ) )
                    {
                        SortByColumns.Add( column );
                    }
                    else if ( column.CurrentDirection == SortDirection.None )
                        SortByColumns.Remove( column );
                }

                dirtyFilter = dirtyView = true;

                if ( ManualReadMode )
                    return HandleReadData( CancellationToken.None );
            }

            return Task.CompletedTask;
        }

        protected internal Task OnFilterChanged( DataGridColumn<TItem> column, string value )
        {
            column.Filter.SearchValue = value;
            dirtyFilter = dirtyView = true;

            if ( ManualReadMode )
                return HandleReadData( CancellationToken.None );

            return Task.CompletedTask;
        }

        protected Task OnClearFilterCommand()
        {
            foreach ( var column in Columns )
            {
                column.Filter.SearchValue = null;
            }

            dirtyFilter = dirtyView = true;

            if ( ManualReadMode )
                return HandleReadData( CancellationToken.None );

            return Task.CompletedTask;
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

        private void FilterData()
        {
            FilterData( Data?.AsQueryable() );
        }

        private void FilterData( IQueryable<TItem> query )
        {
            if ( query == null )
            {
                filteredData.Clear();
                FilteredDataChanged?.Invoke( filteredData );

                return;
            }

            // only use internal filtering if we're not using custom data loading
            if ( !ManualReadMode )
            {
                var firstSort = true;

                foreach ( var sortByColumn in SortByColumns )
                {
                    if ( firstSort )
                    {
                        if ( sortByColumn.CurrentDirection == SortDirection.Ascending )
                            query = query.OrderBy( x => sortByColumn.GetValue( x ) );
                        else
                            query = query.OrderByDescending( x => sortByColumn.GetValue( x ) );

                        firstSort = false;
                    }
                    else
                    {
                        if ( sortByColumn.CurrentDirection == SortDirection.Ascending )
                            query = ( query as IOrderedQueryable<TItem> ).ThenBy( x => sortByColumn.GetValue( x ) );
                        else
                            query = ( query as IOrderedQueryable<TItem> ).ThenByDescending( x => sortByColumn.GetValue( x ) );
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

                    if ( string.IsNullOrEmpty( column.Filter.SearchValue ) )
                        continue;

                    query = from item in query
                            let cellRealValue = column.GetValue( item )
                            let cellStringValue = cellRealValue == null ? string.Empty : cellRealValue.ToString()
                            where CompareFilterValues( cellStringValue, column.Filter.SearchValue )
                            select item;
                }
            }

            filteredData = query.ToList();

            dirtyFilter = false;

            FilteredDataChanged?.Invoke( filteredData );
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
            if ( !ManualReadMode )
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

        public Task SelectRow( TItem item )
        {
            if ( editState != DataGridEditState.None )
                return Task.CompletedTask;

            SelectedRow = item;

            return SelectedRowChanged.InvokeAsync( SelectedRow );
        }

        #endregion

        #endregion

        #region Properties

        [Inject] private IJSRuntime JSRuntime { get; set; }

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
        /// List of all the columns associated with this datagrid.
        /// </summary>
        protected List<DataGridColumn<TItem>> Columns { get; } = new List<DataGridColumn<TItem>>();

        /// <summary>
        /// List of all the aggregate columns associated with this datagrid.
        /// </summary>
        protected List<DataGridAggregate<TItem>> Aggregates { get; } = new List<DataGridAggregate<TItem>>();

        /// <summary>
        /// Gets only columns that are available for editing.
        /// </summary>
        protected IEnumerable<DataGridColumn<TItem>> EditableColumns => Columns.Where( x => !x.ExcludeFromEdit && x.Editable );

        /// <summary>
        /// Gets only columns that are available for display in the grid.
        /// </summary>
        protected IEnumerable<DataGridColumn<TItem>> DisplayableColumns
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
        /// Returns true if data is not empty, data is not loaded, empty and loading template is not set.
        /// </summary>
        protected bool IsDisplayDataVisible => !IsLoadingTemplateVisible && !IsEmptyTemplateVisible;

        /// <summary>
        /// Returns true if LoadingTemplate is set and IsLoading is true.
        /// </summary>
        protected bool IsLoadingTemplateVisible => !IsNewItemInGrid && LoadingTemplate != null && IsLoading;

        /// <summary>
        /// Returns true if ReadData will be invoked.
        /// </summary>
        protected bool IsLoading { get; set; }

        /// <summary>
        /// Returns true if EmptyTemplate is set and Data is null or empty.
        /// </summary>
        protected bool IsEmptyTemplateVisible => !IsLoadingTemplateVisible && !IsNewItemInGrid && EmptyTemplate != null && ( Data == null || !Data.Any() );

        /// <summary>
        /// Returns true if ShowPager is true and grid is not empty or loading.
        /// </summary>
        protected bool IsPagerVisible => ShowPager && !IsLoadingTemplateVisible && ( ( IsButtonRowVisible && ButtonRowTemplate != null ) || !IsEmptyTemplateVisible );

        /// <summary>
        /// Returns true if current state is for new item and editing fields are shown on datagrid.
        /// </summary>
        protected bool IsNewItemInGrid => Editable && editState == DataGridEditState.New && EditMode != DataGridEditMode.Popup;

        /// <summary>
        /// True if user is using <see cref="ReadData"/> for loading the data.
        /// </summary>
        public bool ManualReadMode => ReadData.HasDelegate;

        /// <summary>
        /// Gets the current datagrid editing state.
        /// </summary>
        public DataGridEditState EditState => editState;

        /// <summary>
        /// Gets the sort solumn info for current SortMode.
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
        protected bool PopupVisible = false;

        /// <summary>
        /// Defines the size of popup dialog.
        /// </summary>
        [Parameter] public ModalSize PopupSize { get; set; } = ModalSize.Default;

        /// <summary>
        /// Gets the reference to the associated command column.
        /// </summary>
        public DataGridCommandColumn<TItem> CommandColumn { get; private set; }

        /// <summary>
        /// Gets the reference to the associated multiselect column.
        /// </summary>
        public DataGridMultiSelectColumn<TItem> MultiSelectColumn { get; private set; }

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
                    var unselectedRows = viewData.Except( SelectedRows ).Count();

                    return MultiSelect && hasSelectedRows && unselectedRows > 0 && unselectedRows < viewData.Count();
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the datagrid data-source.
        /// </summary>
        [Parameter]
        public IEnumerable<TItem> Data
        {
            get { return data; }
            set
            {
                data = value;

                // make sure everything is recalculated
                dirtyFilter = dirtyView = true;
            }
        }

        /// <summary>
        /// Gets or sets the clalculated aggregate data.
        /// </summary>
        /// <remarks>
        /// Used only in manual mode along with the <see cref="ReadData"/> handler.
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
        protected internal IEnumerable<TItem> FilteredData
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
        [Parameter] public Action<IEnumerable<TItem>> FilteredDataChanged { get; set; }

        /// <summary>
        /// Gets the data to show on grid based on the filter and current page.
        /// </summary>
        internal IEnumerable<TItem> DisplayData
        {
            get
            {
                if ( dirtyView )
                    viewData = FilterViewData();

                return viewData;
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
        /// Gets or sets whether users can resize datagrid columns.
        /// </summary>
        [Parameter]
        public bool Resizable
        {
            get => resizable;
            set
            {
                if ( resizable == value )
                    return;

                resizable = value;

                if ( !resizable )
                    ExecuteAfterRender( () => DestroyResizable().AsTask() );
            }
        }

        /// <summary>
        /// Gets or sets whether the user can resize on header or columns.
        /// </summary>
        [Parameter] public DataGridResizeMode ResizeMode { get; set; }

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
        [Parameter] public RenderFragment<PageButtonContext> PageButtonTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of items per page of grid.
        /// </summary>
        [Parameter] public RenderFragment ItemsPerPageTemplate { get; set; }

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
        [Parameter] public DataGridSelectionMode SelectionMode { get; set; }

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
        /// Event called after the row is clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowClicked { get; set; }

        /// <summary>
        /// Event called after the row is double clicked.
        /// </summary>
        [Parameter] public EventCallback<DataGridRowMouseEventArgs<TItem>> RowDoubleClicked { get; set; }

        /// <summary>
        /// Occurs after the selected page has changed.
        /// </summary>
        [Parameter] public EventCallback<DataGridPageChangedEventArgs> PageChanged { get; set; }

        /// <summary>
        /// Event handler used to load data manually based on the current page and filter data settings.
        /// </summary>
        [Parameter] public EventCallback<DataGridReadDataEventArgs<TItem>> ReadData { get; set; }

        /// <summary>
        /// Specifes the grid editing modes.
        /// </summary>
        [Parameter] public DataGridEditMode EditMode { get; set; } = DataGridEditMode.Form;

        /// <summary>
        /// Specifies the grid command mode.
        /// </summary>
        [Parameter] public DataGridCommandMode CommandMode { get; set; }

        /// <summary>
        /// A trigger function used to handle the visibility of detail row.
        /// </summary>
        [Parameter] public Func<TItem, bool> DetailRowTrigger { get; set; }

        /// <summary>
        /// Handles the selection of the clicked row.
        /// If not set it will default to always true.
        /// </summary>
        [Parameter] public Func<TItem, bool> RowSelectable { get; set; }

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
        [Parameter] public Func<TItem, bool> CustomFilter { get; set; }

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
        /// If true, shows feedbacks for all validations.
        /// </summary>
        [Parameter] public bool ShowValidationFeedback { get; set; } = false;

        /// <summary>
        /// If true, shows summary for all validations.
        /// </summary>
        [Parameter] public bool ShowValidationsSummary { get; set; } = true;

        /// <summary>
        /// Label for validaitons summary.
        /// </summary>
        [Parameter] public string ValidationsSummaryLabel { get; set; }

        /// <summary>
        /// Custom localizer handlers to override default <see cref="DataGrid{TItem}"/> localization.
        /// </summary>
        [Parameter] public DataGridLocalizers Localizers { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="DataGrid{TItem}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}