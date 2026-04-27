#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Context passed to PivotGrid column header templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridColumnHeaderContext<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridColumnHeaderContext{TItem}"/>.
    /// </summary>
    public PivotGridColumnHeaderContext( BasePivotGridField<TItem> field, object value, string formattedValue, string text, int level, IReadOnlyList<object> path, PivotGridAxisItem<TItem> column, IReadOnlyList<TItem> items, bool isTotal, bool isGrandTotal )
    {
        Field = field;
        Value = value;
        FormattedValue = formattedValue;
        Text = text;
        Level = level;
        Path = path;
        Column = column;
        Items = items;
        IsTotal = isTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets the column field being rendered.
    /// </summary>
    public BasePivotGridField<TItem> Field { get; }

    /// <summary>
    /// Gets the raw column field value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the formatted column field value.
    /// </summary>
    public string FormattedValue { get; }

    /// <summary>
    /// Gets the default column header text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the column level.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets the full column path.
    /// </summary>
    public IReadOnlyList<object> Path { get; }

    /// <summary>
    /// Gets the column axis item.
    /// </summary>
    public PivotGridAxisItem<TItem> Column { get; }

    /// <summary>
    /// Gets the source items included in this column.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Gets whether the column header belongs to a subtotal column.
    /// </summary>
    public bool IsTotal { get; }

    /// <summary>
    /// Gets whether the column header belongs to the grand total column.
    /// </summary>
    public bool IsGrandTotal { get; }
}