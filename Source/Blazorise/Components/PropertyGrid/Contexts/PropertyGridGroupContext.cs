using System.Collections.Generic;
using System.Linq;

namespace Blazorise;

/// <summary>
/// Provides context for a property grid group template.
/// </summary>
public sealed class PropertyGridGroupContext
{
    private readonly PropertyGridView owner;

    internal PropertyGridGroupContext( PropertyGridView owner, PropertyGridGroupDefinition group )
    {
        this.owner = owner;
        Group = group;
    }

    /// <summary>
    /// Gets the group definition.
    /// </summary>
    public PropertyGridGroupDefinition Group { get; }

    /// <summary>
    /// Gets contexts for the visible properties in the group.
    /// </summary>
    public IEnumerable<PropertyGridItemContext> Properties
        => Group.VisibleProperties.Select( property => new PropertyGridItemContext( owner, property ) );
}