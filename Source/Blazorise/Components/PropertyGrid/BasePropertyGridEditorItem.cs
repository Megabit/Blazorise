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
    /// Defines whether the property editor action is shown.
    /// </summary>
    [Parameter] public bool ShowAction { get; set; } = true;

    /// <summary>
    /// Defines whether the property editor action is disabled.
    /// </summary>
    [Parameter] public bool ActionDisabled { get; set; }

    /// <summary>
    /// Defines the property editor action color.
    /// </summary>
    [Parameter] public Color ActionColor { get; set; } = Color.Light;

    /// <summary>
    /// Defines the property editor action icon.
    /// </summary>
    [Parameter] public object ActionIcon { get; set; }

    /// <summary>
    /// Defines the property editor action text.
    /// </summary>
    [Parameter] public string ActionText { get; set; }

    /// <summary>
    /// Defines the accessible title for the property editor action.
    /// </summary>
    [Parameter] public string ActionTitle { get; set; }

    /// <summary>
    /// Defines custom content for the property editor action.
    /// </summary>
    [Parameter] public RenderFragment ActionContent { get; set; }

    /// <summary>
    /// Defines a complete template for the trailing property action.
    /// </summary>
    [Parameter] public RenderFragment ActionTemplate { get; set; }

    /// <summary>
    /// Occurs when the property editor action is clicked.
    /// </summary>
    [Parameter] public EventCallback ActionClicked { get; set; }

    #endregion
}