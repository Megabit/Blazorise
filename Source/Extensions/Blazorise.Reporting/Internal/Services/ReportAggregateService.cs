#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportAggregateService
{
    #region Methods

    internal IReadOnlyList<ReportAggregateSummaryLocation> GetSummaryLocations( ReportDefinition definition, int sourceSectionIndex )
    {
        var locations = new List<ReportAggregateSummaryLocation>
        {
            new()
            {
                TargetSectionIndex = -1,
                Name = "Grand Total (Report Footer)",
            },
        };

        if ( definition?.Sections is null || sourceSectionIndex < 0 || sourceSectionIndex >= definition.Sections.Count )
            return locations;

        if ( !TryFindGroupLocation( definition, sourceSectionIndex, out ReportSectionDefinition groupHeader, out int groupFooterIndex ) )
            return locations;

        locations.Add( new()
        {
            TargetSectionIndex = groupFooterIndex,
            Name = $"Group Total ({ReportDefinitionHelper.GetSectionDisplayName( groupHeader )})",
        } );

        return locations;
    }

    internal bool CanInsertSection( ReportDefinition definition, int sectionIndex )
    {
        if ( definition?.Sections is null || sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return false;

        ReportSectionDefinition section = definition.Sections[sectionIndex];

        if ( section.Type == ReportSectionType.GroupHeader )
        {
            int detailSectionIndex = ResolveDetailSectionIndexForGroupHeader( definition, sectionIndex );

            return !ReportValueResolver.ResolveStaticSuppress( section )
                && !string.IsNullOrWhiteSpace( section.GroupBy )
                && TryFindGroupLocation( definition, detailSectionIndex, out _, out _ );
        }

        if ( section.Type == ReportSectionType.GroupFooter )
        {
            return !ReportValueResolver.ResolveStaticSuppress( section )
                && TryFindGroupHeaderForGroupFooter( definition, sectionIndex, out _ );
        }

        return !ReportValueResolver.ResolveStaticSuppress( section );
    }

    internal bool CanInsertGroup( ReportSectionDefinition section )
    {
        return section is not null
            && !ReportValueResolver.ResolveStaticSuppress( section )
            && section.Type == ReportSectionType.Detail;
    }

    internal bool TryFindGroupLocation( ReportDefinition definition, int detailSectionIndex, out ReportSectionDefinition groupHeader, out int groupFooterIndex )
    {
        groupHeader = null;
        groupFooterIndex = -1;

        for ( int sectionIndex = detailSectionIndex - 1; sectionIndex >= 0; sectionIndex-- )
        {
            ReportSectionDefinition section = definition.Sections[sectionIndex];

            if ( ReportValueResolver.ResolveStaticSuppress( section ) )
                continue;

            if ( section.Type == ReportSectionType.GroupHeader )
            {
                if ( string.IsNullOrWhiteSpace( section.GroupBy ) )
                    return false;

                groupHeader = section;
                break;
            }

            if ( section.Type is ReportSectionType.Detail or ReportSectionType.ReportHeader or ReportSectionType.PageHeader )
                return false;
        }

        if ( groupHeader is null )
            return false;

        for ( int sectionIndex = detailSectionIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            ReportSectionDefinition section = definition.Sections[sectionIndex];

            if ( ReportValueResolver.ResolveStaticSuppress( section ) )
                continue;

            if ( section.Type == ReportSectionType.GroupFooter )
            {
                groupFooterIndex = sectionIndex;
                return true;
            }

            if ( section.Type is ReportSectionType.Detail or ReportSectionType.ReportFooter or ReportSectionType.PageFooter or ReportSectionType.GroupHeader )
                return false;
        }

        return false;
    }

    internal int ResolveDetailSectionIndexForGroupHeader( ReportDefinition definition, int groupHeaderIndex )
    {
        if ( definition?.Sections is null || groupHeaderIndex < 0 || groupHeaderIndex >= definition.Sections.Count )
            return -1;

        for ( int sectionIndex = groupHeaderIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            ReportSectionDefinition section = definition.Sections[sectionIndex];

            if ( ReportValueResolver.ResolveStaticSuppress( section ) )
                continue;

            if ( section.Type == ReportSectionType.Detail )
                return sectionIndex;

            if ( section.Type is ReportSectionType.ReportFooter or ReportSectionType.PageFooter or ReportSectionType.GroupFooter )
                return -1;

            if ( section.Type == ReportSectionType.GroupHeader
                && !string.Equals( section.GroupBy, definition.Sections[groupHeaderIndex].GroupBy, StringComparison.OrdinalIgnoreCase ) )
            {
                return -1;
            }
        }

        return -1;
    }

    internal bool TryFindGroupHeaderForGroupFooter( ReportDefinition definition, int groupFooterIndex, out ReportSectionDefinition groupHeader )
    {
        groupHeader = null;

        if ( definition?.Sections is null || groupFooterIndex < 0 || groupFooterIndex >= definition.Sections.Count )
            return false;

        int detailSectionIndex = -1;

        for ( int sectionIndex = groupFooterIndex - 1; sectionIndex >= 0; sectionIndex-- )
        {
            ReportSectionDefinition section = definition.Sections[sectionIndex];

            if ( ReportValueResolver.ResolveStaticSuppress( section ) )
                continue;

            if ( section.Type == ReportSectionType.Detail )
            {
                detailSectionIndex = sectionIndex;
                break;
            }

            if ( section.Type is ReportSectionType.ReportFooter or ReportSectionType.PageFooter or ReportSectionType.GroupHeader )
                return false;
        }

        return detailSectionIndex >= 0
            && TryFindGroupLocation( definition, detailSectionIndex, out groupHeader, out int foundGroupFooterIndex )
            && foundGroupFooterIndex <= groupFooterIndex;
    }

    internal IReadOnlyList<ReportDesignerFieldOption> GetDetailGroupFieldOptions( ReportDefinition definition, object data, int? selectedSectionIndex )
    {
        if ( definition?.Sections is null || selectedSectionIndex is not { } sectionIndex )
            return [];

        ReportSectionDefinition section = definition.Sections[sectionIndex];

        if ( section.Type != ReportSectionType.Detail )
            return [];

        string dataSourceName = section.DataSource;
        object dataSourceValue = ReportDataResolver.ResolveDataSourceValue( definition, data, dataSourceName );
        var fields = ReportDataSourceExplorer.ResolveDataSourceFields( dataSourceValue ).ToList();
        var fieldOptions = FlattenFieldOptions( sectionIndex, dataSourceName, fields )
            .OrderBy( field => field.DisplayName )
            .ToList();

        foreach ( ReportFieldElementDefinition fieldElement in ( section.Elements ?? [] ).OfType<ReportFieldElementDefinition>().Where( element => !string.IsNullOrWhiteSpace( element.Field ) ) )
        {
            if ( fieldOptions.Any( field => string.Equals( field.FieldName, fieldElement.Field, StringComparison.OrdinalIgnoreCase ) ) )
                continue;

            fieldOptions.Add( new()
            {
                SourceSectionIndex = sectionIndex,
                DataSourceName = dataSourceName,
                FieldName = fieldElement.Field,
                DisplayName = fieldElement.Field,
            } );
        }

        return fieldOptions;
    }

    internal IEnumerable<ReportDesignerFieldOption> FlattenFieldOptions( int sourceSectionIndex, string dataSourceName, IEnumerable<ReportDesignerFieldNode> fields )
    {
        foreach ( ReportDesignerFieldNode field in fields ?? [] )
        {
            if ( field.Children.Count == 0 )
            {
                yield return new()
                {
                    SourceSectionIndex = sourceSectionIndex,
                    DataSourceName = dataSourceName,
                    FieldName = field.Path,
                    DisplayName = field.Path,
                    DataType = field.DataType,
                };

                continue;
            }

            foreach ( ReportDesignerFieldOption child in FlattenFieldOptions( sourceSectionIndex, dataSourceName, field.Children ) )
            {
                yield return child;
            }
        }
    }

    internal ReportElementDefinition FindDetailFieldElement( ReportSectionDefinition section, string fieldName )
    {
        return section?.Elements?.FirstOrDefault( element =>
            element is ReportFieldElementDefinition fieldElement
            && string.Equals( fieldElement.Field, fieldName, StringComparison.OrdinalIgnoreCase ) );
    }

    internal IReadOnlyList<ReportAggregateFunction> ResolveSupportedFunctions( ReportDefinition definition, object data, ReportDesignerFieldOption field )
    {
        return field is null
            ? []
            : ReportAggregateResolver.GetSupportedFunctions( definition, data, field.DataSourceName, field.FieldName, field.DataType );
    }

    internal ReportSectionDefinition CreateGroupHeaderSection( ReportDefinition definition, string groupBy )
    {
        string groupName = ResolveGroupName( groupBy );

        return new()
        {
            Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{groupName} group header" ),
            Type = ReportSectionType.GroupHeader,
            Height = ReportDesignerConstants.DefaultGroupSectionHeight,
            GroupBy = groupBy,
            Default = false,
            Suppress = false,
            Elements =
            [
                new ReportTextElementDefinition
                {
                    Name = groupName,
                    Text = ReportExpressionFormatter.FormatFieldExpression( null, groupBy ),
                    X = ReportDesignerConstants.DefaultGroupHeaderElementX,
                    Y = ReportDesignerConstants.DefaultGroupHeaderElementY,
                    Width = ReportDesignerConstants.DefaultGroupHeaderElementWidth,
                    Height = ReportDesignerConstants.DefaultGroupHeaderElementHeight,
                    Font = new()
                    {
                        Bold = true,
                    },
                },
            ],
        };
    }

    internal ReportSectionDefinition CreateGroupFooterSection( ReportDefinition definition, string groupBy )
    {
        string groupName = ResolveGroupName( groupBy );

        return new()
        {
            Name = ReportDefinitionHelper.CreateUniqueSectionName( definition, $"{groupName} group footer" ),
            Type = ReportSectionType.GroupFooter,
            Height = ReportDesignerConstants.DefaultGroupSectionHeight,
            GroupBy = groupBy,
            Default = false,
            Suppress = false,
            Elements =
            [
                new ReportLineElementDefinition
                {
                    Name = $"{groupName} separator",
                    X = ReportDesignerConstants.DefaultGroupFooterLineX,
                    Y = ReportDesignerConstants.DefaultGroupFooterLineY,
                    Width = Math.Max( ReportDesignerConstants.DefaultGroupFooterLineMinimumWidth, ( definition?.Page?.Width ?? ReportDesignerConstants.DefaultPageWidthFallback ) - ReportDesignerConstants.DefaultGroupFooterLinePagePadding ),
                    Height = ReportDesignerConstants.DefaultGroupFooterLineHeight,
                },
            ],
        };
    }

    internal ReportElementDefinition CreateAggregateElement( ReportSectionDefinition sourceSection, ReportElementDefinition sourceElement, ReportAggregateFunction function, ReportSectionDefinition targetSection, bool groupScoped )
    {
        if ( sourceElement is not ReportFieldElementDefinition sourceFieldElement )
            return null;

        string fieldName = sourceFieldElement.Field;
        string functionName = ReportAggregateResolver.GetFunctionDisplayName( function );

        return new ReportFieldElementDefinition
        {
            Name = $"{functionName} of {fieldName}",
            Field = fieldName,
            Format = ReportFormats.Clone( sourceFieldElement.Format ),
            DataSource = groupScoped ? null : string.IsNullOrWhiteSpace( sourceSection.DataSource ) ? sourceFieldElement.DataSource : sourceSection.DataSource,
            X = sourceElement.X,
            Y = GetAggregateElementY( targetSection ),
            Width = sourceElement.Width,
            Height = Math.Max( sourceElement.Height, ReportDesignerConstants.AggregateElementMinimumHeight ),
            Font = new()
            {
                Bold = true,
                Alignment = sourceElement.Font?.Alignment ?? TextAlignment.Default,
                VerticalAlignment = sourceElement.Font?.VerticalAlignment ?? VerticalAlignment.Default,
            },
            Aggregate = new()
            {
                Function = function,
            },
        };
    }

    internal int EnsureTargetSection( ReportDefinition definition, int sourceSectionIndex )
    {
        for ( int sectionIndex = sourceSectionIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            ReportSectionType sectionType = definition.Sections[sectionIndex].Type;

            if ( sectionType is ReportSectionType.ReportFooter )
                return sectionIndex;

            if ( sectionType == ReportSectionType.PageFooter )
                return InsertReportFooter( definition, sectionIndex );
        }

        return InsertReportFooter( definition, definition.Sections.Count );
    }

    private static string ResolveGroupName( string groupBy )
    {
        if ( string.IsNullOrWhiteSpace( groupBy ) )
            return "Group";

        string normalizedGroupBy = groupBy.Trim();
        int lastSeparatorIndex = normalizedGroupBy.LastIndexOf( '.' );

        return lastSeparatorIndex >= 0 && lastSeparatorIndex < normalizedGroupBy.Length - 1
            ? normalizedGroupBy[( lastSeparatorIndex + 1 )..]
            : normalizedGroupBy;
    }

    private static int InsertReportFooter( ReportDefinition definition, int sectionIndex )
    {
        var reportFooter = new ReportSectionDefinition
        {
            Name = "Aggregates",
            Type = ReportSectionType.ReportFooter,
            Height = ReportDesignerConstants.AggregateReportFooterHeight,
        };

        definition.Sections.Insert( sectionIndex, reportFooter );

        return sectionIndex;
    }

    private static double GetAggregateElementY( ReportSectionDefinition targetSection )
    {
        if ( targetSection?.Elements is null || targetSection.Elements.Count == 0 )
            return ReportDesignerConstants.PasteElementOffset;

        var aggregateElements = targetSection.Elements.OfType<ReportFieldElementDefinition>().Where( element => element.Aggregate is not null ).ToList();

        return aggregateElements.Count == 0
            ? Math.Max( ReportDesignerConstants.PasteElementOffset, targetSection.Elements.Max( element => element.Y + element.Height ) + ReportDesignerConstants.KeyboardMoveStep )
            : aggregateElements.Min( element => element.Y );
    }

    #endregion
}