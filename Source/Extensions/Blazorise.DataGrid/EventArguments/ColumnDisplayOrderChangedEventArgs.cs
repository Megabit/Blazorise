namespace Blazorise.DataGrid;

/// <summary>
/// Represents the arguments for the event that is raised when the display order of a data grid column changes.
/// </summary>
/// <typeparam name="TItem">The type of the item in the data grid column.</typeparam>
public class ColumnDisplayOrderChangedEventArgs<TItem>
{
    /// <summary>
    /// Gets or sets the column whose display state has changed.
    /// </summary>
    /// <value>The <see cref="DataGridColumn{TItem}"/> that has changed.</value>
    public DataGridColumn<TItem> Column { get; set; }

    /// <summary>
    /// Gets the display order of the column.
    /// </summary>
    public int DisplayOrder { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnDisplayChangedEventArgs{TItem}"/> class.
    /// </summary>
    /// <param name="column">The column whose display state is changing.</param>
    /// <param name="displayOrder">The new display order of the column.</param>
    public ColumnDisplayOrderChangedEventArgs( DataGridColumn<TItem> column, int displayOrder )
    {
        Column = column;
        DisplayOrder = displayOrder;
    }
}
