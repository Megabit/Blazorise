using System;
using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Describes a value-type select property.
/// </summary>
public sealed class PropertyGridSelectProperty<TValue> : PropertyGridProperty<TValue>
    where TValue : struct
{
    /// <summary>
    /// Initializes a value-type select property.
    /// </summary>
    public PropertyGridSelectProperty( string key, string label, TValue value, IReadOnlyList<PropertyGridSelectOption<TValue>> options )
        : base( key, label, value )
    {
        Options = options ?? [];
    }

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<TValue>> Options { get; }

    internal override Type AtomicComponentType => typeof( PropertyGridSelectItem<TValue> );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
        => parameters[nameof( PropertyGridSelectItem<TValue>.Options )] = Options;
}