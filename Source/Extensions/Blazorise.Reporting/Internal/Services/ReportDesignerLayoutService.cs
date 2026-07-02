#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerLayoutService
{
    #region Members

    private (
        ReportDefinition Definition,
        ReportBandMode BandMode,
        int CollapsedSectionsVersion,
        int SectionCount,
        int ResizeSectionIndex,
        double ResizeHeight,
        double[] SectionOffsets,
        double ContentHeight ) cache;

    #endregion

    #region Methods

    internal double GetSectionOffsetY( ReportDefinition definition, int sectionIndex, ReportBandMode bandMode, int collapsedSectionsVersion, ReportSectionPointerResizeState sectionPointerResize, Func<ReportSectionDefinition, bool> isSectionCollapsed )
    {
        EnsureCache( definition, bandMode, collapsedSectionsVersion, sectionPointerResize, isSectionCollapsed );

        return cache.SectionOffsets is not null && sectionIndex >= 0 && sectionIndex < cache.SectionOffsets.Length
            ? cache.SectionOffsets[sectionIndex]
            : ReportLayoutGeometry.GetSectionOffsetY( definition, sectionIndex, ( index, section ) => GetSectionHeight( index, section, bandMode, sectionPointerResize, isSectionCollapsed ) );
    }

    internal double GetContentHeight( ReportDefinition definition, ReportBandMode bandMode, int collapsedSectionsVersion, ReportSectionPointerResizeState sectionPointerResize, Func<ReportSectionDefinition, bool> isSectionCollapsed )
    {
        EnsureCache( definition, bandMode, collapsedSectionsVersion, sectionPointerResize, isSectionCollapsed );

        return cache.SectionOffsets is not null
            ? cache.ContentHeight
            : ReportLayoutGeometry.GetContentHeight( definition, ( index, section ) => GetSectionHeight( index, section, bandMode, sectionPointerResize, isSectionCollapsed ) );
    }

    internal double GetSectionHeight( int sectionIndex, ReportSectionDefinition section, ReportBandMode bandMode, ReportSectionPointerResizeState sectionPointerResize, Func<ReportSectionDefinition, bool> isSectionCollapsed )
    {
        if ( sectionPointerResize is not null && sectionPointerResize.SectionIndex == sectionIndex )
            return GetSectionDesignerHeight( sectionPointerResize.TargetHeight, bandMode );

        if ( bandMode == ReportBandMode.Rail && section is not null && !section.Suppressed && isSectionCollapsed( section ) )
        {
            return ReportMeasurementConverter.FromCssPixelValue( ReportDesignerConstants.DesignerCollapsedBandHeight );
        }

        return GetSectionDesignerHeight( section?.Height ?? 0, bandMode );
    }

    private static double GetSectionDesignerHeight( double sectionHeight, ReportBandMode bandMode )
    {
        return bandMode == ReportBandMode.Classic
            ? sectionHeight + ReportDesignerConstants.DesignerBandHeaderHeight
            : sectionHeight;
    }

    internal void Invalidate()
    {
        cache = default;
    }

    private void EnsureCache( ReportDefinition definition, ReportBandMode bandMode, int collapsedSectionsVersion, ReportSectionPointerResizeState sectionPointerResize, Func<ReportSectionDefinition, bool> isSectionCollapsed )
    {
        if ( definition is null )
            return;

        int resizeSectionIndex = sectionPointerResize?.SectionIndex ?? -1;
        double resizeHeight = sectionPointerResize?.TargetHeight ?? 0;
        int sectionCount = definition.Sections.Count;

        if ( ReferenceEquals( cache.Definition, definition )
             && cache.SectionOffsets is not null
             && cache.BandMode == bandMode
             && cache.CollapsedSectionsVersion == collapsedSectionsVersion
             && cache.SectionCount == sectionCount
             && cache.ResizeSectionIndex == resizeSectionIndex
             && Math.Abs( cache.ResizeHeight - resizeHeight ) < ReportDesignerConstants.DragPreviewChangeTolerance )
        {
            return;
        }

        double[] sectionOffsets = new double[sectionCount];
        double offset = 0;

        for ( int i = 0; i < sectionCount; i++ )
        {
            sectionOffsets[i] = offset;
            offset += GetSectionHeight( i, definition.Sections[i], bandMode, sectionPointerResize, isSectionCollapsed );
        }

        cache = ( definition, bandMode, collapsedSectionsVersion, sectionCount, resizeSectionIndex, resizeHeight, sectionOffsets, offset );
    }

    #endregion
}