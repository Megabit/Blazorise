#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportPreviewRenderPlanner
{
    #region Methods

    internal static IReadOnlyList<ReportRenderSection> BuildRenderSections( ReportDefinition definition, object data )
    {
        if ( definition?.Sections is null )
            return [];

        var renderSections = new List<ReportRenderSection>();
        var consumedSectionIndexes = new HashSet<int>();
        var instanceIndex = 0;

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            if ( consumedSectionIndexes.Contains( sectionIndex ) )
                continue;

            var section = definition.Sections[sectionIndex];

            if ( !ShouldRenderSection( section, 1, 1 ) )
                continue;

            if ( section.Type == ReportSectionType.GroupFooter )
                continue;

            if ( IsGroupHeader( section ) && !string.IsNullOrWhiteSpace( section.GroupBy )
                && TryFindGroupedDetail( definition, sectionIndex, out var headerSectionIndexes, out var detailSectionIndex, out var footerSectionIndexes ) )
            {
                AppendGroupedSections( renderSections, definition, data, headerSectionIndexes, detailSectionIndex, footerSectionIndexes, consumedSectionIndexes, ref instanceIndex );
                continue;
            }

            foreach ( var item in ResolveSectionItems( definition, data, section ) )
            {
                renderSections.Add( new()
                {
                    SectionIndex = sectionIndex,
                    InstanceIndex = instanceIndex++,
                    Section = section,
                    Item = item,
                    RenderElements = !section.Suppressed,
                } );
            }
        }

        return renderSections;
    }

    internal static IReadOnlyList<ReportRenderPage> BuildRenderPages( ReportDefinition definition, object data )
    {
        if ( definition?.Sections is null )
            return [];

        definition.Page = ReportPageDefinitionHelper.ResolvePage( definition.Page );

        var pageHeaderSections = BuildPageBandRenderSections( definition, data, ReportSectionType.PageHeader );
        var pageFooterSections = BuildPageBandRenderSections( definition, data, ReportSectionType.PageFooter );
        var allSections = BuildRenderSections( definition, data );
        var bodySections = allSections.Where( renderSection => renderSection.Section.Type != ReportSectionType.PageFooter ).ToList();
        var pages = PaginateBodySections( definition, bodySections, pageHeaderSections, pageFooterSections );
        var totalPages = pages.Count;

        for ( var pageIndex = 0; pageIndex < totalPages; pageIndex++ )
        {
            var pageNumber = pageIndex + 1;
            var page = pages[pageIndex];

            page.PageNumber = pageNumber;
            page.HeaderSections = pageNumber == 1
                ? []
                : pageHeaderSections
                    .Where( renderSection => ShouldRenderSection( renderSection.Section, pageNumber, totalPages ) )
                    .ToList();
            page.FooterSections = pageFooterSections
                .Where( renderSection => ShouldRenderSection( renderSection.Section, pageNumber, totalPages ) )
                .ToList();
        }

        return pages;
    }

    private static List<ReportRenderSection> BuildPageBandRenderSections( ReportDefinition definition, object data, ReportSectionType sectionType )
    {
        var renderSections = new List<ReportRenderSection>();

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Type != sectionType || section.Suppressed && !section.ReserveSpaceWhenSuppressed )
                continue;

            renderSections.Add( new()
            {
                SectionIndex = sectionIndex,
                InstanceIndex = sectionIndex,
                Section = section,
                Item = ReportDataResolver.ResolveItems( definition, data, section.DataSource ).FirstOrDefault(),
                RenderElements = !section.Suppressed,
            } );
        }

        return renderSections;
    }

    private static void AppendGroupedSections(
        List<ReportRenderSection> renderSections,
        ReportDefinition definition,
        object data,
        IReadOnlyList<int> headerSectionIndexes,
        int detailSectionIndex,
        IReadOnlyList<int> footerSectionIndexes,
        HashSet<int> consumedSectionIndexes,
        ref int instanceIndex )
    {
        var detailSection = definition.Sections[detailSectionIndex];
        var headerSection = definition.Sections[headerSectionIndexes[0]];
        var detailItems = ReportDataResolver.ResolveItems( definition, data, detailSection.DataSource ).ToList();

        if ( detailItems.Count == 0 )
            detailItems.Add( null );

        foreach ( var group in detailItems.GroupBy( item => ResolveGroupKey( definition, data, item, headerSection.GroupBy ) ) )
        {
            var groupItems = group.ToList();
            var firstItem = groupItems.FirstOrDefault();

            foreach ( var headerSectionIndex in headerSectionIndexes )
            {
                var section = definition.Sections[headerSectionIndex];

                renderSections.Add( new()
                {
                    SectionIndex = headerSectionIndex,
                    InstanceIndex = instanceIndex++,
                    Section = section,
                    Item = firstItem,
                    RenderElements = !section.Suppressed,
                } );
            }

            foreach ( var item in groupItems )
            {
                renderSections.Add( new()
                {
                    SectionIndex = detailSectionIndex,
                    InstanceIndex = instanceIndex++,
                    Section = detailSection,
                    Item = item,
                    RenderElements = !detailSection.Suppressed,
                } );
            }

            foreach ( var footerSectionIndex in footerSectionIndexes )
            {
                var section = definition.Sections[footerSectionIndex];

                renderSections.Add( new()
                {
                    SectionIndex = footerSectionIndex,
                    InstanceIndex = instanceIndex++,
                    Section = section,
                    Item = groupItems,
                    RenderElements = !section.Suppressed,
                } );
            }
        }

        foreach ( var headerSectionIndex in headerSectionIndexes )
        {
            consumedSectionIndexes.Add( headerSectionIndex );
        }

        consumedSectionIndexes.Add( detailSectionIndex );

        foreach ( var footerSectionIndex in footerSectionIndexes )
        {
            consumedSectionIndexes.Add( footerSectionIndex );
        }
    }

    private static List<ReportRenderPage> PaginateBodySections(
        ReportDefinition definition,
        IReadOnlyList<ReportRenderSection> bodySections,
        IReadOnlyList<ReportRenderSection> pageHeaderSections,
        IReadOnlyList<ReportRenderSection> pageFooterSections )
    {
        var pages = new List<ReportRenderPage>();
        var currentPageSections = new List<ReportRenderSection>();
        var usedHeight = 0d;

        foreach ( var renderSection in bodySections )
        {
            var availableHeight = GetAvailableBodyHeight( definition, pageHeaderSections, pageFooterSections, pages.Count + 1 );
            var sectionHeight = GetRenderSectionHeight( renderSection );

            if ( currentPageSections.Count > 0 && usedHeight + sectionHeight > availableHeight )
            {
                pages.Add( new() { BodySections = currentPageSections } );
                currentPageSections = [];
                usedHeight = 0;
                availableHeight = GetAvailableBodyHeight( definition, pageHeaderSections, pageFooterSections, pages.Count + 1 );
            }

            currentPageSections.Add( renderSection );
            usedHeight += sectionHeight;
        }

        if ( currentPageSections.Count > 0 || pages.Count == 0 )
        {
            pages.Add( new() { BodySections = currentPageSections } );
        }

        return pages;
    }

    private static double GetAvailableBodyHeight(
        ReportDefinition definition,
        IReadOnlyList<ReportRenderSection> pageHeaderSections,
        IReadOnlyList<ReportRenderSection> pageFooterSections,
        int pageNumber )
    {
        var contentHeight = ReportPageDefinitionHelper.GetContentHeight( definition.Page );
        var pageHeaderHeight = pageNumber == 1 ? 0 : pageHeaderSections.Sum( GetRenderSectionHeight );
        var pageFooterHeight = pageFooterSections.Sum( GetRenderSectionHeight );

        return Math.Max( 1, contentHeight - pageHeaderHeight - pageFooterHeight );
    }

    private static double GetRenderSectionHeight( ReportRenderSection renderSection )
    {
        return Math.Max( 0, renderSection?.Section?.Height ?? 0 );
    }

    private static bool IsGroupHeader( ReportSectionDefinition section )
    {
        return section.Type is ReportSectionType.Group or ReportSectionType.GroupHeader;
    }

    private static bool IsGroupBoundary( ReportSectionDefinition section )
    {
        return section.Type is ReportSectionType.ReportFooter or ReportSectionType.Footer or ReportSectionType.PageFooter
            || IsGroupHeader( section );
    }

    private static object ResolveGroupKey( ReportDefinition definition, object data, object item, string groupBy )
    {
        return ReportExpressionResolver.ResolveValue( definition, data, item, groupBy )
            ?? string.Empty;
    }

    private static bool ShouldRenderSection( ReportSectionDefinition section, int pageNumber, int totalPages )
    {
        if ( section.Suppressed && !section.ReserveSpaceWhenSuppressed )
            return false;

        if ( section.Type == ReportSectionType.PageFooter )
        {
            if ( pageNumber == 1 && !section.PrintOnFirstPage )
                return false;

            if ( pageNumber == totalPages && !section.PrintOnLastPage )
                return false;

            if ( pageNumber > 1 && pageNumber < totalPages && !section.RepeatOnEveryPage )
                return false;
        }

        return true;
    }

    private static IEnumerable<object> ResolveSectionItems( ReportDefinition definition, object data, ReportSectionDefinition section )
    {
        var items = section.Type == ReportSectionType.Detail
            ? ReportDataResolver.ResolveItems( definition, data, section.DataSource ).ToList()
            : new List<object> { ReportDataResolver.ResolveItems( definition, data, section.DataSource ).FirstOrDefault() };

        if ( items.Count == 0 )
            items.Add( null );

        return items;
    }

    private static bool TryFindGroupedDetail( ReportDefinition definition, int headerSectionIndex, out IReadOnlyList<int> headerSectionIndexes, out int detailSectionIndex, out IReadOnlyList<int> footerSectionIndexes )
    {
        var headers = new List<int>();
        var footers = new List<int>();

        headerSectionIndexes = headers;
        detailSectionIndex = -1;
        footerSectionIndexes = footers;

        var groupBy = definition.Sections[headerSectionIndex].GroupBy;

        for ( var sectionIndex = headerSectionIndex; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( !ShouldRenderSection( section, 1, 1 ) )
                continue;

            if ( IsGroupHeader( section ) && string.Equals( section.GroupBy, groupBy, StringComparison.OrdinalIgnoreCase ) )
            {
                headers.Add( sectionIndex );
                continue;
            }

            if ( section.Type == ReportSectionType.Detail )
            {
                detailSectionIndex = sectionIndex;
                break;
            }

            if ( IsGroupBoundary( section ) )
                return false;
        }

        if ( detailSectionIndex < 0 )
            return false;

        for ( var sectionIndex = detailSectionIndex + 1; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( !ShouldRenderSection( section, 1, 1 ) )
                continue;

            if ( section.Type == ReportSectionType.GroupFooter )
            {
                footers.Add( sectionIndex );
                continue;
            }

            if ( IsGroupBoundary( section ) )
                break;
        }

        return true;
    }

    #endregion
}