#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides information about the DataGrid's sort order
/// </summary>
public class DataGridSortingChangedEventArgs
{
    /// <inheritdoc cref="DataGrid{TItem}.SortMode"/>
    public DataGridSortMode SortMode { get; init; }

    /// <summary>
    /// Contains all columns by which to sort.
    /// </summary>
    /// <remarks>Returns an empty list if no sorting is applied.</remarks>
    public IReadOnlyCollection<DataGridSortColumnInfo> Columns { get; init; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sortMode"><inheritdoc cref="SortMode"/></param>
    /// <param name="columns"><inheritdoc cref="Columns"/></param>
    public DataGridSortingChangedEventArgs( DataGridSortMode sortMode, IReadOnlyCollection<DataGridSortColumnInfo> columns )
    {
        SortMode = sortMode;
        Columns = columns;
    }
}