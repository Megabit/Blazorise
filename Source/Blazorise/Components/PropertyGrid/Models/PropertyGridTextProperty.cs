#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a text property.
/// </summary>
public sealed class PropertyGridTextProperty : PropertyGridProperty<string>
{
    #region Constructors

    /// <summary>
    /// Initializes a text property.
    /// </summary>
    public PropertyGridTextProperty( string key, string label, string value )
        : base( key, label, value )
    {
    }

    #endregion

    #region Methods

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridTextItem.ReadOnly )] = ReadOnly;
        parameters[nameof( PropertyGridTextItem.Immediate )] = Immediate;
    }

    #endregion

    #region Properties

    internal override Type AtomicComponentType => typeof( PropertyGridTextItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Text;

    /// <summary>
    /// Gets or sets whether the text editor is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets whether changes are reported immediately.
    /// </summary>
    public bool? Immediate { get; set; }

    #endregion
}