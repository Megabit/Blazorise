using System;
using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Describes a string select property.
/// </summary>
public sealed class PropertyGridStringSelectProperty : PropertyGridProperty<string>
{
    /// <summary>
    /// Initializes a string select property.
    /// </summary>
    public PropertyGridStringSelectProperty( string key, string label, string value, IReadOnlyList<PropertyGridSelectOption<string>> options )
        : base( key, label, value )
    {
        Options = options ?? [];
    }

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<string>> Options { get; }

    internal override Type AtomicComponentType => typeof( PropertyGridStringSelectItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
        => parameters[nameof( PropertyGridStringSelectItem.Options )] = Options;
}