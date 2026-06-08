#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDefinitionHelper
{
    #region Methods

    internal static ReportElementDefinition CreateElementFromToolbox( ReportElementType elementType, string text, double x, double y )
    {
        var definition = new ReportElementDefinition
        {
            Name = elementType.ToString(),
            Type = elementType,
            X = x,
            Y = y,
            Width = 160,
            Height = 24,
        };

        switch ( elementType )
        {
            case ReportElementType.Text:
                definition.Name = text;
                definition.Text = text;
                break;
            case ReportElementType.Image:
                definition.Height = 96;
                definition.Text = "Image";
                break;
            case ReportElementType.Line:
                definition.Height = 1;
                break;
            case ReportElementType.Rectangle:
                definition.Height = 64;
                break;
        }

        return definition;
    }

    internal static (string DataSourceName, string FieldName) NormalizeFieldBindingForSection( ReportDefinition definition, ReportSectionDefinition section, string dataSourceName, string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return (dataSourceName, fieldName);

        string fieldPath = ReportExpressionFormatter.FormatFieldPath( dataSourceName, fieldName );

        if ( section is not null && !string.IsNullOrWhiteSpace( section.DataSource ) )
        {
            string sectionDataSource = section.DataSource.Trim();
            string sectionPrefix = $"{sectionDataSource}.";

            if ( fieldPath.StartsWith( sectionPrefix, StringComparison.OrdinalIgnoreCase ) )
                return (null, fieldPath[sectionPrefix.Length..]);

            if ( fieldName.StartsWith( sectionPrefix, StringComparison.OrdinalIgnoreCase ) )
                return (null, fieldName[sectionPrefix.Length..]);

            if ( string.Equals( dataSourceName, sectionDataSource, StringComparison.OrdinalIgnoreCase ) )
                return (null, fieldName);
        }

        string rootDataSourceName = definition?.DataSources.FirstOrDefault()?.Name;

        if ( !string.IsNullOrWhiteSpace( rootDataSourceName ) && string.Equals( dataSourceName, rootDataSourceName, StringComparison.OrdinalIgnoreCase ) )
            return (null, fieldName);

        return (dataSourceName, fieldName);
    }

    internal static string CreateUniqueSectionName( ReportDefinition definition, string baseName )
    {
        if ( string.IsNullOrWhiteSpace( baseName ) )
            baseName = "Band copy";

        if ( definition is null )
            return baseName;

        var name = baseName;
        var index = 2;

        while ( definition.Sections.Any( section => string.Equals( section.Name, name, StringComparison.OrdinalIgnoreCase ) ) )
        {
            name = $"{baseName} {index}";
            index++;
        }

        return name;
    }

    internal static string GetSectionDisplayName( ReportSectionDefinition section )
    {
        return string.IsNullOrWhiteSpace( section.Name ) ? GetSectionTypeDisplayName( section.Type ) : section.Name;
    }

    internal static string GetSectionTypeDisplayName( ReportSectionType type )
    {
        return type switch
        {
            ReportSectionType.Header => "Report Header",
            ReportSectionType.PageHeader => "Page Header",
            ReportSectionType.Detail => "Detail",
            ReportSectionType.Group => "Group Header",
            ReportSectionType.GroupHeader => "Group Header",
            ReportSectionType.GroupFooter => "Group Footer",
            ReportSectionType.PageFooter => "Page Footer",
            ReportSectionType.Footer => "Report Footer",
            _ => type.ToString(),
        };
    }

    internal static ReportTreeNodeKind GetElementTreeNodeKind( ReportElementType type )
    {
        return type switch
        {
            ReportElementType.Field => ReportTreeNodeKind.Field,
            ReportElementType.Table => ReportTreeNodeKind.Table,
            ReportElementType.Image => ReportTreeNodeKind.Image,
            ReportElementType.Line => ReportTreeNodeKind.Line,
            ReportElementType.Rectangle => ReportTreeNodeKind.Rectangle,
            ReportElementType.PageBreak => ReportTreeNodeKind.PageBreak,
            _ => ReportTreeNodeKind.Text,
        };
    }

    internal static string GetDataTypeDisplayName( Type dataType )
    {
        if ( dataType is null )
            return null;

        var nullableType = Nullable.GetUnderlyingType( dataType );

        return nullableType is null
            ? dataType.Name
            : $"{nullableType.Name}?";
    }

    internal static bool CanDeleteSection( ReportSectionDefinition section )
    {
        return section is not null && !section.Default;
    }

    internal static ReportDefinition EnsureDefinitionIds( ReportDefinition definition )
    {
        if ( definition is null )
            return null;

        var definitionIds = new HashSet<string>( StringComparer.Ordinal );
        var dataSourceIds = new HashSet<string>( StringComparer.Ordinal );
        var sectionIds = new HashSet<string>( StringComparer.Ordinal );
        var elementIds = new HashSet<string>( StringComparer.Ordinal );
        var columnIds = new HashSet<string>( StringComparer.Ordinal );

        definition.Id = EnsureUniqueDefinitionId( definition.Id, definitionIds );

        foreach ( var dataSource in definition.DataSources )
        {
            dataSource.Id = EnsureUniqueDefinitionId( dataSource.Id, dataSourceIds );
        }

        foreach ( var section in definition.Sections )
        {
            section.Id = EnsureUniqueDefinitionId( section.Id, sectionIds );

            foreach ( var element in section.Elements )
            {
                element.Id = EnsureUniqueDefinitionId( element.Id, elementIds );

                foreach ( var column in element.Columns )
                {
                    column.Id = EnsureUniqueDefinitionId( column.Id, columnIds );
                }
            }
        }

        return definition;
    }

    internal static string EnsureElementId( ReportElementDefinition element )
    {
        if ( element is null )
            return null;

        if ( string.IsNullOrWhiteSpace( element.Id ) )
            element.Id = CreateDefinitionId();

        return element.Id;
    }

    internal static string EnsureSectionId( ReportSectionDefinition section )
    {
        if ( section is null )
            return null;

        if ( string.IsNullOrWhiteSpace( section.Id ) )
            section.Id = CreateDefinitionId();

        return section.Id;
    }

    internal static bool TryFindElementLocation( ReportDefinition definition, string key, out int sectionIndex, out int elementIndex, out ReportElementDefinition element )
    {
        sectionIndex = -1;
        elementIndex = -1;
        element = null;

        if ( definition is null || string.IsNullOrWhiteSpace( key ) )
            return false;

        foreach ( var section in definition.Sections )
        {
            sectionIndex++;

            for ( var i = 0; i < section.Elements.Count; i++ )
            {
                if ( EnsureElementId( section.Elements[i] ) == key )
                {
                    elementIndex = i;
                    element = section.Elements[i];
                    return true;
                }
            }
        }

        return false;
    }

    internal static string CreateDefinitionId()
    {
        return Guid.NewGuid().ToString( "N" );
    }

    private static string EnsureUniqueDefinitionId( string id, HashSet<string> usedIds )
    {
        if ( string.IsNullOrWhiteSpace( id ) || !usedIds.Add( id ) )
        {
            do
            {
                id = CreateDefinitionId();
            }
            while ( !usedIds.Add( id ) );
        }

        return id;
    }

    #endregion
}