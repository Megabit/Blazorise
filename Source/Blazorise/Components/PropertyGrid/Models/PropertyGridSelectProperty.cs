#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a value-type select property.
/// </summary>
public sealed class PropertyGridSelectProperty<TValue> : PropertyGridProperty<TValue>
    where TValue : struct
{
    #region Constructors

    /// <summary>
    /// Initializes a value-type select property.
    /// </summary>
    public PropertyGridSelectProperty( string key, string label, TValue value, IReadOnlyList<PropertyGridSelectOption<TValue>> options )
        : base( key, label, value )
    {
        Options = options ?? [];
    }

    #endregion

    #region Methods

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
        => parameters[nameof( PropertyGridSelectItem<TValue>.Options )] = Options;

    #endregion

    #region Properties

    internal override Type AtomicComponentType => typeof( PropertyGridSelectItem<TValue> );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<TValue>> Options { get; }

    #endregion
}