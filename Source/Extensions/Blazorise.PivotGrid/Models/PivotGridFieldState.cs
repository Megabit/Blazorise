#region Using directives
using System;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Describes a PivotGrid field in the runtime layout state.
/// </summary>
public class PivotGridFieldState
{
    /// <summary>
    /// Gets or sets the field path.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Gets or sets the field caption.
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Gets or sets the field value type.
    /// </summary>
    public Type FieldType { get; set; }

    /// <summary>
    /// Gets or sets the runtime field area.
    /// </summary>
    public PivotGridFieldArea Area { get; set; }

    /// <summary>
    /// Gets or sets the aggregate function used when <see cref="Area"/> is <see cref="PivotGridFieldArea.Aggregate"/>.
    /// </summary>
    public PivotGridAggregateFunction AggregateFunction { get; set; } = PivotGridAggregateFunction.Sum;

    /// <summary>
    /// Gets or sets the selected filter value key when <see cref="Area"/> is <see cref="PivotGridFieldArea.Filter"/>.
    /// </summary>
    public string FilterValueKey { get; set; }
}