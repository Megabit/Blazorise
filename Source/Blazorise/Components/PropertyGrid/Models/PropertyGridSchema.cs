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

    private readonly IReadOnlyDictionary<string, PropertyGridProperty> propertiesByKey;

    private string filteredSearchText;

    private IReadOnlyList<PropertyGridGroupDefinition> filteredCategorizedGroups;

    private IReadOnlyList<PropertyGridGroupDefinition> filteredAlphabeticalGroups;

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

        propertiesByKey = categorizedGroups
            .SelectMany( group => group.VisibleProperties )
            .GroupBy( property => property.Key, StringComparer.Ordinal )
            .ToDictionary( group => group.Key, group => group.First(), StringComparer.Ordinal );
    }

    #endregion

    #region Methods

    internal IReadOnlyList<PropertyGridGroupDefinition> GetRenderedGroups( PropertyGridViewMode viewMode, string searchText = null )
    {
        if ( string.IsNullOrWhiteSpace( searchText ) )
        {
            return viewMode == PropertyGridViewMode.Alphabetical
                ? alphabeticalGroups
                : categorizedGroups;
        }

        string normalizedSearchText = searchText.Trim();

        if ( !string.Equals( filteredSearchText, normalizedSearchText, StringComparison.CurrentCultureIgnoreCase ) )
        {
            filteredSearchText = normalizedSearchText;
            filteredCategorizedGroups = FilterGroups( categorizedGroups, normalizedSearchText );
            filteredAlphabeticalGroups = FilterGroups( alphabeticalGroups, normalizedSearchText );
        }

        return viewMode == PropertyGridViewMode.Alphabetical
            ? filteredAlphabeticalGroups
            : filteredCategorizedGroups;
    }

    internal PropertyGridProperty FindProperty( string key )
        => key is not null && propertiesByKey.TryGetValue( key, out PropertyGridProperty property )
            ? property
            : null;

    private static IReadOnlyList<PropertyGridGroupDefinition> FilterGroups( IReadOnlyList<PropertyGridGroupDefinition> groups, string searchText )
    {
        List<PropertyGridGroupDefinition> filteredGroups = [];

        foreach ( PropertyGridGroupDefinition group in groups )
        {
            PropertyGridProperty[] properties = group.VisibleProperties
                .Where( property => MatchesSearch( property, searchText ) )
                .ToArray();

            if ( properties.Length == 0 )
                continue;

            filteredGroups.Add( new PropertyGridGroupDefinition( group.Key, group.Title, properties )
            {
                Class = group.Class,
                Expanded = group.Expanded,
                GroupTemplate = group.GroupTemplate,
                HeaderTemplate = group.HeaderTemplate,
                Style = group.Style,
                Visible = group.Visible,
            } );
        }

        return filteredGroups;
    }

    private static bool MatchesSearch( PropertyGridProperty property, string searchText )
        => property.Label.Contains( searchText, StringComparison.CurrentCultureIgnoreCase )
            || property.Key.Contains( searchText, StringComparison.CurrentCultureIgnoreCase )
            || property.Description?.Contains( searchText, StringComparison.CurrentCultureIgnoreCase ) == true;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the property groups.
    /// </summary>
    public IReadOnlyList<PropertyGridGroupDefinition> Groups { get; }

    #endregion
}