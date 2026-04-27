#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Context passed to PivotGrid row header templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridRowHeaderContext<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridRowHeaderContext{TItem}"/>.
    /// </summary>
    public PivotGridRowHeaderContext( BasePivotGridField<TItem> field, object value, string formattedValue, string text, int level, IReadOnlyList<object> path, PivotGridAxisItem<TItem> row, IReadOnlyList<TItem> items, bool isTotal, bool isGrandTotal )
    {
        Field = field;
        Value = value;
        FormattedValue = formattedValue;
        Text = text;
        Level = level;
        Path = path;
        Row = row;
        Items = items;
        IsTotal = isTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets the row field being rendered.
    /// </summary>
    public BasePivotGridField<TItem> Field { get; }

    /// <summary>
    /// Gets the raw row field value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the formatted row field value.
    /// </summary>
    public string FormattedValue { get; }

    /// <summary>
    /// Gets the default row header text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the row level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets the full row path.
    /// </summary>
    public IReadOnlyList<object> Path { get; }

    /// <summary>
    /// Gets the row axis item.
    /// </summary>
    public PivotGridAxisItem<TItem> Row { get; }

    /// <summary>
    /// Gets the source items included in this row.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Gets whether the row header belongs to a subtotal row.
    /// </summary>
    public bool IsTotal { get; }

    /// <summary>
    /// Gets whether the row header belongs to the grand total row.
    /// </summary>
    public bool IsGrandTotal { get; }
}