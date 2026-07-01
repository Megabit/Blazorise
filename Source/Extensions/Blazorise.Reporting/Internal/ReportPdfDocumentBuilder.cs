#region Using directives
using System.Collections.Generic;
using System.Linq;
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
                    PdfElementDefinition pdfElement = CreateElement( definition, data, renderSection, element, x, sectionY );

                    if ( pdfElement is not null )
                        page.Elements.Add( pdfElement );
                }
            }

            sectionY += renderSection.Section.Height;
        }
    }

    private static PdfElementDefinition CreateElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition element, double sectionX, double sectionY )
    {
        return element.Type switch
        {
            ReportElementType.Text => CreateTextElement( definition, data, renderSection, element, sectionX, sectionY ),
            ReportElementType.Field => CreateFieldElement( definition, data, renderSection, element, sectionX, sectionY ),
            ReportElementType.Line => CreateShapeElement( PdfElementType.Line, element, sectionX, sectionY ),
            ReportElementType.Rectangle => CreateShapeElement( PdfElementType.Rectangle, element, sectionX, sectionY ),
            ReportElementType.Image => CreateImageElement( element, sectionX, sectionY ),
            ReportElementType.Table => CreateTableElement( definition, data, renderSection, element, sectionX, sectionY ),
            _ => null,
        };
    }

    private static PdfElementDefinition CreateTextElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Text, element, sectionX, sectionY );
        pdfElement.Text = ReportTextTemplateResolver.ResolveText( definition, data, renderSection.Item, element, renderSection.RunningTotals );
        ApplyTextFormatting( pdfElement, definition, data, renderSection.Section, element );

        return pdfElement;
    }

    private static PdfElementDefinition CreateFieldElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Text, element, sectionX, sectionY );
        object value = ReportExpressionResolver.ResolveFieldValue( definition, data, renderSection.Item, element, renderSection.RunningTotals );
        pdfElement.Text = ReportDataResolver.FormatValue( value, element.Format );
        ApplyTextFormatting( pdfElement, definition, data, renderSection.Section, element );

        return pdfElement;
    }

    private static PdfElementDefinition CreateShapeElement( PdfElementType type, ReportElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( type, element, sectionX, sectionY );
        ApplyShapeFormatting( pdfElement, element );

        return pdfElement;
    }

    private static PdfElementDefinition CreateImageElement( ReportElementDefinition element, double sectionX, double sectionY )
    {
        PdfElementDefinition pdfElement = CreateBaseElement( PdfElementType.Image, element, sectionX, sectionY );
        pdfElement.Source = element.Source;
        pdfElement.Text = element.Text ?? element.Name;

        return pdfElement;
    }

    private static PdfElementDefinition CreateTableElement( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition element, double sectionX, double sectionY )
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
                    .Select( cell => CreateTableCell( definition, data, renderSection, element, cell ) )
                    .ToList(),
            } );
        }

        return pdfElement;
    }

    private static PdfTableCellDefinition CreateTableCell( ReportDefinition definition, object data, ReportRenderSection renderSection, ReportElementDefinition table, ReportTableCellDefinition cell )
    {
        double width = table.Columns
            .Skip( cell.ColumnIndex )
            .Take( cell.ColumnSpan )
            .Sum( column => column.Width );

        PdfTableCellDefinition pdfCell = new()
        {
            Width = width > 0 ? width : 90,
            ColumnSpan = cell.ColumnSpan,
            RowSpan = cell.RowSpan,
        };

        foreach ( ReportElementDefinition child in cell.Elements.Where( element => ShouldRenderElement( definition, data, renderSection.Section, element, renderSection.Item ) ) )
        {
            PdfElementDefinition pdfChild = CreateElement( definition, data, renderSection, child, 0, 0 );

            if ( pdfChild is not null )
                pdfCell.Elements.Add( pdfChild );
        }

        return pdfCell;
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
        };
    }

    private static void ApplyTextFormatting( PdfElementDefinition pdfElement, ReportDefinition definition, object data, ReportSectionDefinition section, ReportElementDefinition element )
    {
        ApplyShapeFormatting( pdfElement, element );

        pdfElement.Font.Family = element.Font?.Family ?? "Helvetica";
        pdfElement.Font.Size = element.Font?.Size ?? 12;
        pdfElement.Font.Color = ResolveColor( element.Font?.Color ?? ReportColors.Black );
        pdfElement.Font.Alignment = ResolveTextAlignment( ReportElementDefinitionHelper.ResolveTextAlignment( element, definition, data, section ) );
        pdfElement.Font.VerticalAlignment = ResolveVerticalAlignment( ReportElementDefinitionHelper.ResolveVerticalAlignment( element ) );
        pdfElement.Font.Bold = element.Font?.Bold == true;
        pdfElement.Font.Italic = element.Font?.Italic == true;
    }

    private static void ApplyShapeFormatting( PdfElementDefinition pdfElement, ReportElementDefinition element )
    {
        pdfElement.Border.Color = ResolveColor( element.Border?.Color ?? ReportColors.Black );
        pdfElement.Border.Width = ResolveBorderWidth( element );
        pdfElement.Appearance.BackgroundColor = ResolveColor( element.Appearance?.BackgroundColor ?? ReportColor.Default );
    }

    private static double ResolveBorderWidth( ReportElementDefinition element )
    {
        if ( ShouldRequireExplicitBorder( element.Type ) && !HasDefinedBorder( element.Border ) )
            return 0;

        return element.Border?.Width ?? 1;
    }

    private static bool ShouldRequireExplicitBorder( ReportElementType elementType )
    {
        return elementType is ReportElementType.Text or ReportElementType.Field or ReportElementType.Table;
    }

    private static bool HasDefinedBorder( ReportBorderDefinition border )
    {
        return border?.Width is not null || !( border?.Color ?? ReportColor.Default ).IsDefault;
    }

    private static bool ShouldRenderElement( ReportDefinition definition, object data, ReportSectionDefinition section, ReportElementDefinition element, object item )
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

    private static PdfTextAlignment ResolveTextAlignment( TextAlignment alignment )
    {
        return alignment switch
        {
            TextAlignment.Center => PdfTextAlignment.Center,
            TextAlignment.End => PdfTextAlignment.End,
            _ => PdfTextAlignment.Start,
        };
    }

    private static PdfVerticalAlignment ResolveVerticalAlignment( VerticalAlignment alignment )
    {
        return alignment switch
        {
            VerticalAlignment.Middle => PdfVerticalAlignment.Middle,
            VerticalAlignment.Bottom => PdfVerticalAlignment.Bottom,
            _ => PdfVerticalAlignment.Top,
        };
    }

    private static string ResolveColor( ReportColor color )
    {
        return color.ToCssString();
    }

    #endregion
}