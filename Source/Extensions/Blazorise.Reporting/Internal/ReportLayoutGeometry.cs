#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportLayoutGeometry
{
    #region Members

    internal const double DefaultMinimumElementSize = 8;

    internal const double DefaultLineThickness = 1;

    private const double SnapToGridSize = 8;

    #endregion

    #region Methods

    internal static bool Intersects( double left, double top, double width, double height, double otherLeft, double otherTop, double otherWidth, double otherHeight )
    {
        return left < otherLeft + otherWidth
            && left + width > otherLeft
            && top < otherTop + otherHeight
            && top + height > otherTop;
    }

    internal static double Clamp( double value, double minimum, double maximum )
    {
        return Math.Min( Math.Max( value, minimum ), maximum );
    }

    internal static ReportSectionDefinition GetSection( ReportDefinition definition, int sectionIndex )
    {
        if ( definition is null || sectionIndex < 0 || sectionIndex >= definition.Sections.Count )
            return null;

        return definition.Sections[sectionIndex];
    }

    internal static double GetSectionOffsetY( ReportDefinition definition, int sectionIndex, Func<int, ReportSectionDefinition, double> getSectionHeight )
    {
        if ( definition is null || sectionIndex <= 0 )
            return 0;

        var y = 0d;

        for ( var i = 0; i < sectionIndex && i < definition.Sections.Count; i++ )
        {
            y += getSectionHeight( i, definition.Sections[i] );
        }

        return y;
    }

    internal static double GetContentHeight( ReportDefinition definition, Func<int, ReportSectionDefinition, double> getSectionHeight )
    {
        if ( definition is null )
            return 0;

        var height = 0d;

        for ( var i = 0; i < definition.Sections.Count; i++ )
        {
            height += getSectionHeight( i, definition.Sections[i] );
        }

        return Math.Max( definition.Page?.Height ?? 0, height );
    }

    internal static double GetMinimumElementHeight( ReportElementDefinition element )
    {
        return GetMinimumElementHeight( element?.Type );
    }

    internal static double GetMinimumElementHeight( ReportElementType? elementType )
    {
        return DefaultMinimumElementSize;
    }

    internal static double GetLineThickness( ReportElementDefinition element )
    {
        return Math.Max( DefaultLineThickness, element?.Thickness ?? DefaultLineThickness );
    }

    internal static double GetElementRenderHeight( ReportElementDefinition element )
    {
        if ( element is null )
            return DefaultMinimumElementSize;

        return Math.Max( GetMinimumElementHeight( element ), element.Height );
    }

    internal static double GetMinimumSectionHeight( ReportSectionDefinition section )
    {
        if ( section is null )
            return DefaultMinimumElementSize;

        var height = DefaultMinimumElementSize;

        foreach ( var element in section.Elements )
        {
            if ( element is not null )
                height = Math.Max( height, element.Y + GetElementRenderHeight( element ) );
        }

        return height;
    }

    internal static void GrowSectionToFitElements( ReportSectionDefinition section )
    {
        if ( section is null )
            return;

        foreach ( var element in section.Elements )
        {
            GrowSectionToFitElement( section, element );
        }
    }

    internal static void GrowSectionToFitElement( ReportSectionDefinition section, ReportElementDefinition element )
    {
        if ( section is null || element is null )
            return;

        section.Height = Math.Max( section.Height, element.Y + GetElementRenderHeight( element ) );
    }

    internal static double SnapToGrid( double value )
    {
        return Math.Max( 0, Math.Round( value / SnapToGridSize ) * SnapToGridSize );
    }

    #endregion
}