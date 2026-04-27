#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Context passed to row and column field value templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridFieldValueContext<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridFieldValueContext{TItem}"/>.
    /// </summary>
    public PivotGridFieldValueContext( BasePivotGridField<TItem> field, object value, string formattedValue, int level, IReadOnlyList<object> path, bool isTotal, bool isGrandTotal )
    {
        Field = field;
        Value = value;
        FormattedValue = formattedValue;
        Level = level;
        Path = path;
        IsTotal = isTotal;
        IsGrandTotal = isGrandTotal;
    }

    /// <summary>
    /// Gets the field being rendered.
    /// </summary>
    public BasePivotGridField<TItem> Field { get; }

    /// <summary>
    /// Gets the raw field value.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Gets the formatted field value.
    /// </summary>
    public string FormattedValue { get; }

    /// <summary>
    /// Gets the field level in the axis.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets the full axis path for the rendered value.
    /// </summary>
    public IReadOnlyList<object> Path { get; }

    /// <summary>
    /// Gets whether this value represents a subtotal.
    /// </summary>
    public bool IsTotal { get; }

    /// <summary>
    /// Gets whether this value represents a grand total.
    /// </summary>
    public bool IsGrandTotal { get; }
}