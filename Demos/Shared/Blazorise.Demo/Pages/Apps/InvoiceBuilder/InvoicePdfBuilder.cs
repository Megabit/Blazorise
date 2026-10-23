#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Pdf;
#endregion

namespace Blazorise.Demo.Pages.Apps.InvoiceBuilder;

/// <summary>
/// Lays out the demo invoice on A4 pages using native Blazorise PDF elements.
/// </summary>
internal sealed class InvoicePdfBuilder
{
    #region Members

    private const double Margin = 40;

    private const double ContentWidth = 515;

    private const double ContentBottom = 760;

    private const double LineHeight = 14;

    private const string Ink = "#202938";

    private const string Muted = "#64748b";

    private const string Accent = "#2447a8";

    private const string Rule = "#e2e8f0";

    private readonly InvoiceBuilderPage.InvoiceDraft invoice;

    private readonly Func<decimal, string> formatMoney;

    private readonly PdfDocumentBuilder document = PdfDocumentBuilder.Create().PageSetup( PdfPageSize.A4 );

    private readonly List<PdfPageBuilder> pages = [];

    private PdfPageBuilder page;

    private double y;

    private bool hasTableHeader;

    #endregion

    #region Constructors

    public InvoicePdfBuilder( InvoiceBuilderPage.InvoiceDraft invoice, Func<decimal, string> formatMoney )
    {
        this.invoice = invoice;
        this.formatMoney = formatMoney;
    }

    #endregion

    #region Methods

    public PdfDocumentDefinition Build()
    {
        document.Title( $"{invoice.DocumentType} {invoice.Number}" );
        NewPage();
        DrawHeader();
        DrawParties();

        foreach ( var field in invoice.CustomFields )
        {
            DrawSection( field.Name, field.Text );
        }

        DrawSection( "Description", invoice.Description );

        if ( invoice.Items.Count == 0 )
        {
            EnsureSpace( 30 );
            DrawTableHeader();
        }

        foreach ( var item in invoice.Items )
        {
            DrawItem( item );
        }

        DrawTotals();
        DrawSection( "Payment details", invoice.PaymentDetails );
        DrawSection( "Terms", invoice.Terms );

        for ( var index = 0; index < pages.Count; index++ )
        {
            page = pages[index];
            page.Line( Margin, 790, ContentWidth, 0 ).BorderColor( Rule ).BorderWidth( 0.5 );
            DrawText( $"{invoice.DocumentType} {invoice.Number}", Margin, 800, 350, 9, color: Muted );
            DrawText( $"Page {index + 1} of {pages.Count}", 440, 800, 115, 9, alignment: TextAlignment.End, color: Muted );
        }

        return document.Build();
    }

    private void NewPage()
    {
        document.Page( newPage => page = newPage );
        pages.Add( page );
        y = Margin;
        hasTableHeader = false;

        if ( pages.Count > 1 )
        {
            DrawText( $"{invoice.DocumentType} {invoice.Number}", Margin, y, ContentWidth, 12, bold: true );
            page.Line( Margin, y + 26, ContentWidth, 0 ).BorderColor( Rule ).BorderWidth( 0.5 );
            y += 42;
        }
    }

    private void EnsureSpace( double height )
    {
        if ( y + height > ContentBottom )
        {
            NewPage();
        }
    }

    private void DrawHeader()
    {
        if ( !string.IsNullOrWhiteSpace( invoice.Logo ) )
        {
            page.Image( invoice.Logo, Margin, y, 180, 65 ).Fit( PdfImageFit.Contain );
        }

        DrawText( invoice.DocumentType.ToUpperInvariant(), 320, y, 235, 26, bold: true, alignment: TextAlignment.End, color: Accent );
        DrawText( $"# {invoice.Number}", 320, y + 35, 235, 12, alignment: TextAlignment.End, color: Muted );
        y += 65;
        DrawHeaderDetail( "Issue date", invoice.IssueDate.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture ) );
        DrawHeaderDetail( "Due date", invoice.DueDate.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture ) );
        DrawHeaderDetail( "PO number", invoice.PurchaseOrderNumber );
        DrawHeaderDetail( "Payment Terms", invoice.PaymentTerms );

        y += 15;
        page.Line( Margin, y, ContentWidth, 0 ).BorderColor( Accent ).BorderWidth( 1.5 );
        y += 22;
    }

    private void DrawHeaderDetail( string label, string text )
    {
        if ( string.IsNullOrWhiteSpace( text ) )
        {
            return;
        }

        var lines = WrapText( text, 125 );

        DrawText( label, 330, y, 95, 10, color: Muted );

        foreach ( var line in lines )
        {
            DrawText( line, 430, y, 125, alignment: TextAlignment.End );
            y += LineHeight;
        }

        y += 6;
    }

    private void DrawParties()
    {
        var sender = WrapText( $"{invoice.Sender.CompanyName}\n{invoice.Sender.FormatContactDetails()}", 240 );
        var recipient = WrapText( $"{invoice.Recipient.CompanyName}\n{invoice.Recipient.FormatContactDetails()}", 240 );
        var lineCount = Math.Max( sender.Count, recipient.Count );
        var offset = 0;

        while ( offset < lineCount )
        {
            EnsureSpace( 50 );
            DrawText( offset == 0 ? "FROM" : "FROM (continued)", Margin, y, 240, 9, bold: true, color: Muted );
            DrawText( offset == 0 ? "BILL TO" : "BILL TO (continued)", 315, y, 240, 9, bold: true, color: Muted );
            y += 22;

            var count = Math.Min( lineCount - offset, (int)( ( ContentBottom - y ) / LineHeight ) );

            for ( var index = offset; index < offset + count; index++ )
            {
                if ( index < sender.Count )
                {
                    DrawText( sender[index], Margin, y, 240, bold: index == 0 );
                }

                if ( index < recipient.Count )
                {
                    DrawText( recipient[index], 315, y, 240, bold: index == 0 );
                }

                y += LineHeight;
            }

            offset += count;

            if ( offset < lineCount )
            {
                NewPage();
            }
        }

        y += 22;
    }

    private void DrawTableHeader()
    {
        page.Rectangle( Margin, y, ContentWidth, 30 ).BackgroundColor( Ink ).BorderWidth( 0 );
        DrawText( "ITEM / DESCRIPTION", 50, y + 8, 200, 9, bold: true, color: "#ffffff" );
        DrawText( "QTY", 260, y + 8, 40, 9, bold: true, alignment: TextAlignment.End, color: "#ffffff" );
        DrawText( "UNIT PRICE", 310, y + 8, 90, 9, bold: true, alignment: TextAlignment.End, color: "#ffffff" );
        DrawText( "TAX", 410, y + 8, 40, 9, bold: true, alignment: TextAlignment.End, color: "#ffffff" );
        DrawText( "AMOUNT", 460, y + 8, 85, 9, bold: true, alignment: TextAlignment.End, color: "#ffffff" );
        y += 30;
        hasTableHeader = true;
    }

    private void DrawItem( InvoiceBuilderPage.InvoiceItem item )
    {
        var nameLines = WrapText( string.IsNullOrWhiteSpace( item.Name ) ? "Untitled item" : item.Name, 200 );
        var lines = new List<string>( nameLines );

        if ( !string.IsNullOrWhiteSpace( item.Description ) )
        {
            lines.AddRange( WrapText( item.Description, 200 ) );
        }

        var offset = 0;
        var rowHeight = lines.Count * LineHeight + 20;

        // Keep ordinary items together. Only split a row if it cannot fit on a fresh page.
        if ( y + Math.Min( rowHeight, 648 ) + ( hasTableHeader ? 0 : 30 ) > ContentBottom )
        {
            NewPage();
        }

        if ( !hasTableHeader )
        {
            DrawTableHeader();
        }

        while ( offset < lines.Count )
        {
            var count = Math.Min( lines.Count - offset, (int)( ( ContentBottom - y - 20 ) / LineHeight ) );

            if ( count < 1 )
            {
                NewPage();
                DrawTableHeader();
                continue;
            }

            var height = count * LineHeight + 20;

            page.Rectangle( Margin, y, ContentWidth, height ).BackgroundColor( "#f8fafc" ).BorderWidth( 0 );

            for ( var index = 0; index < count; index++ )
            {
                var isName = offset + index < nameLines.Count;
                DrawText( lines[offset + index], 50, y + 10 + index * LineHeight, 200, bold: isName, color: isName ? Ink : Muted );
            }

            if ( offset == 0 )
            {
                DrawText( ( item.Quantity ?? 0 ).ToString( "0.##", CultureInfo.InvariantCulture ), 260, y + 10, 40, alignment: TextAlignment.End );
                DrawText( formatMoney( item.UnitPrice ?? 0 ), 310, y + 10, 90, alignment: TextAlignment.End );
                DrawText( $"{item.TaxRate:0.##}%", 410, y + 10, 40, alignment: TextAlignment.End );
                DrawText( formatMoney( item.Subtotal ), 460, y + 10, 85, bold: true, alignment: TextAlignment.End );
            }
            else
            {
                DrawText( "continued", 460, y + 10, 85, 8, alignment: TextAlignment.End, color: Muted );
            }

            y += height;
            page.Line( Margin, y, ContentWidth, 0 ).BorderColor( Rule ).BorderWidth( 0.5 );
            offset += count;
        }
    }

    private void DrawTotals()
    {
        var subtotal = invoice.Items.Sum( item => item.Subtotal );
        var tax = invoice.Items.Sum( item => decimal.Round( item.Subtotal * item.TaxRate / 100, 2 ) );
        var total = subtotal + tax;
        var hasPayment = invoice.AmountPaid != 0 || !string.IsNullOrWhiteSpace( invoice.PaymentDetails );

        EnsureSpace( hasPayment ? 182 : 134 );
        y += 20;
        DrawText( $"SUMMARY ({invoice.Currency})", 315, y, 240, 9, bold: true, color: Muted );
        y += 22;
        DrawTotalRow( "Subtotal", subtotal );
        DrawTotalRow( "Tax", tax );
        DrawTotalRow( "Total", total, true );

        if ( hasPayment )
        {
            DrawTotalRow( "Paid", invoice.AmountPaid );
            DrawTotalRow( total >= invoice.AmountPaid ? "Balance due" : "Credit", Math.Abs( total - invoice.AmountPaid ), true );
        }

        y += 20;
    }

    private void DrawTotalRow( string label, decimal amount, bool highlight = false )
    {
        if ( highlight )
        {
            page.Rectangle( 305, y - 3, 250, 24 ).BackgroundColor( "#edf2ff" ).BorderWidth( 0 );
        }

        DrawText( label, 315, y, 100, 10, bold: highlight, color: highlight ? Accent : Muted );
        DrawText( formatMoney( amount ), 420, y, 125, 10, bold: highlight, alignment: TextAlignment.End, color: highlight ? Accent : Ink );
        y += 24;
    }

    private void DrawSection( string heading, string text )
    {
        if ( string.IsNullOrWhiteSpace( text ) )
        {
            return;
        }

        var lines = WrapText( text, ContentWidth );
        var offset = 0;

        while ( offset < lines.Count )
        {
            EnsureSpace( 50 );
            DrawText( offset == 0 ? heading : $"{heading} (continued)", Margin, y, ContentWidth, 10, bold: true );
            y += 22;

            var count = Math.Min( lines.Count - offset, (int)( ( ContentBottom - y ) / LineHeight ) );

            for ( var index = 0; index < count; index++ )
            {
                DrawText( lines[offset + index], Margin, y, ContentWidth, color: Muted );
                y += LineHeight;
            }

            offset += count;

            if ( offset < lines.Count )
            {
                NewPage();
            }
        }

        y += 18;
    }

    private void DrawText( string text, double x, double top, double width, double size = 9.5, bool bold = false, TextAlignment alignment = TextAlignment.Start, string color = Ink )
    {
        // Fit single-line values, including large amounts and long invoice numbers, inside their column.
        var units = ( text ?? string.Empty ).Sum( GlyphWidth );
        var fontSize = units > 0 ? Math.Min( size, width / units ) : size;

        page.Text( text ?? string.Empty, x, top, width, Math.Max( 16, size * 1.4 ) )
            .FontFamily( "Helvetica" )
            .FontSize( fontSize )
            .Bold( bold )
            .TextColor( color )
            .TextAlignment( alignment )
            .Wrap( false );
    }

    private static List<string> WrapText( string text, double width )
    {
        var lines = new List<string>();
        var maxUnits = width / 9.5;

        foreach ( var paragraph in ( text ?? string.Empty ).Replace( "\r\n", "\n" ).Replace( '\r', '\n' ).Split( '\n' ) )
        {
            var remaining = paragraph.Trim();

            while ( remaining.Length > 0 )
            {
                var length = 0;
                var units = 0d;

                while ( length < remaining.Length && units + GlyphWidth( remaining[length] ) <= maxUnits )
                {
                    units += GlyphWidth( remaining[length++] );
                }

                if ( length < remaining.Length )
                {
                    var wordEnd = remaining.LastIndexOf( ' ', Math.Max( 0, length - 1 ), length );

                    if ( wordEnd > 0 )
                    {
                        length = wordEnd;
                    }
                }

                length = Math.Max( 1, length );
                lines.Add( remaining[..length].TrimEnd() );
                remaining = remaining[length..].TrimStart();
            }

            if ( string.IsNullOrWhiteSpace( paragraph ) )
            {
                lines.Add( string.Empty );
            }
        }

        return lines;
    }

    // Conservative Helvetica widths allow shared line breaks for regular and bold text.
    private static double GlyphWidth( char character )
    {
        if ( character == ' ' )
        {
            return 0.32;
        }

        if ( "ilIjt.,:;!'|()[]-".Contains( character ) )
        {
            return 0.36;
        }

        if ( "MWmw@%&".Contains( character ) || character > 126 )
        {
            return 1.05;
        }

        return char.IsUpper( character ) ? 0.82 : 0.65;
    }

    #endregion
}