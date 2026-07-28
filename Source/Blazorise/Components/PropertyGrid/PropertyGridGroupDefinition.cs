using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Describes a group rendered by a <see cref="PropertyGridView"/>.
/// </summary>
public sealed class PropertyGridGroupDefinition
{
    /// <summary>
    /// Initializes a new property group.
    /// </summary>
    public PropertyGridGroupDefinition( string key, string title, IReadOnlyList<PropertyGridProperty> properties )
    {
        Key = key;
        Title = title;
        Properties = properties ?? [];
    }

    /// <summary>
    /// Gets the stable group key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the group title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the properties in the group.
    /// </summary>
    public IReadOnlyList<PropertyGridProperty> Properties { get; }

    /// <summary>
    /// Gets or sets whether the group is rendered.
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets custom group classes.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Gets or sets custom group styles.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Gets or sets a property-specific group template.
    /// </summary>
    public RenderFragment<PropertyGridGroupContext> GroupTemplate { get; set; }

    /// <summary>
    /// Gets or sets a property-specific header template.
    /// </summary>
    public RenderFragment<PropertyGridGroupContext> HeaderTemplate { get; set; }
}
