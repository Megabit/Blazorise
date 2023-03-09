#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides information about the DataGrid's sort order
/// </summary>
public class DataGridSortDefinitionChangedEventArgs
{
    /// <inheritdoc cref="DataGrid{TItem}.SortMode"/>
    public DataGridSortMode SortMode { get; init; }

    /// <summary>
    /// A collection of <see cref="SortDefinition"/> items. Contains all columns by which to sort.
    /// </summary>
    /// <remarks>Returns an empty list if no sorting is applied.</remarks>
    public IReadOnlyCollection<DataGridSortDefinition> SortDefinition { get; init; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="sortMode"><inheritdoc cref="SortMode"/></param>
    /// <param name="sortDefinition"><inheritdoc cref="SortDefinition"/></param>
    public DataGridSortDefinitionChangedEventArgs( DataGridSortMode sortMode, IReadOnlyCollection<DataGridSortDefinition> sortDefinition )
    {
        SortMode = sortMode;
        SortDefinition = sortDefinition;
    }
}