#region Using directives
using Microsoft.AspNetCore.Components.Rendering;
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

    internal override void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
    {
        builder.OpenComponent<PropertyGridTextItem>( 0 );
        owner.AddAtomicComponentParameters( builder, this, selected, ariaDescribedBy );
        builder.AddAttribute( 15, nameof( PropertyGridTextItem.ReadOnly ), ReadOnly );
        builder.AddAttribute( 16, nameof( PropertyGridTextItem.Immediate ), Immediate );
        builder.CloseComponent();
    }

    #endregion

    #region Properties

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