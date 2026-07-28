using System.Collections.Generic;

namespace Blazorise;

/// <summary>
/// Describes the groups and properties rendered by a <see cref="PropertyGridView"/>.
/// </summary>
public sealed class PropertyGridSchema
{
    /// <summary>
    /// Initializes a new property grid schema.
    /// </summary>
    /// <param name="groups">The property groups.</param>
    public PropertyGridSchema( IReadOnlyList<PropertyGridGroupDefinition> groups )
    {
        Groups = groups ?? [];
    }

    /// <summary>
    /// Gets the property groups.
    /// </summary>
    public IReadOnlyList<PropertyGridGroupDefinition> Groups { get; }
}
