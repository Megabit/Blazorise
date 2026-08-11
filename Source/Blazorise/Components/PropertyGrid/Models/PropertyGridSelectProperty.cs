#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Rendering;
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

    internal override void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
    {
        builder.OpenComponent<PropertyGridSelectItem<TValue>>( 0 );
        owner.AddAtomicComponentParameters( builder, this, selected, ariaDescribedBy );
        builder.AddAttribute( 15, nameof( PropertyGridSelectItem<TValue>.Options ), Options );
        builder.CloseComponent();
    }

    #endregion

    #region Properties

    internal override PropertyGridEditorType EditorType => PropertyGridEditorType.Select;

    /// <summary>
    /// Gets the available options.
    /// </summary>
    public IReadOnlyList<PropertyGridSelectOption<TValue>> Options { get; }

    #endregion
}