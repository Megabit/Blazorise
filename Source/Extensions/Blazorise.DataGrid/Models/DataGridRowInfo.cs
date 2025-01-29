#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Represents information about a row in a data grid.
/// </summary>
/// <typeparam name="TItem">The type of the data item represented by the row.</typeparam>
public class DataGridRowInfo<TItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataGridRowInfo{TItem}"/> class.
    /// </summary>
    /// <param name="item">The data item associated with this row.</param>
    /// <param name="columns">The collection of columns associated with this row.</param>
    public DataGridRowInfo( TItem item, IEnumerable<DataGridColumn<TItem>> columns )
    {
        Item = item;
        Columns = columns;
    }

    /// <summary>
    /// Gets the collection of columns associated with this row.
    /// </summary>
    public IEnumerable<DataGridColumn<TItem>> Columns { get; }

    /// <summary>
    /// Gets the table row instance linked to this row information.
    /// </summary>
    internal TableRow TableRow { get; private set; }

    /// <summary>
    /// Gets the data item associated with this row.
    /// </summary>
    public TItem Item { get; }

    /// <summary>
    /// Gets a value indicating whether the detail row is currently expanded.
    /// </summary>
    public bool DetailRowExpanded { get; private set; }

    /// <summary>
    /// Configures the detail row for this row.
    /// </summary>
    /// <param name="hasDetailRow">Indicates whether a detail row is present.</param>
    /// <param name="toggleable">Indicates whether the detail row can be toggled.</param>
    public void SetRowDetail( bool hasDetailRow, bool toggleable )
        => DetailRowExpanded = ( toggleable && !DetailRowExpanded & hasDetailRow ) || ( !toggleable && hasDetailRow );

    /// <summary>
    /// Toggles the visibility state of the detail row.
    /// </summary>
    public void ToggleDetailRow()
        => DetailRowExpanded = !DetailRowExpanded;

    /// <summary>
    /// Associates a table row instance with this row information.
    /// </summary>
    /// <param name="tableRow">The table row instance to associate with this row.</param>
    internal void SetTableRow( TableRow tableRow )
        => TableRow = tableRow;
}