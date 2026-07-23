#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerRulerService
{
    #region Methods

    internal IReadOnlyList<ReportDesignerRulerTick> BuildTicks( ReportMeasurementUnit unit, double length, double gridSize, bool showFineTicks )
    {
        if ( length <= 0 )
            return [];

        double minorStep = Math.Max( 1, gridSize );
        double tickStep = showFineTicks
            ? minorStep
            : minorStep * ReportLayoutGeometry.GridMajorDivisions;

        List<ReportDesignerRulerTick> ticks = [];
        int count = (int)Math.Floor( length / tickStep );

        for ( int index = 0; index <= count; index++ )
        {
            double position = index * tickStep;
            bool major = !showFineTicks || index % ReportLayoutGeometry.GridMajorDivisions == 0;

            ticks.Add( new()
            {
                Position = position,
                Major = major,
                Label = major ? FormatLabel( unit, position ) : null,
            } );
        }

        return ticks;
    }

    internal ReportDesignerRulerMarker CreateMarker(
        ReportDefinition definition,
        ReportDesignerInteractionState state,
        IReadOnlyList<ReportSelectedElementContext> selectedElements,
        int? selectedSectionIndex,
        Func<int, double> getSectionOffsetY,
        Func<int, double> getSectionHeight,
        double sectionBodyTopOffset = 0 )
    {
        if ( definition is null )
            return null;

        double marginLeft = GetMarginLeft( definition );

        if ( state?.DragPreview is not null )
            return CreateDragPreviewMarker( state.DragPreview, marginLeft, getSectionOffsetY, sectionBodyTopOffset );

        if ( state?.SelectionBox is not null )
        {
            return new()
            {
                X = marginLeft + state.SelectionBox.X,
                Y = state.SelectionBox.Y,
                Width = state.SelectionBox.Width,
                Height = state.SelectionBox.Height,
                Active = true,
            };
        }

        ReportDesignerRulerMarker marker = CreateSelectedElementsMarker( selectedElements, marginLeft, getSectionOffsetY, sectionBodyTopOffset );

        if ( marker is not null )
            return marker;

        return CreateSelectedSectionMarker( definition, selectedSectionIndex, getSectionOffsetY, getSectionHeight );
    }

    private static ReportDesignerRulerMarker CreateDragPreviewMarker( ReportDesignerDragPreview preview, double marginLeft, Func<int, double> getSectionOffsetY, double sectionBodyTopOffset )
    {
        return new()
        {
            X = marginLeft + preview.X,
            Y = getSectionOffsetY( preview.SectionIndex ) + sectionBodyTopOffset + preview.Y,
            Width = preview.Width,
            Height = preview.Height,
            Active = true,
        };
    }

    private static ReportDesignerRulerMarker CreateSelectedElementsMarker( IReadOnlyList<ReportSelectedElementContext> selectedElements, double marginLeft, Func<int, double> getSectionOffsetY, double sectionBodyTopOffset )
    {
        if ( selectedElements is null || selectedElements.Count == 0 )
            return null;

        double left = double.MaxValue;
        double top = double.MaxValue;
        double right = double.MinValue;
        double bottom = double.MinValue;

        foreach ( ReportSelectedElementContext selectedElement in selectedElements )
        {
            ReportElementDefinition element = selectedElement.Element;

            if ( element is null )
                continue;

            double elementLeft = marginLeft + selectedElement.OwnerOffsetX + element.X;
            double elementTop = getSectionOffsetY( selectedElement.SectionIndex ) + sectionBodyTopOffset + selectedElement.OwnerOffsetY + element.Y;
            double elementRight = elementLeft + element.Width;
            double elementBottom = elementTop + ReportLayoutGeometry.GetElementRenderHeight( element );

            left = Math.Min( left, elementLeft );
            top = Math.Min( top, elementTop );
            right = Math.Max( right, elementRight );
            bottom = Math.Max( bottom, elementBottom );
        }

        return left == double.MaxValue
            ? null
            : new()
            {
                X = left,
                Y = top,
                Width = Math.Max( 0, right - left ),
                Height = Math.Max( 0, bottom - top ),
            };
    }

    private static ReportDesignerRulerMarker CreateSelectedSectionMarker( ReportDefinition definition, int? selectedSectionIndex, Func<int, double> getSectionOffsetY, Func<int, double> getSectionHeight )
    {
        if ( selectedSectionIndex is not { } sectionIndex
            || sectionIndex < 0
            || sectionIndex >= definition.Bands.Count )
        {
            return null;
        }

        return new()
        {
            X = GetMarginLeft( definition ),
            Y = getSectionOffsetY( sectionIndex ),
            Width = ReportPageDefinitionHelper.GetContentWidth( definition.Page ),
            Height = getSectionHeight( sectionIndex ),
        };
    }

    private static double GetMarginLeft( ReportDefinition definition )
    {
        return Math.Max( 0, definition?.Page?.Margins?.Left ?? 0 );
    }

    private static string FormatLabel( ReportMeasurementUnit unit, double position )
    {
        double value = ReportMeasurementConverter.FromPoints( position, unit );

        return unit switch
        {
            ReportMeasurementUnit.Inch => value.ToString( "0.##", CultureInfo.InvariantCulture ),
            ReportMeasurementUnit.Centimeter => value.ToString( "0.#", CultureInfo.InvariantCulture ),
            ReportMeasurementUnit.Millimeter => value.ToString( "0", CultureInfo.InvariantCulture ),
            _ => value.ToString( "0", CultureInfo.InvariantCulture ),
        };
    }

    #endregion
}