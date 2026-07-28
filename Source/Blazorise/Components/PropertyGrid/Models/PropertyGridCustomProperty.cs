#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a custom property rendered by its editor or item template.
/// </summary>
public sealed class PropertyGridCustomProperty<TValue> : PropertyGridProperty<TValue>
{
    #region Constructors

    /// <summary>
    /// Initializes a custom property.
    /// </summary>
    public PropertyGridCustomProperty( string key, string label, TValue value, RenderFragment<PropertyGridEditorContext> editorTemplate )
        : base( key, label, value )
    {
        EditorTemplate = editorTemplate ?? throw new ArgumentNullException( nameof( editorTemplate ) );
    }

    #endregion
}