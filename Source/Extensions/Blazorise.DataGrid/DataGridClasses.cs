using Blazorise;

namespace Blazorise.DataGrid;

/// <summary>
/// Component classes for <see cref="DataGrid{TItem}"/>.
/// </summary>
public sealed record class DataGridClasses : ComponentClasses
{
    /// <summary>
    /// Targets the form wrapper element.
    /// </summary>
    public string Form { get; init; }

    /// <summary>
    /// Targets the pagination wrapper element.
    /// </summary>
    public string Pagination { get; init; }

    /// <summary>
    /// Targets the column drop zone element.
    /// </summary>
    public string ColumnDropZone { get; init; }
}

/// <summary>
/// Component styles for <see cref="DataGrid{TItem}"/>.
/// </summary>
public sealed record class DataGridStyles : ComponentStyles
{
    /// <summary>
    /// Targets the form wrapper element.
    /// </summary>
    public string Form { get; init; }

    /// <summary>
    /// Targets the pagination wrapper element.
    /// </summary>
    public string Pagination { get; init; }

    /// <summary>
    /// Targets the column drop zone element.
    /// </summary>
    public string ColumnDropZone { get; init; }
}