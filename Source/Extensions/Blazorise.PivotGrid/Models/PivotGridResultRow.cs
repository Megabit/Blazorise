#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a PivotGrid result row.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridResultRow<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridResultRow{TItem}"/>.
    /// </summary>
    /// <param name="row">Row axis item.</param>
    /// <param name="cells">Aggregate value cells for the row.</param>
    public PivotGridResultRow( PivotGridAxisItem<TItem> row, IReadOnlyList<PivotGridCell<TItem>> cells )
    {
        Row = row;
        Cells = cells;
    }

    /// <summary>
    /// Gets the row axis item.
    /// </summary>
    public PivotGridAxisItem<TItem> Row { get; }

    /// <summary>
    /// Gets aggregate value cells for the row.
    /// </summary>
    public IReadOnlyList<PivotGridCell<TItem>> Cells { get; }
}