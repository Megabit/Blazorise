#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a CSS color property.
/// </summary>
public sealed class PropertyGridColorProperty : PropertyGridProperty<string>
{
    #region Constructors

    /// <summary>
    /// Initializes a CSS color property.
    /// </summary>
    public PropertyGridColorProperty( string key, string label, string value )
        : base( key, label, value )
    {
    }

    #endregion

    #region Methods

    internal override void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
    {
        builder.OpenComponent<PropertyGridColorItem>( 0 );
        owner.AddAtomicComponentParameters( builder, this, selected, ariaDescribedBy );
        builder.AddAttribute( 15, nameof( PropertyGridColorItem.NamedColors ), NamedColors );
        builder.AddAttribute( 16, nameof( PropertyGridColorItem.Clearable ), Clearable );
        builder.AddAttribute( 17, nameof( PropertyGridColorItem.ClearTitle ), ClearTitle );
        builder.CloseComponent();
    }

    #endregion

    #region Properties

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Color;

    /// <summary>
    /// Gets or sets the named color options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<string>> NamedColors { get; set; }

    /// <summary>
    /// Gets or sets whether the color can be cleared.
    /// </summary>
    public bool Clearable { get; set; } = true;

    /// <summary>
    /// Gets or sets the accessible title for the clear action.
    /// </summary>
    public string ClearTitle { get; set; } = "Clear color";

    #endregion
}