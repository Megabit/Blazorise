#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents information about a row in a data grid.
/// </summary>
/// <typeparam name="TItem">The type of the item represented by the row.</typeparam>
public class DataGridRowInfo<TItem>
{
    private bool hasDetailRow;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridRowInfo{TItem}"/> class.
    /// </summary>
    /// <param name="item">The item associated with the row.</param>
    /// <param name="columns">The collection of columns in the row.</param>
    public DataGridRowInfo( TItem item, IEnumerable<DataGridColumn<TItem>> columns )
    {
        Item = item;
        Columns = columns;
    }

    /// <summary>
    /// Gets the collection of columns in the row.
    /// </summary>
    public IEnumerable<DataGridColumn<TItem>> Columns { get; }

    /// <summary>
    /// Gets the table row associated with this row info.
    /// </summary>
    public TableRow TableRow { get; private set; }

    /// <summary>
    /// Gets the item associated with the row.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets a value indicating whether the row has a detail row.
    /// </summary>
    public bool HasDetailRow => hasDetailRow;

    /// <summary>
    /// Sets the detail row for the current row.
    /// </summary>
    /// <param name="hasDetailRow">Indicates whether the detail row is present.</param>
    /// <param name="toggleable">Indicates whether the detail row can be toggled.</param>
    public void SetRowDetail( bool hasDetailRow, bool toggleable )
        => this.hasDetailRow = ( toggleable && !this.hasDetailRow & hasDetailRow ) || ( !toggleable && hasDetailRow );

    /// <summary>
    /// Toggles the visibility of the detail row.
    /// </summary>
    public void ToggleDetailRow()
        => hasDetailRow = !hasDetailRow;

    /// <summary>
    /// Sets the table row associated with this row info.
    /// </summary>
    /// <param name="tableRow">The table row to associate with this row info.</param>
    internal void SetTableRow( TableRow tableRow )
        => TableRow = tableRow;
}