#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    /// <typeparam name="TItem"></typeparam>
    public class DataGridReadDataEventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of read-data event argument.
        /// </summary>
        /// <param name="page">Page number at the moment of initialization.</param>
        /// <param name="pageSize">Maximum number of items per page.</param>
        /// <param name="columns">List of all the columns in the grid.</param>
        public DataGridReadDataEventArgs( int page, int pageSize, IEnumerable<DataGridColumn<TItem>> columns, CancellationToken cancellationToken )
        {
            Page = page;
            PageSize = pageSize;
            Columns = columns?.Select( x => new DataGridColumnInfo( x.Field, x.Filter?.SearchValue, x.CurrentDirection, x.ColumnType ) );
            CancellationToken = cancellationToken;
        }

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
        public MultiSelectEventArgs( TItem item, bool selected )
        {
            Item = item;
            Selected = selected;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public TItem Item { get; }

        /// <summary>
        /// Returns true if the row is selected.
        /// </summary>
        public bool Selected { get; }
    }
}