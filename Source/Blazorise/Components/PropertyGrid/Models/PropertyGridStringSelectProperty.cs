#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Rendering;
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

    internal override void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
    {
        builder.OpenComponent<PropertyGridStringSelectItem>( 0 );
        owner.AddAtomicComponentParameters( builder, this, selected, ariaDescribedBy );
        builder.AddAttribute( 15, nameof( PropertyGridStringSelectItem.Options ), Options );
        builder.CloseComponent();
    }

    #endregion

    #region Properties

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<string>> Options { get; }

    #endregion
}