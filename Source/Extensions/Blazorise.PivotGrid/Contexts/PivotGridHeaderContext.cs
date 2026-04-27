#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Context passed to PivotGrid header templates.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class PivotGridHeaderContext<TItem>
{
    /// <summary>
    /// Initializes a new <see cref="PivotGridHeaderContext{TItem}"/>.
    /// </summary>
    public PivotGridHeaderContext( BasePivotGridField<TItem> field, string caption, int level, IReadOnlyList<object> path )
    {
        Field = field;
        Caption = caption;
        Level = level;
        Path = path;
    }

    /// <summary>
    /// Gets the field being rendered.
    /// </summary>
    public BasePivotGridField<TItem> Field { get; }

    /// <summary>
    /// Gets the header caption.
    /// </summary>
    public string Caption { get; }

    /// <summary>
    /// Gets the field level in the axis.
    /// </summary>
    public int Level { get; }

    /// <summary>
    /// Gets the current axis path.
    /// </summary>
    public IReadOnlyList<object> Path { get; }
}