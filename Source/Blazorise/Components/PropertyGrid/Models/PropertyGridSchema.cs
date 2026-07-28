#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise;

/// <summary>
/// Describes the groups and properties rendered by a <see cref="PropertyGridView"/>.
/// </summary>
public sealed class PropertyGridSchema
{
    #region Members

    private readonly IReadOnlyList<PropertyGridGroupDefinition> categorizedGroups;

    private readonly IReadOnlyList<PropertyGridGroupDefinition> alphabeticalGroups;

    #endregion

    #region Constructors

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

    #endregion

    #region Methods

    internal IReadOnlyList<PropertyGridGroupDefinition> GetRenderedGroups( PropertyGridViewMode viewMode )
        => viewMode == PropertyGridViewMode.Alphabetical
            ? alphabeticalGroups
            : categorizedGroups;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the property groups.
    /// </summary>
    public IReadOnlyList<PropertyGridGroupDefinition> Groups { get; }

    #endregion
}