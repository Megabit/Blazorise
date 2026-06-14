#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerInteractionService
{
    #region Methods

    internal static ReportDesignerDragPreview CreateElementDragPreview(
        ReportDefinition definition,
        ReportElementPointerDragState pointerDrag,
        ReportElementDefinition element,
        int targetSectionIndex,
        double clientX,
        double clientY,
        Func<int, double> getSectionOffsetY,
        Func<double, double> applyGrid )
    {
        if ( pointerDrag is null || element is null )
            return null;

        var deltaX = ReportMeasurementConverter.FromCssPixelValue( clientX - pointerDrag.StartClientX );
        var deltaY = ReportMeasurementConverter.FromCssPixelValue( clientY - pointerDrag.StartClientY );
        var x = pointerDrag.OriginalX + deltaX;
        var pageY = getSectionOffsetY( pointerDrag.SourceSectionIndex ) + pointerDrag.OriginalY + deltaY;
        var y = pageY - getSectionOffsetY( targetSectionIndex );

        x = applyGrid( x );
        y = applyGrid( y );

        return ConstrainDragPreview( definition, CreateDragPreview( definition, targetSectionIndex, element, x, y ) );
    }

    internal static void ApplyElementPointerDrag( ReportDefinition definition, ReportElementPointerDragState pointerDrag, Func<int, double> getSectionOffsetY )
    {
        if ( definition is null || pointerDrag is null )
            return;

        if ( pointerDrag.TargetSectionIndex < 0 || pointerDrag.TargetSectionIndex >= definition.Sections.Count )
            return;

        var activeOriginalPageY = getSectionOffsetY( pointerDrag.SourceSectionIndex ) + pointerDrag.OriginalY;
        var activeTargetPageY = getSectionOffsetY( pointerDrag.TargetSectionIndex ) + pointerDrag.TargetY;
        var activeCrossedSections = pointerDrag.TargetSectionIndex != pointerDrag.SourceSectionIndex;
        var deltaX = pointerDrag.TargetX - pointerDrag.OriginalX;
        var deltaLocalY = pointerDrag.TargetY - pointerDrag.OriginalY;
        var deltaPageY = activeTargetPageY - activeOriginalPageY;
        var affectedSectionIndexes = new HashSet<int>();
        var selectedElementKeys = pointerDrag.SelectedElements.Select( selectedItem => selectedItem.ElementKey ).ToList();

        foreach ( var item in pointerDrag.SelectedElements )
        {
            if ( !ReportDefinitionHelper.TryFindElementLocation( definition, item.ElementKey, out var sourceSectionIndex, out var sourceElementIndex, out var element ) )
                continue;

            var targetSectionIndex = activeCrossedSections
                ? ResolveSectionIndex( definition, item.OriginalPageY + deltaPageY, getSectionOffsetY )
                : item.OriginalSectionIndex;

            if ( targetSectionIndex < 0 || targetSectionIndex >= definition.Sections.Count )
                targetSectionIndex = sourceSectionIndex;

            var targetSection = definition.Sections[targetSectionIndex];
            var targetLocalY = activeCrossedSections
                ? item.OriginalPageY + deltaPageY - getSectionOffsetY( targetSectionIndex )
                : item.OriginalY + deltaLocalY;

            element.X = ReportLayoutGeometry.Clamp( item.OriginalX + deltaX, 0, Math.Max( 0, definition.Page.Width - element.Width ) );
            element.Y = Math.Max( 0, targetLocalY );

            ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                definition,
                sourceSectionIndex,
                targetSectionIndex,
                element,
                item.OriginalX,
                item.OriginalWidth,
                element.X,
                element.Width,
                selectedElementKeys );

            if ( sourceSectionIndex != targetSectionIndex )
            {
                definition.Sections[sourceSectionIndex].Elements.RemoveAt( sourceElementIndex );
                targetSection.Elements.Add( element );
            }

            affectedSectionIndexes.Add( targetSectionIndex );
        }

        foreach ( var sectionIndex in affectedSectionIndexes )
        {
            ReportLayoutGeometry.GrowSectionToFitElements( definition.Sections[sectionIndex] );
        }
    }

    internal static ReportDesignerDragPreview CreateElementResizePreview(
        ReportDefinition definition,
        ReportElementPointerResizeState pointerResize,
        ReportElementDefinition element,
        double clientX,
        double clientY,
        Func<double, double> applyGrid )
    {
        if ( pointerResize is null || element is null )
            return null;

        var deltaX = ReportMeasurementConverter.FromCssPixelValue( clientX - pointerResize.StartClientX );
        var deltaY = ReportMeasurementConverter.FromCssPixelValue( clientY - pointerResize.StartClientY );
        var left = pointerResize.OriginalX;
        var top = pointerResize.OriginalY;
        var right = pointerResize.OriginalX + pointerResize.OriginalWidth;
        var bottom = pointerResize.OriginalY + pointerResize.OriginalHeight;
        var resizingLeft = HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.West );
        var resizingTop = HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.North );

        if ( resizingLeft )
            left += deltaX;
        else if ( HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.East ) )
            right += deltaX;

        if ( resizingTop )
            top += deltaY;
        else if ( HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.South ) )
            bottom += deltaY;

        left = applyGrid( left );
        top = applyGrid( top );
        right = applyGrid( right );
        bottom = applyGrid( bottom );

        if ( right - left < 8 )
        {
            if ( resizingLeft )
                left = right - 8;
            else
                right = left + 8;
        }

        if ( bottom - top < pointerResize.MinimumHeight )
        {
            if ( resizingTop )
                top = bottom - pointerResize.MinimumHeight;
            else
                bottom = top + pointerResize.MinimumHeight;
        }

        left = Math.Max( 0, left );
        top = Math.Max( 0, top );

        return new()
        {
            SectionIndex = pointerResize.SourceSectionIndex,
            ElementType = element.Type,
            Text = ReportElementDefinitionHelper.GetDisplayText( definition, element ),
            X = left,
            Y = top,
            Width = Math.Max( 8, right - left ),
            Height = Math.Max( pointerResize.MinimumHeight, bottom - top ),
        };
    }

    internal static void ApplyElementPointerResize( ReportDefinition definition, ReportElementPointerResizeState pointerResize )
    {
        if ( definition is null || pointerResize is null )
            return;

        var deltaX = pointerResize.TargetX - pointerResize.OriginalX;
        var deltaY = pointerResize.TargetY - pointerResize.OriginalY;
        var deltaWidth = pointerResize.TargetWidth - pointerResize.OriginalWidth;
        var deltaHeight = pointerResize.TargetHeight - pointerResize.OriginalHeight;

        foreach ( var item in pointerResize.SelectedElements )
        {
            if ( !ReportDefinitionHelper.TryFindElementLocation( definition, item.ElementKey, out var sectionIndex, out _, out var element ) )
                continue;

            var section = definition.Sections[sectionIndex];
            var minimumHeight = ReportLayoutGeometry.GetMinimumElementHeight( element );
            var targetWidth = Math.Max( 8, item.OriginalWidth + deltaWidth );
            var targetHeight = Math.Max( minimumHeight, item.OriginalHeight + deltaHeight );
            var targetX = item.OriginalX + deltaX;
            var targetY = item.OriginalY + deltaY;

            element.Width = Math.Min( targetWidth, Math.Max( 8, definition.Page.Width ) );
            element.Height = targetHeight;
            element.X = ReportLayoutGeometry.Clamp( targetX, 0, Math.Max( 0, definition.Page.Width - element.Width ) );
            element.Y = Math.Max( 0, targetY );

            ReportDetailHeaderSynchronizer.SyncMatchingPageHeaderForDetailElement(
                definition,
                sectionIndex,
                sectionIndex,
                element,
                item.OriginalX,
                item.OriginalWidth,
                element.X,
                element.Width,
                pointerResize.SelectedElements.Select( selectedItem => selectedItem.ElementKey ) );

            ReportLayoutGeometry.GrowSectionToFitElement( section, element );
        }
    }

    internal static double CreateSectionResizeHeight( ReportSectionPointerResizeState pointerResize, double clientY, double minimumHeight, Func<double, double> applyGrid )
    {
        return Math.Max( minimumHeight, applyGrid( pointerResize.OriginalHeight + ReportMeasurementConverter.FromCssPixelValue( clientY - pointerResize.StartClientY ) ) );
    }

    internal static ReportDesignerDragPreview ConstrainDragPreview( ReportDefinition definition, ReportDesignerDragPreview preview )
    {
        if ( preview is null )
            return null;

        var section = ReportLayoutGeometry.GetSection( definition, preview.SectionIndex );

        if ( definition?.Page is null || section is null )
            return preview;

        var minimumHeight = ReportLayoutGeometry.GetMinimumElementHeight( preview.ElementType );

        preview.Width = Math.Min( Math.Max( 8, preview.Width ), Math.Max( 8, definition.Page.Width ) );
        preview.Height = Math.Max( minimumHeight, preview.Height );
        preview.X = ReportLayoutGeometry.Clamp( preview.X, 0, Math.Max( 0, definition.Page.Width - preview.Width ) );
        preview.Y = Math.Max( 0, preview.Y );

        return preview;
    }

    internal static IEnumerable<string> FindElementsInsideSelectionBox(
        ReportDefinition definition,
        ReportDesignerSelectionBox selectionBox,
        Func<ReportSectionDefinition, bool> isSectionCollapsed,
        Func<int, double> getSectionOffsetY )
    {
        if ( definition is null || selectionBox is null )
            yield break;

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var section = definition.Sections[sectionIndex];

            if ( section.Suppressed || isSectionCollapsed( section ) )
                continue;

            var sectionOffsetY = getSectionOffsetY( sectionIndex );

            foreach ( var element in section.Elements )
            {
                if ( element.Suppress )
                    continue;

                if ( ReportLayoutGeometry.Intersects( selectionBox.X, selectionBox.Y, selectionBox.Width, selectionBox.Height, element.X, sectionOffsetY + element.Y, element.Width, element.Height ) )
                    yield return ReportDefinitionHelper.EnsureElementId( element );
            }
        }
    }

    internal static IEnumerable<ReportElementPointerItemState> CaptureElementPointerItems( ReportDefinition definition, IEnumerable<string> elementKeys, Func<int, double> getSectionOffsetY )
    {
        if ( definition is null || elementKeys is null )
            yield break;

        foreach ( var elementKey in elementKeys )
        {
            if ( !ReportDefinitionHelper.TryFindElementLocation( definition, elementKey, out var sectionIndex, out _, out var element ) )
                continue;

            if ( element.Suppress )
                continue;

            yield return new()
            {
                ElementKey = elementKey,
                OriginalSectionIndex = sectionIndex,
                OriginalX = element.X,
                OriginalY = element.Y,
                OriginalPageY = getSectionOffsetY( sectionIndex ) + element.Y,
                OriginalWidth = element.Width,
                OriginalHeight = element.Height,
            };
        }
    }

    private static ReportDesignerDragPreview CreateDragPreview( ReportDefinition definition, int targetSectionIndex, ReportElementDefinition element, double? x = null, double? y = null )
    {
        return new()
        {
            SectionIndex = targetSectionIndex,
            ElementType = element.Type,
            Text = ReportElementDefinitionHelper.GetDisplayText( definition, element ),
            X = x ?? element.X,
            Y = y ?? element.Y,
            Width = Math.Max( 8, element.Width ),
            Height = Math.Max( 8, element.Height ),
        };
    }

    private static int ResolveSectionIndex( ReportDefinition definition, double pageY, Func<int, double> getSectionOffsetY )
    {
        if ( definition?.Sections is null || definition.Sections.Count == 0 )
            return -1;

        for ( var sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            var sectionTop = getSectionOffsetY( sectionIndex );
            var nextSectionTop = sectionIndex + 1 < definition.Sections.Count
                ? getSectionOffsetY( sectionIndex + 1 )
                : sectionTop + Math.Max( 0, definition.Sections[sectionIndex].Height );

            if ( pageY >= sectionTop && pageY < nextSectionTop )
                return sectionIndex;
        }

        return pageY < getSectionOffsetY( 0 )
            ? 0
            : definition.Sections.Count - 1;
    }

    private static bool HasResizeHandle( ReportElementResizeHandle handle, ReportElementResizeHandle flag )
        => ( handle & flag ) == flag;

    #endregion
}