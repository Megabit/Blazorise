#region Using directives
using System;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportPageDefinitionHelper
{
    #region Methods

    internal static ReportPageDefinition ResolvePage( ReportPageDefinition page )
    {
        page ??= new();
        page.Margins ??= new();

        NormalizeMargins( page.Margins );

        if ( page.Size != ReportPageSize.Custom || page.Width <= 0 || page.Height <= 0 )
        {
            ApplySize( page, page.Size );
        }

        page.Width = Math.Max( 1, page.Width );
        page.Height = Math.Max( 1, page.Height );

        return page;
    }

    internal static void ApplySize( ReportPageDefinition page, ReportPageSize size )
    {
        page ??= new();
        page.Size = size;

        if ( size == ReportPageSize.Custom )
        {
            if ( page.Width <= 0 || page.Height <= 0 )
            {
                (double defaultWidth, double defaultHeight) = GetPageSize( ReportPageSize.A4 );

                if ( page.Orientation == ReportOrientation.Landscape )
                {
                    (defaultWidth, defaultHeight) = (defaultHeight, defaultWidth);
                }

                page.Width = defaultWidth;
                page.Height = defaultHeight;
            }

            page.Width = Math.Max( 1, page.Width );
            page.Height = Math.Max( 1, page.Height );
            return;
        }

        (double width, double height) = GetPageSize( size );

        if ( page.Orientation == ReportOrientation.Landscape )
        {
            (width, height) = (height, width);
        }

        page.Width = width;
        page.Height = height;
    }

    internal static void ApplyOrientation( ReportPageDefinition page, ReportOrientation orientation )
    {
        page ??= new();
        page.Orientation = orientation;

        if ( page.Size != ReportPageSize.Custom )
        {
            ApplySize( page, page.Size );
            return;
        }

        if ( orientation == ReportOrientation.Portrait && page.Width > page.Height )
        {
            (page.Width, page.Height) = (page.Height, page.Width);
        }
        else if ( orientation == ReportOrientation.Landscape && page.Height > page.Width )
        {
            (page.Width, page.Height) = (page.Height, page.Width);
        }
    }

    internal static double GetContentWidth( ReportPageDefinition page )
    {
        page = ResolvePage( page );

        return Math.Max( 1, page.Width - page.Margins.Left - page.Margins.Right );
    }

    internal static double GetContentHeight( ReportPageDefinition page )
    {
        page = ResolvePage( page );

        return Math.Max( 1, page.Height - page.Margins.Top - page.Margins.Bottom );
    }

    private static (double Width, double Height) GetPageSize( ReportPageSize size )
    {
        return size switch
        {
            ReportPageSize.A3 => (841.8897637795276d, 1190.5511811023623d),
            ReportPageSize.A5 => (419.52755905511816d, 595.2755905511812d),
            ReportPageSize.Letter => (612d, 792d),
            ReportPageSize.Legal => (612d, 1008d),
            _ => (595.2755905511812d, 841.8897637795276d),
        };
    }

    private static void NormalizeMargins( ReportPageMarginsDefinition margins )
    {
        margins.Left = Math.Max( 0, margins.Left );
        margins.Top = Math.Max( 0, margins.Top );
        margins.Right = Math.Max( 0, margins.Right );
        margins.Bottom = Math.Max( 0, margins.Bottom );
    }

    #endregion
}