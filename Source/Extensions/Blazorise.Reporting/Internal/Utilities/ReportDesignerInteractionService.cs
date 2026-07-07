#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerInteractionService
{
    #region Methods

    internal static void BeginFieldDrag( ReportDesignerInteractionState state, string dataSourceName, string fieldName )
    {
        if ( state is null )
            return;

        state.DraggedKind = ReportDesignerDragKind.Field;
        state.DraggedDataSourceName = dataSourceName;
        state.DraggedFieldName = fieldName;
        state.DraggedElementType = null;
        state.DraggedElementText = null;
        state.DraggedElementKey = null;
        state.DraggedElement = null;
        state.DragPreview = null;
        state.LastDragPreviewRenderTime = DateTime.MinValue;
        state.EditingElementKey = null;
        state.ElementPointerDrag = null;
        state.ElementPointerResize = null;
        state.TablePointerResize = null;
        state.SectionPointerResize = null;
    }

    internal static void BeginToolboxElementDrag( ReportDesignerInteractionState state, ReportElementType elementType, string text )
    {
        if ( state is null )
            return;

        state.DraggedKind = ReportDesignerDragKind.ToolboxElement;
        state.DraggedElementType = elementType;
        state.DraggedElementText = text;
        state.DraggedDataSourceName = null;
        state.DraggedFieldName = null;
        state.DraggedElementKey = null;
        state.DraggedElement = null;
        state.DragPreview = null;
        state.LastDragPreviewRenderTime = DateTime.MinValue;
        state.EditingElementKey = null;
        state.SelectionBox = null;
        state.ElementPointerDrag = null;
        state.ElementPointerResize = null;
        state.TablePointerResize = null;
    }

    internal static bool IsExternalDesignerDragActive( ReportDesignerInteractionState state )
    {
        return state?.DraggedKind is ReportDesignerDragKind.Field or ReportDesignerDragKind.ToolboxElement;
    }

    internal static bool TryBeginElementPointerDrag(
        ReportDesignerInteractionState state,
        string elementKey,
        ReportElementDefinition element,
        int sectionIndex,
        PointerEventArgs eventArgs,
        bool snapToGrid,
        IReadOnlyList<ReportElementPointerItemState> selectedElements )
    {
        if ( state is null || element is null || eventArgs is null )
            return false;

        state.DraggedKind = ReportDesignerDragKind.Element;
        state.DraggedElementKey = elementKey;
        state.DraggedElement = element;
        state.DraggedDataSourceName = null;
        state.DraggedFieldName = null;
        state.DraggedElementType = null;
        state.DraggedElementText = null;
        state.DragPreview = null;
        state.LastDragPreviewRenderTime = DateTime.MinValue;
        state.TablePointerResize = null;
        state.ElementPointerDrag = new()
        {
            ElementKey = elementKey,
            SourceSectionIndex = sectionIndex,
            TargetSectionIndex = sectionIndex,
            OriginalX = element.X,
            OriginalY = element.Y,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            PointerOffsetX = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ),
            PointerOffsetY = ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ),
            TargetX = element.X,
            TargetY = element.Y,
            SnapToGrid = snapToGrid,
            SelectedElements = selectedElements?.ToList() ?? [],
        };

        return true;
    }

    internal static bool TryBeginElementPointerResize(
        ReportDesignerInteractionState state,
        string elementKey,
        ReportElementDefinition element,
        int sectionIndex,
        ReportElementResizeHandle handle,
        PointerEventArgs eventArgs,
        bool snapToGrid,
        IReadOnlyList<ReportElementPointerItemState> selectedElements )
    {
        if ( state is null || element is null || eventArgs is null )
            return false;

        state.DraggedKind = ReportDesignerDragKind.Element;
        state.DraggedElementKey = elementKey;
        state.DraggedElement = element;
        state.DraggedDataSourceName = null;
        state.DraggedFieldName = null;
        state.DraggedElementType = null;
        state.DraggedElementText = null;
        state.DragPreview = null;
        state.LastDragPreviewRenderTime = DateTime.MinValue;
        state.ElementPointerDrag = null;
        state.TablePointerResize = null;
        state.ElementPointerResize = new()
        {
            ElementKey = elementKey,
            SourceSectionIndex = sectionIndex,
            Handle = handle,
            OriginalX = element.X,
            OriginalY = element.Y,
            OriginalWidth = element.Width,
            OriginalHeight = element.Height,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            TargetX = element.X,
            TargetY = element.Y,
            TargetWidth = element.Width,
            TargetHeight = element.Height,
            MinimumHeight = ReportLayoutGeometry.GetMinimumElementHeight( element ),
            SnapToGrid = snapToGrid,
            SelectedElements = selectedElements?.ToList() ?? [],
        };

        return true;
    }

    internal static void ClearDragState( ReportDesignerInteractionState state )
    {
        if ( state is null )
            return;

        state.DraggedKind = ReportDesignerDragKind.None;
        state.DraggedDataSourceName = null;
        state.DraggedFieldName = null;
        state.DraggedElementType = null;
        state.DraggedElementText = null;
        state.DraggedElementKey = null;
        state.DraggedElement = null;
        state.DragPreview = null;
        state.LastDragPreviewRenderTime = DateTime.MinValue;
        state.ElementPointerDrag = null;
        state.ElementPointerResize = null;
        state.TablePointerResize = null;
    }

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
        var resizingRight = HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.East );
        var resizingBottom = HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.South );

        if ( resizingLeft )
            left = applyGrid( left + deltaX );
        else if ( resizingRight )
            right = applyGrid( right + deltaX );

        if ( resizingTop )
            top = applyGrid( top + deltaY );
        else if ( resizingBottom )
            bottom = applyGrid( bottom + deltaY );

        if ( right - left < ReportLayoutGeometry.DefaultMinimumElementSize )
        {
            if ( resizingLeft )
                left = right - ReportLayoutGeometry.DefaultMinimumElementSize;
            else
                right = left + ReportLayoutGeometry.DefaultMinimumElementSize;
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
            Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, right - left ),
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
            var targetWidth = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, item.OriginalWidth + deltaWidth );
            var targetHeight = Math.Max( minimumHeight, item.OriginalHeight + deltaHeight );
            var targetX = item.OriginalX + deltaX;
            var targetY = item.OriginalY + deltaY;

            if ( HasResizeHandle( pointerResize.Handle, ReportElementResizeHandle.South ) )
                targetHeight = ClampResizeHeightToSectionBottom( section, targetY, targetHeight, minimumHeight );

            element.Width = Math.Min( targetWidth, Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, definition.Page.Width ) );
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

        preview.Width = Math.Min( Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, preview.Width ), Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, definition.Page.Width ) );
        preview.Height = Math.Max( minimumHeight, preview.Height );
        preview.X = ReportLayoutGeometry.Clamp( preview.X, 0, Math.Max( 0, definition.Page.Width - preview.Width ) );
        preview.Y = Math.Max( 0, preview.Y );

        return preview;
    }

    internal static bool TryBeginSelectionBox( ReportDesignerInteractionState state, ReportDefinition definition, int sectionIndex, PointerEventArgs eventArgs, double sectionOffsetY, double contentHeight )
    {
        if ( state is null || definition?.Page is null || eventArgs is null )
            return false;

        if ( state.DraggedKind != ReportDesignerDragKind.None
            || state.ElementPointerDrag is not null
            || state.ElementPointerResize is not null
            || state.TablePointerResize is not null
            || state.SectionPointerResize is not null )
        {
            return false;
        }

        ReportSectionDefinition section = ReportLayoutGeometry.GetSection( definition, sectionIndex );

        if ( section is null || ReportValueResolver.ResolveStaticSuppress( section ) )
            return false;

        double x = ReportLayoutGeometry.Clamp( ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetX ), 0, definition.Page.Width );
        double y = ReportLayoutGeometry.Clamp( sectionOffsetY + ReportMeasurementConverter.FromCssPixelValue( eventArgs.OffsetY ), 0, contentHeight );

        state.SelectionBox = new()
        {
            SectionIndex = sectionIndex,
            StartX = x,
            StartY = y,
            CurrentX = x,
            CurrentY = y,
            StartClientX = eventArgs.ClientX,
            StartClientY = eventArgs.ClientY,
            Additive = eventArgs.CtrlKey,
        };

        state.LastSelectionBoxRenderTime = DateTime.MinValue;

        return true;
    }

    internal static void UpdateSelectionBox( ReportDesignerInteractionState state, ReportDefinition definition, PointerEventArgs eventArgs, double contentHeight )
    {
        if ( state?.SelectionBox is null || definition?.Page is null || eventArgs is null )
            return;

        state.SelectionBox.CurrentX = ReportLayoutGeometry.Clamp( state.SelectionBox.StartX + ReportMeasurementConverter.FromCssPixelValue( eventArgs.ClientX - state.SelectionBox.StartClientX ), 0, definition.Page.Width );
        state.SelectionBox.CurrentY = ReportLayoutGeometry.Clamp( state.SelectionBox.StartY + ReportMeasurementConverter.FromCssPixelValue( eventArgs.ClientY - state.SelectionBox.StartClientY ), 0, contentHeight );
        state.SelectionBox.HasMoved = state.SelectionBox.HasMoved
            || Math.Abs( ReportMeasurementConverter.ToCssPixelValue( state.SelectionBox.CurrentX - state.SelectionBox.StartX ) ) > 2
            || Math.Abs( ReportMeasurementConverter.ToCssPixelValue( state.SelectionBox.CurrentY - state.SelectionBox.StartY ) ) > 2;
    }

    internal static bool CanRenderSelectionBoxPreview( ReportDesignerInteractionState state, double previousX, double previousY, double previousWidth, double previousHeight )
    {
        if ( state?.SelectionBox is null )
            return false;

        if ( Math.Abs( state.SelectionBox.X - previousX ) < ReportDesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( state.SelectionBox.Y - previousY ) < ReportDesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( state.SelectionBox.Width - previousWidth ) < ReportDesignerConstants.DragPreviewChangeTolerance
            && Math.Abs( state.SelectionBox.Height - previousHeight ) < ReportDesignerConstants.DragPreviewChangeTolerance )
        {
            return false;
        }

        DateTime now = DateTime.UtcNow;

        if ( now - state.LastSelectionBoxRenderTime < ReportDesignerConstants.SelectionBoxFrameThrottle )
            return false;

        state.LastSelectionBoxRenderTime = now;

        return true;
    }

    internal static ReportDesignerSelectionBox CompleteSelectionBox( ReportDesignerInteractionState state )
    {
        if ( state is null )
            return null;

        ReportDesignerSelectionBox selectionBox = state.SelectionBox;

        state.SelectionBox = null;
        state.LastSelectionBoxRenderTime = DateTime.MinValue;

        return selectionBox;
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

            if ( ReportValueResolver.ResolveStaticSuppress( section ) || isSectionCollapsed( section ) )
                continue;

            var sectionOffsetY = getSectionOffsetY( sectionIndex );

            foreach ( var element in section.Elements )
            {
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

            if ( element.Suppress?.Value == true )
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
            Width = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, element.Width ),
            Height = Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, element.Height ),
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

    private static double ClampResizeHeightToSectionBottom( ReportSectionDefinition section, double targetY, double targetHeight, double minimumHeight )
    {
        if ( section is null )
            return targetHeight;

        double targetBottom = targetY + targetHeight;
        double overflow = targetBottom - section.Height;

        if ( overflow <= 0 || overflow > ReportLayoutGeometry.SnapToGridSize )
            return targetHeight;

        return Math.Max( minimumHeight, section.Height - targetY );
    }

    #endregion
}