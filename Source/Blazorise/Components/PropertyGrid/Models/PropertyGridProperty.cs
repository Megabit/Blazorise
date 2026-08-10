#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise;

/// <summary>
/// Base definition for a property rendered by a <see cref="PropertyGridView"/>.
/// </summary>
public abstract class PropertyGridProperty
{
    #region Constructors

    /// <summary>
    /// Initializes a property definition.
    /// </summary>
    protected PropertyGridProperty( string key, string label )
    {
        Key = key;
        Label = label;
    }

    #endregion

    #region Methods

    internal abstract object CreateValueChangedCallback( PropertyGridView owner );

    internal virtual void RenderAtomicComponent( RenderTreeBuilder builder, PropertyGridView owner, bool selected, string ariaDescribedBy )
        => throw new InvalidOperationException( $"Property '{GetType().FullName}' requires an editor or item template." );

    #endregion

    #region Properties

    internal virtual PropertyGridEditorType EditorType => PropertyGridEditorType.None;

    /// <summary>
    /// Gets the stable property key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the property label.
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// Gets or sets the property description shown in the help panel.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets the runtime property value.
    /// </summary>
    public abstract object Value { get; }

    /// <summary>
    /// Gets the runtime property value type.
    /// </summary>
    public abstract Type ValueType { get; }

    /// <summary>
    /// Gets whether the property is rendered.
    /// </summary>
    public bool Visible { get; init; } = true;

    /// <summary>
    /// Gets or sets whether the property represents multiple different values.
    /// </summary>
    public bool Mixed { get; set; }

    /// <summary>
    /// Gets or sets the editor size.
    /// </summary>
    public Size Size { get; set; } = Size.Small;

    /// <summary>
    /// Gets or sets custom property classes.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets custom property styles.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets additional attributes applied to the property item.
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Gets or sets the optional property action.
    /// </summary>
    public PropertyGridAction Action { get; set; }

    /// <summary>
    /// Gets or sets a property-specific full item template.
    /// </summary>
    public RenderFragment<PropertyGridItemContext> ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets a property-specific label template.
    /// </summary>
    public RenderFragment<PropertyGridLabelContext> LabelTemplate { get; set; }

    /// <summary>
    /// Gets or sets a property-specific editor template.
    /// </summary>
    public RenderFragment<PropertyGridEditorContext> EditorTemplate { get; set; }

    #endregion
}