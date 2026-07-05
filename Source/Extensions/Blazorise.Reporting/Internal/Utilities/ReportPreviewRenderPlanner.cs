#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportPreviewRenderPlanner
{
    #region Members

    private const bool SectionsCannotSplitAcrossPages = true;

    #endregion

    #region Methods

    internal static IReadOnlyList<ReportRenderSection> BuildRenderSections( ReportDefinition definition, object data )
    {
        if ( definition?.Sections is null )
            return [];

        var renderSections = new List<ReportRenderSection>();
        var consumedSectionIndexes = new HashSet<int>();
        var runningTotals = ReportRunningTotalResolver.CreateState( definition, data );
        var instanceIndex = 0;

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            if ( consumedSectionIndexes.Contains( sectionIndex ) )
                continue;

            var section = definition.Sections[sectionIndex];

            if ( !ShouldRenderSectionBeforeItems( definition, data, section ) )
                continue;

            if ( section.Type == ReportSectionType.GroupFooter )
                continue;

            if ( IsGroupHeader( section ) && !string.IsNullOrWhiteSpace( section.GroupBy )
                && TryFindGroupedDetail( definition, sectionIndex, out var headerSectionIndexes, out var detailSectionIndex, out var footerSectionIndexes ) )
            {
                AppendGroupedSections( renderSections, definition, data, headerSectionIndexes, detailSectionIndex, footerSectionIndexes, consumedSectionIndexes, runningTotals, ref instanceIndex );
                continue;
            }

            foreach ( var item in ResolveSectionItems( definition, data, section ) )
            {
                if ( !ShouldRenderSection( definition, data, section, item, 1, 1 ) )
                    continue;

                renderSections.Add( CreateRenderSection( sectionIndex, instanceIndex++, section, item, !IsSectionSuppressed( definition, data, section, item ), runningTotals ) );
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
        var pages = PaginateBodySections( definition, data, bodySections, pageHeaderSections, pageFooterSections );
        var totalPages = pages.Count;

        for ( var pageIndex = 0; pageIndex < totalPages; pageIndex++ )
        {
            var pageNumber = pageIndex + 1;
            var page = pages[pageIndex];

            page.PageNumber = pageNumber;
            page.HeaderSections = pageNumber == 1
                ? []
                : pageHeaderSections
                    .Where( renderSection => ShouldRenderSection( definition, data, renderSection.Section, renderSection.Item, pageNumber, totalPages ) )
                    .ToList();
            page.FooterSections = pageFooterSections
                .Where( renderSection => ShouldRenderSection( definition, data, renderSection.Section, renderSection.Item, pageNumber, totalPages ) )
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

            if ( section.Type != sectionType )
                continue;

            var item = ReportDataResolver.ResolveItems( definition, data, section.DataSource ).FirstOrDefault();
            var suppressed = IsSectionSuppressed( definition, data, section, item );

            if ( suppressed && !section.ReserveSpaceWhenSuppressed )
                continue;

            renderSections.Add( new()
            {
                SectionIndex = sectionIndex,
                InstanceIndex = sectionIndex,
                Section = section,
                Item = item,
                RenderElements = !suppressed,
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
        ReportRunningTotalState runningTotals,
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
                var suppressed = IsSectionSuppressed( definition, data, section, firstItem );

                if ( suppressed && !section.ReserveSpaceWhenSuppressed )
                    continue;

                renderSections.Add( CreateRenderSection( headerSectionIndex, instanceIndex++, section, firstItem, !suppressed, runningTotals ) );
            }

            foreach ( var item in groupItems )
            {
                var detailSuppressed = IsSectionSuppressed( definition, data, detailSection, item );

                if ( detailSuppressed && !detailSection.ReserveSpaceWhenSuppressed )
                    continue;

                renderSections.Add( CreateRenderSection( detailSectionIndex, instanceIndex++, detailSection, item, !detailSuppressed, runningTotals ) );
            }

            foreach ( var footerSectionIndex in footerSectionIndexes )
            {
                var section = definition.Sections[footerSectionIndex];
                var suppressed = IsSectionSuppressed( definition, data, section, groupItems );

                if ( suppressed && !section.ReserveSpaceWhenSuppressed )
                    continue;

                renderSections.Add( CreateRenderSection( footerSectionIndex, instanceIndex++, section, groupItems, !suppressed, runningTotals ) );
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

    private static ReportRenderSection CreateRenderSection( int sectionIndex, int instanceIndex, ReportSectionDefinition section, object item, bool renderElements, ReportRunningTotalState runningTotals )
    {
        var renderSection = new ReportRenderSection
        {
            SectionIndex = sectionIndex,
            InstanceIndex = instanceIndex,
            Section = section,
            Item = item,
            RenderElements = renderElements,
        };

        runningTotals?.ProcessSection( renderSection );
        renderSection.RunningTotals = runningTotals?.BuildSnapshot();

        return renderSection;
    }

    private static List<ReportRenderPage> PaginateBodySections(
        ReportDefinition definition,
        object data,
        IReadOnlyList<ReportRenderSection> bodySections,
        IReadOnlyList<ReportRenderSection> pageHeaderSections,
        IReadOnlyList<ReportRenderSection> pageFooterSections )
    {
        var pages = new List<ReportRenderPage>();
        var currentPageSections = new List<ReportRenderSection>();
        var usedHeight = 0d;

        for ( var sectionIndex = 0; sectionIndex < bodySections.Count; sectionIndex++ )
        {
            var renderSection = bodySections[sectionIndex];
            var availableHeight = GetAvailableBodyHeight( definition, pageHeaderSections, pageFooterSections, pages.Count + 1 );
            var sectionHeight = GetRenderSectionHeight( renderSection );
            var newPageBefore = ResolveNewPageBefore( definition, data, renderSection );
            var keepTogether = ResolveKeepTogether( definition, data, renderSection );

            if ( ShouldStartNewPage( currentPageSections, usedHeight, availableHeight, sectionHeight, keepTogether, newPageBefore ) )
            {
                pages.Add( new() { BodySections = currentPageSections } );
                currentPageSections = [];
                usedHeight = 0;
                availableHeight = GetAvailableBodyHeight( definition, pageHeaderSections, pageFooterSections, pages.Count + 1 );
            }

            currentPageSections.Add( renderSection );
            usedHeight += sectionHeight;

            if ( ResolveNewPageAfter( definition, data, renderSection ) && sectionIndex < bodySections.Count - 1 )
            {
                pages.Add( new() { BodySections = currentPageSections } );
                currentPageSections = [];
                usedHeight = 0;
            }
        }

        if ( currentPageSections.Count > 0 || pages.Count == 0 )
        {
            pages.Add( new() { BodySections = currentPageSections } );
        }

        return pages;
    }

    private static bool ShouldStartNewPage(
        IReadOnlyList<ReportRenderSection> currentPageSections,
        double usedHeight,
        double availableHeight,
        double sectionHeight,
        bool keepTogether,
        bool newPageBefore )
    {
        if ( currentPageSections.Count == 0 )
            return false;

        if ( newPageBefore )
            return true;

        var sectionOverflowsPage = usedHeight + sectionHeight > availableHeight;

        if ( !sectionOverflowsPage )
            return false;

        return keepTogether || SectionsCannotSplitAcrossPages;
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
        return section.Type is ReportSectionType.ReportFooter or ReportSectionType.PageFooter
            || IsGroupHeader( section );
    }

    private static object ResolveGroupKey( ReportDefinition definition, object data, object item, string groupBy )
    {
        return ReportExpressionResolver.ResolveValue( definition, data, item, groupBy )
            ?? string.Empty;
    }

    private static bool ShouldRenderSection( ReportDefinition definition, object data, ReportSectionDefinition section, object item, int pageNumber, int totalPages )
    {
        if ( IsSectionSuppressed( definition, data, section, item ) && !section.ReserveSpaceWhenSuppressed )
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

    private static bool ShouldRenderSectionBeforeItems( ReportDefinition definition, object data, ReportSectionDefinition section )
    {
        return section?.Suppress?.HasFormula == true
            || ShouldRenderSection( definition, data, section, null, 1, 1 );
    }

    private static bool ShouldIncludeSectionInStructure( ReportDefinition definition, ReportSectionDefinition section )
    {
        return section?.Suppress?.HasFormula == true
            || ShouldRenderSection( definition, null, section, null, 1, 1 );
    }

    private static bool IsSectionSuppressed( ReportDefinition definition, object data, ReportSectionDefinition section, object item )
    {
        return ReportValueResolver.ResolveSuppress( section, definition, data, item );
    }

    private static bool ResolveKeepTogether( ReportDefinition definition, object data, ReportRenderSection renderSection )
    {
        return ReportValueResolver.ResolveKeepTogether( renderSection.Section, definition, data, renderSection.Item );
    }

    private static bool ResolveNewPageBefore( ReportDefinition definition, object data, ReportRenderSection renderSection )
    {
        return ReportValueResolver.ResolveNewPageBefore( renderSection.Section, definition, data, renderSection.Item );
    }

    private static bool ResolveNewPageAfter( ReportDefinition definition, object data, ReportRenderSection renderSection )
    {
        return ReportValueResolver.ResolveNewPageAfter( renderSection.Section, definition, data, renderSection.Item );
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

            if ( !ShouldIncludeSectionInStructure( definition, section ) )
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

            if ( !ShouldIncludeSectionInStructure( definition, section ) )
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