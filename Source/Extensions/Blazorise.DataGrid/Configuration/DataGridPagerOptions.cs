namespace Blazorise.DataGrid;

/// <summary>
/// Defines the options for <see cref="DataGrid{TItem}"/> pager.
/// </summary>
public class DataGridPagerOptions
{
    /// <summary>
    /// Configures the pager buttons size.
    /// </summary>
    public Size ButtonSize { get; set; }

    /// <summary>
    /// Button Row Position. 
    /// </summary>
    public PagerElementPosition ButtonRowPosition { get; set; } = PagerElementPosition.Default;

    /// <summary>
    /// Column Chooser Position. 
    /// </summary>
    public PagerElementPosition ColumnChooserPosition { get; set; } = PagerElementPosition.Default;

    /// <summary>
    /// Pagination Position. 
    /// </summary>
    public PagerElementPosition PaginationPosition { get; set; } = PagerElementPosition.Default;

    /// <summary>
    /// Total Items Position. 
    /// </summary>
    public PagerElementPosition TotalItemsPosition { get; set; } = PagerElementPosition.Default;
}