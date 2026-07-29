#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Describes a group rendered by a <see cref="PropertyGridView"/>.
/// </summary>
public sealed class PropertyGridGroupDefinition
{
    #region Constructors

    /// <summary>
    /// Initializes a new property group.
    /// </summary>
    public PropertyGridGroupDefinition( string key, string title, IReadOnlyList<PropertyGridProperty> properties )
    {
        Key = key;
        Title = title;
        Properties = properties?.ToArray() ?? [];
        VisibleProperties = Properties
            .Where( property => property.Visible )
            .ToArray();
    }

    #endregion

    #region Properties

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
    /// Gets the properties rendered in the group.
    /// </summary>
    internal IReadOnlyList<PropertyGridProperty> VisibleProperties { get; }

    /// <summary>
    /// Gets whether the group is rendered.
    /// </summary>
    public bool Visible { get; init; } = true;

    /// <summary>
    /// Gets or sets whether the group is initially expanded.
    /// </summary>
    public bool Expanded { get; set; } = true;

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

    #endregion
}