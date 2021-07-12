#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Represents the base class for cancellable events when datagrid item is saving.
    /// </summary>
    /// <typeparam name="TItem">Model type param.</typeparam>
    public class CancellableRowChange<TItem> : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the cancelable event argument.
        /// </summary>
        /// <param name="item">Saved item.</param>
        public CancellableRowChange( TItem item )
        {
            Item = item;
        }

        /// <summary>
        /// Gets the model that was saved.
        /// </summary>
        public TItem Item { get; }
    }

    /// <summary>
    /// Represents the base class for cancellable events when datagrid item is saving.
    /// </summary>
    /// <typeparam name="TItem">Model type param.</typeparam>
    /// <typeparam name="TValues">Values type param.</typeparam>
    public class CancellableRowChange<TItem, TValues> : CancellableRowChange<TItem>
    {
        /// <summary>
        /// Initializes a new instance of the cancelable event argument.
        /// </summary>
        /// <param name="item">Saved item.</param>
        /// <param name="values">Edited values.</param>
        public CancellableRowChange( TItem item, TValues values )
            : base( item )
        {
            Values = values;
        }

        /// <summary>
        /// Values that are being edited by the datagrid.
        /// </summary>
        public TValues Values { get; }
    }

    /// <summary>
    /// Represents the base class for items saved by the datagrid.
    /// </summary>
    /// <typeparam name="TItem">Model type param.</typeparam>
    /// <typeparam name="TValues">Values type param.</typeparam>
    public class SavedRowItem<TItem, TValues> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the event argument.
        /// </summary>
        /// <param name="item">Saved item.</param>
        /// <param name="values">Edited values.</param>
        public SavedRowItem( TItem item, TValues values )
        {
            Item = item;
            Values = values;
        }

        /// <summary>
        /// Gets the model that was saved.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Values that are being edited by the datagrid.
        /// </summary>
        public TValues Values { get; }
    }

    /// <summary>
    /// Provides the data for datagrid page change.
    /// </summary>
    public class DataGridPageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of page changed event argument.
        /// </summary>
        /// <param name="page">Page number at the moment of initialization.</param>
        /// <param name="pageSize">Maximum number of items per page.</param>
        public DataGridPageChangedEventArgs( int page, int pageSize )
        {
            Page = page;
            PageSize = pageSize;
        }

        /// <summary>
        /// Gets the requested page number.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets the max number of items requested by page.
        /// </summary>
        public int PageSize { get; }
    }

    /// <summary>
    /// Provides all the information for loading the datagrid data manually.
    /// </summary>
    /// <typeparam name="TItem">Type of the data model.</typeparam>
    public class DataGridReadDataEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of read-data event argument.
        /// </summary>
        /// <param name="readDataMode">ReadData Mode.</param>
        /// <param name="columns">List of all the columns in the grid.</param>
        /// <param name="sortByColumns">List of all the columns by which we're sorting the grid.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="page">Page number at the moment of initialization.</param>
        /// <param name="pageSize">Maximum number of items per page.</param>
        /// <param name="virtualizeStartIndex">Requested data start index by Virtualize.</param>
        /// <param name="virtualizeCount">Max number of items requested by Virtualize.</param>
        public DataGridReadDataEventArgs(
            ReadDataMode readDataMode,
            IEnumerable<DataGridColumn<TItem>> columns,
            IList<DataGridColumn<TItem>> sortByColumns,
            CancellationToken cancellationToken = default,
            int page = 0, int pageSize = 0, int virtualizeStartIndex = 0, int virtualizeCount = 0
            )
        {
            Page = page;
            PageSize = pageSize;
            Columns = columns?.Select( x => new DataGridColumnInfo(
                x.Field,
                x.Filter?.SearchValue,
                x.CurrentSortDirection,
                sortByColumns?.IndexOf( x ) ?? -1,
                x.ColumnType ) );
            CancellationToken = cancellationToken;
            this.VirtualizeStartIndex = virtualizeStartIndex;
            this.VirtualizeCount = virtualizeCount;
            this.ReadDataMode = readDataMode;
        }

        /// <summary>
        /// Gets the ReadData Mode.
        /// </summary>
        public ReadDataMode ReadDataMode { get; }

        /// <summary>
        /// Gets the requested data start index by Virtualize.
        /// </summary>
        public int VirtualizeStartIndex { get; }

        /// <summary>
        /// Gets the max number of items requested by Virtualize.
        /// </summary>
        public int VirtualizeCount { get; }

        /// <summary>
        /// Gets the requested page number.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets the max number of items requested by page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the list of columns.
        /// </summary>
        public IEnumerable<DataGridColumnInfo> Columns { get; }

        /// <summary>
        /// Gets the CancellationToken
        /// </summary>
        public CancellationToken CancellationToken { get; set; }
    }

    /// <summary>
    /// Provides all the information for filtered data items.
    /// </summary>
    /// <typeparam name="TItem">Type of the data model.</typeparam>
    public class DataGridFilteredDataEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of filtered-data event argument.
        /// </summary>
        /// <param name="filteredData">List of filtered data items.</param>
        /// <param name="filteredItems">Number of filtered items.</param>
        /// <param name="totalItems">Total available items in the data-source.</param>
        public DataGridFilteredDataEventArgs( IEnumerable<TItem> filteredData, int filteredItems, int totalItems )
        {
            Data = filteredData;
            FilteredItems = filteredItems;
            TotalItems = totalItems;
        }

        /// <summary>
        /// Gets the list of filtered data items.
        /// </summary>
        public IEnumerable<TItem> Data { get; }

        /// <summary>
        /// Gets the number of filtered items.
        /// </summary>
        public int FilteredItems { get; }

        /// <summary>
        /// Gets the total available items in the data-source.
        /// </summary>
        public int TotalItems { get; }
    }

    /// <summary>
    /// Provides all the information about the clicked event on datagrid row.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class DataGridRowMouseEventArgs<TItem> : EventArgs
    {
        public DataGridRowMouseEventArgs( TItem item, BLMouseEventArgs mouseEventArgs )
        {
            Item = item;
            MouseEventArgs = mouseEventArgs;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Gets the mouse event details.
        /// </summary>
        public BLMouseEventArgs MouseEventArgs { get; }
    }

    /// <summary>
    /// Provides all the information about the multi select event.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class MultiSelectEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Default constructors.
        /// </summary>
        /// <param name="item">Model that belongs to the grid row.</param>
        /// <param name="selected">Indicates if the row is selected or not.</param>
        /// <param name="shiftKey">True if the user is holding shift key.</param>
        public MultiSelectEventArgs( TItem item, bool selected, bool shiftKey )
        {
            Item = item;
            Selected = selected;
            ShiftKey = shiftKey;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Returns true if the row is selected.
        /// </summary>
        public bool Selected { get; }

        /// <summary>
        /// Returns true if user has ShiftClicked.
        /// </summary>
        public bool ShiftKey { get; }
    }

    /// <summary>
    /// Provides all the information about the current column sorting.
    /// </summary>
    public class DataGridSortChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructors.
        /// </summary>
        /// <param name="fieldName">Column field name.</param>
        /// <param name="sortDirection">Column sort direction.</param>
        public DataGridSortChangedEventArgs( string fieldName, SortDirection sortDirection )
        {
            FieldName = fieldName;
            SortDirection = sortDirection;
        }

        /// <summary>
        /// Gets the field name of the column that is being sorted.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Gets the new sort direction of the specified field name.
        /// </summary>
        public SortDirection SortDirection { get; }
    }
}