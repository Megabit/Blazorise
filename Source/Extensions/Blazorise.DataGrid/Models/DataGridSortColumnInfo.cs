namespace Blazorise.DataGrid;

/// <summary>
/// A column sort definition
/// </summary>
public class DataGridSortColumnInfo
{
    /// <summary>
    /// Gets or sets the field name of the column used for sorting (see <see cref="BaseDataGridColumn{TItem}.Field"/>).
    /// </summary>
    public string Field { get; init; }

    /// <summary>
    /// Gets or sets direction of the column used for sorting (see <see cref="DataGridColumn{TItem}.SortDirection"/>).
    /// </summary>
    public SortDirection SortDirection { get; init; }

    /// <summary>
    /// Creates a new instance
    /// </summary>
    /// <param name="field"><inheritdoc cref="Field"/></param>
    /// <param name="sortDirection"><inheritdoc cref="SortDirection"/></param>
    public DataGridSortColumnInfo( string field, SortDirection sortDirection )
    {
        Field = field;
        SortDirection = sortDirection;
    }
}