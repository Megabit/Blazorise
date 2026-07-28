using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazorise;

/// <summary>
/// Describes the groups and properties rendered by a <see cref="PropertyGridView"/>.
/// </summary>
public sealed class PropertyGridSchema
{
    private readonly IReadOnlyList<PropertyGridGroupDefinition> categorizedGroups;

    private readonly IReadOnlyList<PropertyGridGroupDefinition> alphabeticalGroups;

    /// <summary>
    /// Initializes a new property grid schema.
    /// </summary>
    /// <param name="groups">The property groups.</param>
    public PropertyGridSchema( IReadOnlyList<PropertyGridGroupDefinition> groups )
    {
        Groups = groups?.ToArray() ?? [];
        categorizedGroups = Groups
            .Where( group => group.Visible )
            .ToArray();

        PropertyGridProperty[] alphabeticalProperties = categorizedGroups
            .SelectMany( group => group.VisibleProperties )
            .OrderBy( property => property.Label, StringComparer.CurrentCultureIgnoreCase )
            .ThenBy( property => property.Key, StringComparer.Ordinal )
            .ToArray();

        alphabeticalGroups = alphabeticalProperties.Length == 0
            ? []
            : [new PropertyGridGroupDefinition( "__alphabetical", null, alphabeticalProperties )];
    }

    internal IReadOnlyList<PropertyGridGroupDefinition> GetRenderedGroups( PropertyGridViewMode viewMode )
        => viewMode == PropertyGridViewMode.Alphabetical
            ? alphabeticalGroups
            : categorizedGroups;

    /// <summary>
    /// Gets the property groups.
    /// </summary>
    public IReadOnlyList<PropertyGridGroupDefinition> Groups { get; }
}