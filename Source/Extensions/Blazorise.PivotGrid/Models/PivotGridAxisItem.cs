#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a row or column axis item in a prepared PivotGrid result.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridAxisItem<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridAxisItem{TItem}"/>.
    /// </summary>
    /// <param name="values">Field values that identify the axis item path.</param>
    /// <param name="items">Source items included in the axis item.</param>
    /// <param name="level">Axis hierarchy level.</param>
    /// <param name="isTotal">Whether the axis item represents a subtotal.</param>
    /// <param name="isGrandTotal">Whether the axis item represents the grand total.</param>
    public PivotGridAxisItem( IReadOnlyList<object> values, IReadOnlyList<TItem> items, int level, bool isTotal, bool isGrandTotal )
    {
        Values = values;
        Items = items;
        Level = level;
        IsTotal = isTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets field values that identify the axis item path.
    /// </summary>
    public IReadOnlyList<object> Values { get; }

    /// <summary>
    /// Gets source items included in the axis item.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Gets the axis hierarchy level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets whether the axis item represents a subtotal.
    /// </summary>
    public bool IsTotal { get; }

    /// <summary>
    /// Gets whether the axis item represents the grand total.
    /// </summary>
    public bool IsGrandTotal { get; }
}