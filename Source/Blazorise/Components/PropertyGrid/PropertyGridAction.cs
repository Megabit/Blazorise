using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Describes an action displayed next to a property editor.
/// </summary>
public sealed class PropertyGridAction
{
    /// <summary>
    /// Initializes a property action.
    /// </summary>
    public PropertyGridAction( string name )
    {
        Name = name;
    }

    /// <summary>
    /// Gets the action name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets whether the action is visible.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the action is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets the default action color.
    /// </summary>
    public Color Color { get; set; } = Color.Light;

    /// <summary>
    /// Gets or sets the default action icon.
    /// </summary>
    public object Icon { get; set; }

    /// <summary>
    /// Gets or sets the default action text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the accessible action title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets a property-specific complete action template.
    /// </summary>
    public RenderFragment<PropertyGridActionContext> ActionTemplate { get; set; }

}
