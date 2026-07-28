#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

/// <summary>
/// Provides context for a property grid group template.
/// </summary>
public sealed class PropertyGridGroupContext
{
    #region Members

    private readonly PropertyGridView owner;

    #endregion

    #region Constructors

    internal PropertyGridGroupContext( PropertyGridView owner, PropertyGridGroupDefinition group )
    {
        this.owner = owner;
        Group = group;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the group definition.
    /// </summary>
    public PropertyGridGroupDefinition Group { get; }

    /// <summary>
    /// Gets contexts for the visible properties in the group.
    /// </summary>
    public IEnumerable<PropertyGridItemContext> Properties
        => Group.VisibleProperties.Select( property => new PropertyGridItemContext( owner, property ) );

    #endregion
}