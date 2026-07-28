using System;
using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Describes a boolean property.
/// </summary>
public sealed class PropertyGridBooleanProperty : PropertyGridProperty<bool>
{
    /// <summary>
    /// Initializes a boolean property.
    /// </summary>
    public PropertyGridBooleanProperty( string key, string label, bool value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets the text displayed for a true value.
    /// </summary>
    public string TrueText { get; set; } = "True";

    /// <summary>
    /// Gets or sets the text displayed for a false value.
    /// </summary>
    public string FalseText { get; set; } = "False";

    internal override Type AtomicComponentType => typeof( PropertyGridBooleanItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Boolean;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridBooleanItem.TrueText )] = TrueText;
        parameters[nameof( PropertyGridBooleanItem.FalseText )] = FalseText;
    }
}