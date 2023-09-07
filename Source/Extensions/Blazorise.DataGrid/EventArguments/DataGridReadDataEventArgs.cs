#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Blazorise.Extensions;
#endregion

namespace Blazorise.DataGrid;

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
    public DataGridReadDataEventArgs(
        DataGridReadDataMode readDataMode,
        IEnumerable<DataGridColumn<TItem>> columns,
        IList<DataGridColumn<TItem>> sortByColumns,
        CancellationToken cancellationToken = default )
        : this( readDataMode, columns, sortByColumns, 0, 0, 0, 0, cancellationToken )
    {
    }

    /// <summary>
    /// Initializes a new instance of read-data event argument.
    /// </summary>
    /// <param name="readDataMode">ReadData Mode.</param>
    /// <param name="columns">List of all the columns in the grid.</param>
    /// <param name="sortByColumns">List of all the columns by which we're sorting the grid.</param>        
    /// <param name="page">Page number at the moment of initialization.</param>
    /// <param name="pageSize">Maximum number of items per page.</param>
    /// <param name="virtualizeOffset">Requested data start index by Virtualize.</param>
    /// <param name="virtualizeCount">Max number of items requested by Virtualize.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public DataGridReadDataEventArgs(
        DataGridReadDataMode readDataMode,
        IEnumerable<DataGridColumn<TItem>> columns,
        IList<DataGridColumn<TItem>> sortByColumns,
        int page,
        int pageSize,
        int virtualizeOffset,
        int virtualizeCount,
        CancellationToken cancellationToken = default )
    {
        Page = page;
        PageSize = pageSize;
        Columns = columns?.Select( x => new DataGridColumnInfo(
            x.Field,
            x.Filter?.SearchValue,
            x.CurrentSortDirection,
            sortByColumns?.FirstOrDefault( sortCol => sortCol.IsEqual( x ) )?.SortOrder ?? -1,
            x.ColumnType,
            x.GetFieldToSort(),
            x.GetFilterMethod() ));
        CancellationToken = cancellationToken;
        VirtualizeOffset = virtualizeOffset;
        VirtualizeCount = virtualizeCount;
        ReadDataMode = readDataMode;
    }

    /// <summary>
    /// Gets the ReadData Mode.
    /// </summary>
    public DataGridReadDataMode ReadDataMode { get; }

    /// <summary>
    /// Gets the requested data start index by Virtualize.
    /// </summary>
    public int VirtualizeOffset { get; }

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