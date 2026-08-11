#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
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

    internal override void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
    {
        builder.OpenComponent<PropertyGridBooleanItem>( 0 );
        owner.AddAtomicComponentParameters( builder, this, selected, ariaDescribedBy );
        builder.AddAttribute( 15, nameof( PropertyGridBooleanItem.TrueText ), TrueText );
        builder.AddAttribute( 16, nameof( PropertyGridBooleanItem.FalseText ), FalseText );
        builder.CloseComponent();
    }

    #endregion

    #region Properties

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