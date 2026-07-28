using System;
using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Describes a numeric property.
/// </summary>
public sealed class PropertyGridNumericProperty<TValue> : PropertyGridProperty<TValue>
{
    /// <summary>
    /// Initializes a numeric property.
    /// </summary>
    public PropertyGridNumericProperty( string key, string label, TValue value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets the minimum accepted value.
    /// </summary>
    public double Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum accepted value.
    /// </summary>
    public double? Max { get; set; }

    /// <summary>
    /// Gets or sets the numeric increment.
    /// </summary>
    public decimal? Step { get; set; } = 1m;

    /// <summary>
    /// Gets or sets whether changes are reported immediately.
    /// </summary>
    public bool Immediate { get; set; } = true;

    internal override Type AtomicComponentType => typeof( PropertyGridNumericItem<TValue> );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Numeric;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridNumericItem<TValue>.Min )] = Min;
        parameters[nameof( PropertyGridNumericItem<TValue>.Max )] = Max;
        parameters[nameof( PropertyGridNumericItem<TValue>.Step )] = Step;
        parameters[nameof( PropertyGridNumericItem<TValue>.Immediate )] = Immediate;
    }
}