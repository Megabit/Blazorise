using System;
using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Describes a text property.
/// </summary>
public sealed class PropertyGridTextProperty : PropertyGridProperty<string>
{
    /// <summary>
    /// Initializes a text property.
    /// </summary>
    public PropertyGridTextProperty( string key, string label, string value )
        : base( key, label, value )
    {
    }

    /// <summary>
    /// Gets or sets whether the text editor is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets whether changes are reported immediately.
    /// </summary>
    public bool? Immediate { get; set; }

    internal override Type AtomicComponentType => typeof( PropertyGridTextItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Text;

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridTextItem.ReadOnly )] = ReadOnly;
        parameters[nameof( PropertyGridTextItem.Immediate )] = Immediate;
    }
}