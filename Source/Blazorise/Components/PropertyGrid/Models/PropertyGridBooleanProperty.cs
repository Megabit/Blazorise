#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a boolean property.
/// </summary>
public sealed class PropertyGridBooleanProperty : PropertyGridProperty<bool>
{
    #region Constructors

    /// <summary>
    /// Initializes a boolean property.
    /// </summary>
    public PropertyGridBooleanProperty( string key, string label, bool value )
        : base( key, label, value )
    {
    }

    #endregion

    #region Methods

    internal override void AddAtomicComponentParameters( IDictionary<string, object> parameters )
    {
        parameters[nameof( PropertyGridBooleanItem.TrueText )] = TrueText;
        parameters[nameof( PropertyGridBooleanItem.FalseText )] = FalseText;
    }

    #endregion

    #region Properties

    internal override Type AtomicComponentType => typeof( PropertyGridBooleanItem );

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Boolean;

    /// <summary>
    /// Gets or sets the text displayed for a true value.
    /// </summary>
    public string TrueText { get; set; } = "True";

    /// <summary>
    /// Gets or sets the text displayed for a false value.
    /// </summary>
    public string FalseText { get; set; } = "False";

    #endregion
}