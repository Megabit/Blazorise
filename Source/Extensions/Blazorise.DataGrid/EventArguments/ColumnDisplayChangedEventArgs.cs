namespace Blazorise.DataGrid;

/// <summary>
/// Represents the arguments for the event that is raised when the display state of a data grid column changes.
/// </summary>
/// <typeparam name="TItem">The type of the item in the data grid column.</typeparam>
public class ColumnDisplayChangedEventArgs<TItem>
{
    /// <summary>
    /// Gets or sets the column whose display state has changed.
    /// </summary>
    /// <value>The <see cref="DataGridColumn{TItem}"/> that has changed.</value>
    public DataGridColumn<TItem> Column { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the column is displayed.
    /// </summary>
    /// <value><c>true</c> if the column is to be displayed; otherwise, <c>false</c>.</value>
    public bool Display { get; set; }

    /// <summary>
    /// Gets the display order of the column.
    /// </summary>
    public int DisplayOrder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnDisplayChangedEventArgs{TItem}"/> class.
    /// </summary>
    /// <param name="column">The column whose display state is changing.</param>
    /// <param name="display">if set to <c>true</c>, the column will be displayed; otherwise, it will be hidden.</param>
    public ColumnDisplayChangedEventArgs( DataGridColumn<TItem> column, bool display, int displayOrder )
    {
        Column = column;
        Display = display;
        DisplayOrder = displayOrder;
    }
}
