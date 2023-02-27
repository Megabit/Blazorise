#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides information about the DataGrid's sort order
/// </summary>
public class DataGridSortOrderChangedEventArgs
{
    /// <inheritdoc cref="DataGrid{TItem}.SortMode"/>
    public DataGridSortMode SortMode { get; init; }

    /// <summary>
    /// A collection of <see cref="SortOrder"/> items. Contains all columns by which to sort.
    /// </summary>
    /// <remarks>Returns an empty list if no sorting is applied.</remarks>
    public IReadOnlyCollection<DataGridSortInfo> SortOrder { get; init; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sortMode"><inheritdoc cref="SortMode"/></param>
    /// <param name="sortOrder"><inheritdoc cref="SortOrder"/></param>
    public DataGridSortOrderChangedEventArgs( DataGridSortMode sortMode, IReadOnlyCollection<DataGridSortInfo> sortOrder )
    {
        SortMode = sortMode;
        SortOrder = sortOrder;
    }
}