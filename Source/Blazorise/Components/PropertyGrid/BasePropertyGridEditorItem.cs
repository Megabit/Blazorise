#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Provides shared parameters for built-in <see cref="PropertyGrid"/> editor items.
/// </summary>
public abstract class BasePropertyGridEditorItem : BaseComponent
{
    #region Properties

    /// <summary>
    /// Defines the property label.
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// Defines custom label content. When specified, it takes precedence over <see cref="Label"/>.
    /// </summary>
    [Parameter] public RenderFragment LabelContent { get; set; }

    /// <summary>
    /// Defines whether the property can be selected.
    /// </summary>
    [Parameter] public bool Selectable { get; set; }

    /// <summary>
    /// Defines whether the property is selected.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Occurs after the property selection state changes.
    /// </summary>
    [Parameter] public EventCallback<bool> SelectedChanged { get; set; }

    /// <summary>
    /// Identifies the element that describes the property.
    /// </summary>
    [Parameter] public string AriaDescribedBy { get; set; }

    /// <summary>
    /// Defines the property editor size.
    /// </summary>
    [Parameter] public Size Size { get; set; } = Size.Small;

    /// <summary>
    /// Defines whether the property value represents multiple different values.
    /// </summary>
    [Parameter] public bool Mixed { get; set; }

    /// <summary>
    /// Defines complete trailing content for the property editor.
    /// </summary>
    [Parameter] public RenderFragment ActionContent { get; set; }

    #endregion
}