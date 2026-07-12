#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise;
using Blazorise.Pdf;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportPdfDocumentBuilder
{
    #region Methods

    internal static PdfDocumentDefinition Build( ReportDefinition definition, object data )
    {
        if ( definition is null )
            return new();

        definition.Page = ReportPageDefinitionHelper.ResolvePage( definition.Page );

        PdfDocumentDefinition document = new()
        {
            Title = definition.Name,
            PageSize = ResolvePageSize( definition.Page.Size ),
            Orientation = ResolveOrientation( definition.Page.Orientation ),
            PageWidth = definition.Page.Width,
            PageHeight = definition.Page.Height,
            Fonts = CollectFonts( definition ).Select( CloneFontFamily ).ToList(),
        };

        foreach ( ReportRenderPage renderPage in ReportPreviewRenderPlanner.BuildRenderPages( definition, data ) )
        {
            PdfPageDefinition page = new()
            {
                Size = document.PageSize,
                Orientation = document.Orientation,
                Width = definition.Page.Width,
                Height = definition.Page.Height,
            };

            AppendRenderSections( page, definition, data, renderPage.HeaderSections, definition.Page.Margins.Left, definition.Page.Margins.Top );
            AppendRenderSections( page, definition, data, renderPage.BodySections, definition.Page.Margins.Left, definition.Page.Margins.Top + GetSectionsHeight( renderPage.HeaderSections ) );

            double footerHeight = GetSectionsHeight( renderPage.FooterSections );
            double footerY = definition.Page.Height - definition.Page.Margins.Bottom - footerHeight;
            AppendRenderSections( page, definition, data, renderPage.FooterSections, definition.Page.Margins.Left, footerY );

            document.Pages.Add( page );
        }

        return document;
    }

    private static void AppendRenderSections( PdfPageDefinition page, ReportDefinition definition, object data, IReadOnlyList<ReportRenderSection> renderSections, double x, double y )
    {
        double sectionY = y;

        foreach ( ReportRenderSection renderSection in renderSections ?? [] )
        {
            if ( renderSection.RenderElements )
            {
                foreach ( ReportElementDefinition element in renderSection.Section.Elements.Where( element => ShouldRenderElement( definition, data, renderSection.Section, element, renderSection.Item ) ) )
                {
                    AppendElement( page.Elements, definition, data, renderSection, element, x, sectionY, 0 );
                }
            }

            sectionY += renderSection.Section.Height;
        }
    }

    private static PdfElementDefinition CreateElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition element, double sectionX, double sectionY, int subreportDepth )
    {
        return element.Type switch
        {
            ReportElementType.Text when element is ReportTextElementDefinition textElement => CreateTextElement( definition, data, renderSection, textElement, sectionX, sectionY ),
            ReportElementType.Field when element is ReportFieldElementDefinition fieldElement => CreateFieldElement( definition, data, renderSection, fieldElement, sectionX, sectionY ),
            ReportElementType.Line => CreateShapeElement( PdfElementType.Line, element, sectionX, sectionY ),
            ReportElementType.Rectangle => CreateShapeElement( PdfElementType.Rectangle, element, sectionX, sectionY ),
            ReportElementType.Image when element is ReportImageElementDefinition imageElement => CreateImageElement( imageElement, sectionX, sectionY ),
            ReportElementType.Table when element is ReportTableElementDefinition tableElement => CreateTableElement( definition, data, renderSection, tableElement, sectionX, sectionY, subreportDepth ),
            _ => null,
        };
    }

    private static void AppendElement( IList<PdfElementDefinition> elements, ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition element, double sectionX, double sectionY, int subreportDepth )
    {
        if ( element is ReportSubreportElementDefinition subreportElement )
        {
            AppendSubreportElements( elements, definition, data, renderSection, subreportElement, sectionX, sectionY, subreportDepth );
            return;
        }

        PdfElementDefinition pdfElement = CreateElement( definition, data, renderSection, element, sectionX, sectionY, subreportDepth );

        if ( pdfElement is not null )
            elements.Add( pdfElement );
    }

    private static PdfElementDefinition CreateTextElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportTextElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Text, element, sectionX, sectionY );
        pdfElement.Text = ReportTextTemplateResolver.ResolveText( definition, data, renderSection.Item, element, renderSection.RunningTotals );
        ApplyTextFormatting( pdfElement, definition, data, renderSection.Section, element );
        pdfElement.Wrap = ReportValueResolver.ResolveCanGrow( element, renderSection.Section, definition, data, renderSection.Item, designMode: false );

        return pdfElement;
    }

    private static PdfElementDefinition CreateFieldElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportFieldElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Text, element, sectionX, sectionY );
        object value = ReportExpressionResolver.ResolveFieldValue( definition, data, renderSection.Item, element, renderSection.RunningTotals );
        pdfElement.Text = ReportDataResolver.FormatValue( value, element.Format );
        ApplyTextFormatting( pdfElement, definition, data, renderSection.Section, element );
        pdfElement.Wrap = ReportValueResolver.ResolveCanGrow( element, renderSection.Section, definition, data, renderSection.Item, designMode: false );

        return pdfElement;
    }

    private static PdfElementDefinition CreateShapeElement( PdfElementType type, ReportElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( type, element, sectionX, sectionY );
        ApplyShapeFormatting( pdfElement, element );

        return pdfElement;
    }

    private static PdfElementDefinition CreateImageElement( ReportImageElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Image, element, sectionX, sectionY );
        ApplyShapeFormatting( pdfElement, element );
        pdfElement.Source = element.Source;
        pdfElement.ImageFit = ResolveImageFit( element.Fit );

        return pdfElement;
    }

    private static PdfElementDefinition CreateTableElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportTableElementDefinition element, double sectionX, double sectionY, int subreportDepth )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Table, element, sectionX, sectionY );
        ApplyShapeFormatting( pdfElement, element );

        foreach ( ReportTableRowDefinition row in element.Rows )
        {
            pdfElement.Rows.Add( new()
            {
                Height = row.Height,
                Cells = element.Cells
                    .Where( cell => cell.RowIndex == element.Rows.IndexOf( row ) )
                    .OrderBy( cell => cell.ColumnIndex )
                    .Select( cell => CreateTableCell( definition, data, renderSection, element, cell, subreportDepth ) )
                    .ToList(),
            } );
        }

        return pdfElement;
    }

    private static PdfTableCellDefinition CreateTableCell( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportTableElementDefinition table, ReportTableCellDefinition cell, int subreportDepth )
    {
        double width = table.Columns
            .Skip( cell.ColumnIndex )
            .Take( cell.ColumnSpan )
            .Sum( column => column.Width );

        PdfTableCellDefinition pdfCell = new()
        {
            Width = width > 0 ? width : 90,
        };

        foreach ( ReportElementDefinition child in cell.Elements.Where( element => ShouldRenderElement( definition, data, renderSection.Section, element, renderSection.Item ) ) )
        {
            AppendElement( pdfCell.Elements, definition, data, renderSection, child, 0, 0, subreportDepth );
        }

        return pdfCell;
    }

    private static void AppendSubreportElements( IList<PdfElementDefinition> elements, ReportDefinition parentDefinition, object parentData, ReportRenderSection parentRenderSection, ReportSubreportElementDefinition element, double sectionX, double sectionY, int subreportDepth )
    {
        if ( subreportDepth > 0 )
            return;

        ReportDefinition subreportDefinition = ReportSubreportResolver.ResolveDefinition( element );

        if ( subreportDefinition is null )
            return;

        object subreportData = ReportSubreportResolver.ResolveData( parentDefinition, parentData, parentRenderSection.Item, element );
        ReportRenderPage renderPage = ReportPreviewRenderPlanner.BuildRenderPages( subreportDefinition, subreportData ).FirstOrDefault();

        if ( renderPage is null )
            return;

        ReportPageDefinition subreportPage = ResolvePageCopy( subreportDefinition.Page );

        double subreportX = sectionX + element.X;
        double subreportY = sectionY + element.Y;
        double contentX = subreportX + subreportPage.Margins.Left;
        double contentY = subreportY + subreportPage.Margins.Top;

        AppendRenderSections( elements, subreportDefinition, subreportData, renderPage.HeaderSections, contentX, contentY, subreportDepth + 1 );
        AppendRenderSections( elements, subreportDefinition, subreportData, renderPage.BodySections, contentX, contentY + GetSectionsHeight( renderPage.HeaderSections ), subreportDepth + 1 );

        double footerHeight = GetSectionsHeight( renderPage.FooterSections );
        double footerY = subreportY + subreportPage.Height - subreportPage.Margins.Bottom - footerHeight;
        AppendRenderSections( elements, subreportDefinition, subreportData, renderPage.FooterSections, contentX, footerY, subreportDepth + 1 );
    }

    private static void AppendRenderSections( IList<PdfElementDefinition> elements, ReportDefinition definition, object data, IReadOnlyList<ReportRenderSection> renderSections, double x, double y, int subreportDepth )
    {
        double sectionY = y;

        foreach ( ReportRenderSection renderSection in renderSections ?? [] )
        {
            if ( renderSection.RenderElements )
            {
                foreach ( ReportElementDefinition element in renderSection.Section.Elements.Where( element => ShouldRenderElement( definition, data, renderSection.Section, element, renderSection.Item ) ) )
                {
                    AppendElement( elements, definition, data, renderSection, element, x, sectionY, subreportDepth );
                }
            }

            sectionY += renderSection.Section.Height;
        }
    }

    private static PdfElementDefinition CreateBaseElement( PdfElementType type, ReportElementDefinition element, double sectionX, double sectionY )
    {
        return new()
        {
            Type = type,
            X = sectionX + element.X,
            Y = sectionY + element.Y,
            Width = element.Width,
            Height = element.Height,
            Orientation = ( element as ReportLineElementDefinition )?.Orientation ?? Orientation.Horizontal,
        };
    }

    private static void ApplyTextFormatting( PdfElementDefinition pdfElement, ReportDefinition definition, object data, ReportBandDefinition section, ReportElementDefinition element )
    {
        ApplyShapeFormatting( pdfElement, element );

        pdfElement.Font.Family = element.Font?.Family ?? Fonts.Helvetica;
        pdfElement.Font.Size = element.Font?.Size ?? 12;
        pdfElement.Font.Color = ResolveColor( element.Font?.Color ?? ReportColors.Black );
        pdfElement.Font.Alignment = ReportElementDefinitionHelper.ResolveTextAlignment( element, definition, data, section );
        pdfElement.Font.VerticalAlignment = ReportElementDefinitionHelper.ResolveVerticalAlignment( element );
        pdfElement.Font.Bold = element.Font?.Bold == true;
        pdfElement.Font.Italic = element.Font?.Italic == true;
    }

    private static void ApplyShapeFormatting( PdfElementDefinition pdfElement, ReportElementDefinition element )
    {
        pdfElement.Border.Color = ResolveColor( element.Border?.Color ?? ReportColors.Black );
        pdfElement.Border.Width = ResolveBorderWidth( element );
        pdfElement.Border.Style = ResolveBorderStyle( element.Border?.Style ?? ReportBorderStyle.Default );
        pdfElement.Appearance.BackgroundColor = ResolveColor( element.Appearance?.BackgroundColor ?? ReportColor.Default );
    }

    private static double ResolveBorderWidth( ReportElementDefinition element )
    {
        if ( element is ReportLineElementDefinition )
            return ReportLayoutGeometry.GetLineThickness( element );

        if ( ShouldRequireExplicitBorder( element.Type ) && !HasDefinedBorder( element.Border ) )
            return 0;

        return element.Border?.Width ?? 1;
    }

    private static bool ShouldRequireExplicitBorder( ReportElementType elementType )
    {
        return elementType is ReportElementType.Text or ReportElementType.Field or ReportElementType.Image or ReportElementType.Table or ReportElementType.Subreport;
    }

    private static bool HasDefinedBorder( ReportBorderDefinition border )
    {
        return border?.Width is not null || !( border?.Color ?? ReportColor.Default ).IsDefault || border is not null && border.Style != ReportBorderStyle.Default;
    }

    private static PdfBorderStyle ResolveBorderStyle( ReportBorderStyle style )
    {
        return style switch
        {
            ReportBorderStyle.Dashed => PdfBorderStyle.Dashed,
            ReportBorderStyle.Dotted => PdfBorderStyle.Dotted,
            _ => PdfBorderStyle.Solid,
        };
    }

    private static PdfImageFit ResolveImageFit( ReportImageFit fit )
    {
        return fit switch
        {
            ReportImageFit.Cover => PdfImageFit.Cover,
            ReportImageFit.Fill => PdfImageFit.Fill,
            ReportImageFit.None => PdfImageFit.None,
            ReportImageFit.ScaleDown => PdfImageFit.Scale,
            _ => PdfImageFit.Contain,
        };
    }

    private static bool ShouldRenderElement( ReportDefinition definition, object data, ReportBandDefinition section, ReportElementDefinition element, object item )
    {
        return !ReportValueResolver.ResolveSuppress( element, section, definition, data, item );
    }

    private static double GetSectionsHeight( IReadOnlyList<ReportRenderSection> renderSections )
    {
        return renderSections?.Sum( renderSection => renderSection.Section.Height ) ?? 0;
    }

    private static PdfPageSize ResolvePageSize( ReportPageSize size )
    {
        return size switch
        {
            ReportPageSize.Letter => PdfPageSize.Letter,
            ReportPageSize.Custom => PdfPageSize.Custom,
            _ => PdfPageSize.A4,
        };
    }

    private static PdfOrientation ResolveOrientation( ReportOrientation orientation )
    {
        return orientation == ReportOrientation.Landscape
            ? PdfOrientation.Landscape
            : PdfOrientation.Portrait;
    }

    private static string ResolveColor( ReportColor color )
    {
        return color.ToCssString();
    }

    private static ReportPageDefinition ResolvePageCopy( ReportPageDefinition page )
    {
        return ReportPageDefinitionHelper.ResolvePage( new()
        {
            Size = page?.Size ?? ReportPageSize.A4,
            Orientation = page?.Orientation ?? ReportOrientation.Portrait,
            Width = page?.Width ?? 0,
            Height = page?.Height ?? 0,
            Margins = new()
            {
                Left = page?.Margins?.Left ?? 0,
                Top = page?.Margins?.Top ?? 0,
                Right = page?.Margins?.Right ?? 0,
                Bottom = page?.Margins?.Bottom ?? 0,
            },
        } );
    }

    private static IReadOnlyList<FontFamily> CollectFonts( ReportDefinition definition )
    {
        var fonts = new List<FontFamily>();
        var names = new HashSet<string>( System.StringComparer.OrdinalIgnoreCase );

        CollectFonts( definition, fonts, names, 0 );

        return fonts;
    }

    private static void CollectFonts( ReportDefinition definition, List<FontFamily> fonts, HashSet<string> names, int subreportDepth )
    {
        if ( definition is null )
            return;

        foreach ( FontFamily font in definition.Fonts ?? [] )
        {
            if ( !string.IsNullOrWhiteSpace( font?.Name ) && names.Add( font.Name ) )
                fonts.Add( font );
        }

        if ( subreportDepth > 0 )
            return;

        foreach ( ReportSubreportElementDefinition subreport in EnumerateSubreports( definition ) )
        {
            CollectFonts( subreport.Report, fonts, names, subreportDepth + 1 );
        }
    }

    private static IEnumerable<ReportSubreportElementDefinition> EnumerateSubreports( ReportDefinition definition )
    {
        foreach ( ReportBandDefinition section in definition.Bands ?? [] )
        {
            foreach ( ReportElementDefinition element in section.Elements ?? [] )
            {
                foreach ( ReportSubreportElementDefinition subreport in EnumerateSubreports( element ) )
                    yield return subreport;
            }
        }
    }

    private static IEnumerable<ReportSubreportElementDefinition> EnumerateSubreports( ReportElementDefinition element )
    {
        if ( element is ReportSubreportElementDefinition subreport )
        {
            yield return subreport;
            yield break;
        }

        if ( element is ReportTableElementDefinition table )
        {
            foreach ( ReportTableCellDefinition cell in table.Cells ?? [] )
            {
                foreach ( ReportElementDefinition child in cell.Elements ?? [] )
                {
                    foreach ( ReportSubreportElementDefinition childSubreport in EnumerateSubreports( child ) )
                        yield return childSubreport;
                }
            }
        }
    }

    private static FontFamily CloneFontFamily( FontFamily font )
    {
        if ( font is null )
            return null;

        return new()
        {
            Name = font.Name,
            DisplayName = font.DisplayName,
            CssFamily = font.CssFamily,
            Regular = CloneFontSource( font.Regular ),
            Bold = CloneFontSource( font.Bold ),
            Italic = CloneFontSource( font.Italic ),
            BoldItalic = CloneFontSource( font.BoldItalic ),
            Visible = font.Visible,
        };
    }

    private static FontSource CloneFontSource( FontSource source )
    {
        if ( source is null )
            return null;

        return new()
        {
            Url = source.Url,
            Data = source.Data,
            FileName = source.FileName,
            Format = source.Format,
        };
    }

    #endregion
}