#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

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
    public DataGridFilteredDataEventArgs( IEnumerable<TItem> filteredData, int filteredItems, long totalItems )
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
    public long TotalItems { get; }
}