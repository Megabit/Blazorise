#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.DataGrid.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    public abstract class BaseDataGrid<TItem> : BaseComponent
    {
        #region Members

        /// <summary>
        /// Original data-source.
        /// </summary>
        private IEnumerable<TItem> data;

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

        // Sorting by multiple columns is not ready yet because of the bug in Mono runtime.
        // issue https://github.com/aspnet/AspNetCore/issues/11371
        // Until the bug is fixed only one column will be supported.
        protected BaseDataGridColumn<TItem> sortByColumn = null;

        private readonly Lazy<Func<TItem>> newItemCreator;

        /// <summary>
        /// Currently editing item.
        /// </summary>
        protected TItem editItem;

        /// <summary>
        /// State of the currently editing item.
        /// </summary>
        protected DataGridEditState editState = DataGridEditState.None;

        protected Dictionary<string, CellEditContext> editItemCellValues;

        protected Dictionary<string, CellEditContext> filterCellValues;

        #endregion

        #region Constructors

        public BaseDataGrid()
        {
            newItemCreator = new Lazy<Func<TItem>>( () => FunctionCompiler.CreateNewItem<TItem>() );
        }

        #endregion

        #region Methods

        #region Initialisation

        /// <summary>
        /// Links the column with this datagrid.
        /// </summary>
        /// <param name="column">Column to link with this datagrid.</param>
        internal void Hook( BaseDataGridColumn<TItem> column )
        {
            Columns.Add( column );
        }

        protected override Task OnFirstAfterRenderAsync()
        {
            // after all the columns have being "hooked" we need to resfresh the grid
            StateHasChanged();

            return base.OnFirstAfterRenderAsync();
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
            editItemCellValues = new Dictionary<string, CellEditContext>();

            foreach ( var column in EditableColumns )
            {
                editItemCellValues.Add( column.ElementId, new CellEditContext
                {
                    CellValue = column.GetValue( editItem ),
                } );
            }
        }

        protected Task OnSelectedCommand( TItem item )
        {
            return SelectRow( item );
        }

        protected void OnNewCommand()
        {
            InitEditItem( CreateNewItem() );

            editState = DataGridEditState.New;
        }

        protected void OnEditCommand( TItem item )
        {
            InitEditItem( item );

            editState = DataGridEditState.Edit;
        }

        protected async Task OnDeleteCommand( TItem item )
        {
            if ( Data is ICollection<TItem> data )
            {
                if ( IsSafeToProceed( RowRemoving, item ) )
                {
                    if ( UseInternalEditing )
                    {
                        if ( data.Contains( item ) )
                            data.Remove( item );
                    }
                }

                await RowRemoved.InvokeAsync( item );

                dirtyFilter = dirtyView = true;
            }
        }

        protected async Task OnSaveCommand()
        {
            if ( Data is ICollection<TItem> data )
            {
                // get the list of edited values
                var editedCellValues = EditableColumns
                    .Select( c => new { c.Field, editItemCellValues[c.ElementId].CellValue } ).ToDictionary( x => x.Field, x => x.CellValue );

                if ( IsSafeToProceed( RowSaving, editItem, editedCellValues ) )
                {
                    if ( UseInternalEditing && editState == DataGridEditState.New && CanInsertNewItem )
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
                    }
                    else
                        await RowUpdated.InvokeAsync( new SavedRowItem<TItem, Dictionary<string, object>>( editItem, editedCellValues ) );

                    editState = DataGridEditState.None;
                }
            }
        }

        protected void OnCancelCommand()
        {
            editState = DataGridEditState.None;
        }

        // this is to give user a way to stop save if necessary
        internal bool IsSafeToProceed<TValues>( Action<CancellableRowChange<TItem, TValues>> handler, TItem item, TValues editedCellValues )
        {
            if ( handler != null )
            {
                var args = new CancellableRowChange<TItem, TValues>( item, editedCellValues );

                foreach ( Action<CancellableRowChange<TItem, TValues>> subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal bool IsSafeToProceed( Action<CancellableRowChange<TItem>> handler, TItem item )
        {
            if ( handler != null )
            {
                var args = new CancellableRowChange<TItem>( item );

                foreach ( Action<CancellableRowChange<TItem>> subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Filtering

        protected void OnSortClicked( BaseDataGridColumn<TItem> column )
        {
            if ( AllowSort && column.AllowSort )
            {
                column.Direction = column.Direction == SortDirection.Descending ? SortDirection.Ascending : SortDirection.Descending;
                sortByColumn = column;

                // just one column can be sorted for now!
                foreach ( var col in Columns )
                {
                    if ( col.ElementId == column.ElementId )
                        continue;

                    // reset all others
                    col.Direction = SortDirection.Ascending;
                }

                dirtyFilter = dirtyView = true;
            }
        }

        protected void OnFilterChanged( BaseDataGridColumn<TItem> column, string value )
        {
            column.Filter.SearchValue = value;
            dirtyFilter = dirtyView = true;
        }

        protected void OnClearFilterClicked()
        {
            foreach ( var column in Columns )
            {
                column.Filter.SearchValue = null;
            }

            dirtyFilter = dirtyView = true;
        }

        protected void OnPaginationItemClick( string pageName )
        {
            if ( int.TryParse( pageName, out var pageNumber ) )
                CurrentPage = pageNumber;
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

                    if ( CurrentPage > LastPage )
                        CurrentPage = LastPage;
                }
            }

            PageChanged?.Invoke( CurrentPage.ToString() );
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
                return;
            }

            //IOrderedQueryable<TItem> orderedQuery = null;

            // just one column can be sorted for now!
            if ( sortByColumn != null && sortByColumn.AllowSort )
            {
                //if ( sortByColumn.Direction == "desc" )
                //    orderedQuery = query.OrderByDescending( item => sortByColumn.Field( item ) );
                //else
                //    orderedQuery = query.OrderBy( item => sortByColumn.Field( item ), string.Compare( ) );
                query = query.OrderBy( item => sortByColumn.GetValue( item ),
                    sortByColumn.Direction == SortDirection.Descending ? new MyComparer<object>() : new MyReverseComparer<object>() );
            }

            // Sorting by multiple columns is not ready yet because of bug in Mono runtime.
            // issue https://github.com/aspnet/AspNetCore/issues/11371
            //foreach ( var column in dataGridColumns.Values )
            //{
            //    if ( orderedQuery == null )
            //    {
            //        if ( column.Direction == "desc" )
            //            orderedQuery = query.OrderByDescending( x => column.Field( x ) );
            //        else
            //            orderedQuery = query.OrderBy( x => column.Field( x ) );
            //    }
            //    else
            //    {
            //        if ( column.Direction == "desc" )
            //            orderedQuery = orderedQuery.ThenByDescending( x => column.Field( x ) );
            //        else
            //            orderedQuery = orderedQuery.ThenBy( x => column.Field( x ) );
            //    }
            //}

            foreach ( var column in Columns )
            {
                if ( column.ColumnType == DataGridColumnType.Command )
                    continue;

                if ( string.IsNullOrEmpty( column.Filter.SearchValue ) )
                    continue;

                query = from q in query
                        let cellRealValue = column.GetValue( q )
                        let cellStringValue = cellRealValue == null ? string.Empty : cellRealValue.ToString()
                        where cellStringValue.Contains( column.Filter.SearchValue )
                        select q;
            }

            filteredData = query.ToList();

            dirtyFilter = false;

            //return query.Skip( ( CurrentPage - 1 ) * PageSize ).ToList();
            //return orderedQuery == null
            //    ? query?.ToList()
            //    : orderedQuery?.ToList();
        }

        private IEnumerable<TItem> FilterViewData()
        {
            if ( dirtyFilter )
                FilterData();

            // Again!, the Mono runtime is giving me trouble with Skip().Take() so I had to do it this way...
            // NOTE: report bug with Take()
            for ( int i = ( CurrentPage - 1 ) * PageSize, j = 0; i < filteredData.Count(); ++i, ++j )
            {
                if ( j >= PageSize )
                    break;

                yield return filteredData[i];
            }
        }

        public Task SelectRow( TItem item )
        {
            SelectedRow = item;
            return SelectedRowChanged.InvokeAsync( SelectedRow );
            //StateHasChanged();
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// List of all the columns associated with this datagrid.
        /// </summary>
        protected List<BaseDataGridColumn<TItem>> Columns { get; } = new List<BaseDataGridColumn<TItem>>();

        /// <summary>
        /// Gets only columns that are available for editing.
        /// </summary>
        protected IEnumerable<BaseDataGridColumn<TItem>> EditableColumns => Columns.Where( x => x.ColumnType != DataGridColumnType.Command && x.AllowEdit );

        /// <summary>
        /// Returns true if <see cref="Data"/> is safe to modify.
        /// </summary>
        protected bool CanInsertNewItem => AllowEdit && Data is ICollection<TItem>;

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
        /// Gets the data after all of the filters have being applied.
        /// </summary>
        protected IEnumerable<TItem> FilteredData
        {
            get
            {
                if ( dirtyFilter )
                    FilterData();

                return filteredData;
            }
        }

        /// <summary>
        /// Gets the data to show on grid based on the filter and current page.
        /// </summary>
        protected IEnumerable<TItem> DisplayData
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
        [Parameter] public bool AllowEdit { get; set; }

        /// <summary>
        /// Gets or sets whether end-users can sort data by the column's values.
        /// </summary>
        [Parameter] public bool AllowSort { get; set; } = true;

        /// <summary>
        /// Gets or sets whether users can filter rows by its cell values.
        /// </summary>
        [Parameter] public bool AllowFilter { get; set; }

        /// <summary>
        /// Gets or sets whether users can navigate datagrid by using pagination controls.
        /// </summary>
        [Parameter] public bool ShowPager { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        [Parameter] public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Gets the last page number.
        /// </summary>
        protected int LastPage
        {
            get
            {
                var lastPage = Math.Max( (int)Math.Ceiling( ( FilteredData?.Count() ?? 0 ) / (double)PageSize ), 1 );

                if ( CurrentPage > lastPage )
                    CurrentPage = lastPage;

                return lastPage;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of items for each page.
        /// </summary>
        [Parameter] public int PageSize { get; set; } = 5;

        /// <summary>
        /// Gets or sets currently selected row.
        /// </summary>
        [Parameter] public TItem SelectedRow { get; set; }

        /// <summary>
        /// Occurs after the selected row has changed.
        /// </summary>
        [Parameter] public EventCallback<TItem> SelectedRowChanged { get; set; }

        /// <summary>
        /// Cancelable event called before the row is inserted or updated.
        /// </summary>
        [Parameter] public Action<CancellableRowChange<TItem, Dictionary<string, object>>> RowSaving { get; set; }

        /// <summary>
        /// Cancelable event called before the row is removed.
        /// </summary>
        [Parameter] public Action<CancellableRowChange<TItem>> RowRemoving { get; set; }

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
        /// Occurs after the selected page has changed.
        /// </summary>
        [Parameter] public Action<string> PageChanged { get; set; }

        /// <summary>
        /// Specifes the grid editing modes.
        /// </summary>
        [Parameter] public DataGridEditMode EditMode { get; set; } = DataGridEditMode.Form;

        [Parameter] protected RenderFragment<TItem> DetailRowTemplate { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion

        #region Helpers

        /// <summary>
        /// This is just a temporary solutions until the bug for OrderByDescending in Mono is fixed
        /// https://github.com/aspnet/AspNetCore/issues/11371
        /// </summary>
        class MyComparer<T> : IComparer<T>
        {
            public virtual int Compare( T x, T y )
            {
                return Convert.ToString( x ).CompareTo( Convert.ToString( y ) );
            }
        }

        class MyReverseComparer<T> : MyComparer<T>
        {
            public override int Compare( T x, T y )
            {
                return base.Compare( y, x );
            }
        }

        #endregion
    }
}
