#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a string select property.
/// </summary>
public sealed class PropertyGridStringSelectProperty : PropertyGridProperty<string>
{
    #region Constructors

    /// <summary>
    /// Initializes a string select property.
    /// </summary>
    public PropertyGridStringSelectProperty( string key, string label, string value, IReadOnlyList<PropertyGridSelectOption<string>> options )
        : base( key, label, value )
    {
        Options = options ?? [];
    }

    #endregion

    #region Methods

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
        => parameters[nameof( PropertyGridStringSelectItem.Options )] = Options;

    #endregion

    #region Properties

    internal override Type AtomicComponentType => typeof( PropertyGridStringSelectItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<string>> Options { get; }

    #endregion
}