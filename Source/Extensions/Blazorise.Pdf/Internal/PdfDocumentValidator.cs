#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

internal static class PdfDocumentValidator
{
    #region Members

    private const double MaxPageDimension = 14400;

    private const double MaxCoordinate = 1000000;

    private const int MaxElementDepth = 64;

    private const double DefaultFontSize = 12;

    private const double DefaultRowHeight = 24;

    private const double DefaultCellWidth = 90;

    #endregion

    #region Methods

    internal static void Validate( PdfDocumentDefinition document, PdfGenerationOptions options, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();
        NormalizeOptions( options );

        document.Pages ??= [];
        document.Fonts ??= [];
        document.PageSize = NormalizeEnum( document.PageSize, PdfPageSize.A4 );
        document.Orientation = NormalizeEnum( document.Orientation, PdfOrientation.Portrait );

        double customDocumentWidth = NormalizePageDimension( document.PageWidth, PdfPageMetrics.A4Width );
        double customDocumentHeight = NormalizePageDimension( document.PageHeight, PdfPageMetrics.A4Height );
        (double documentWidth, double documentHeight) = PdfPageMetrics.Resolve( document.PageSize, document.Orientation, customDocumentWidth, customDocumentHeight );
        document.PageWidth = NormalizePageDimension( documentWidth, PdfPageMetrics.A4Width );
        document.PageHeight = NormalizePageDimension( documentHeight, PdfPageMetrics.A4Height );

        for ( int pageIndex = document.Pages.Count - 1; pageIndex >= 0; pageIndex-- )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ( document.Pages[pageIndex] is null )
                document.Pages.RemoveAt( pageIndex );
        }

        int pageCount = Math.Max( 1, document.Pages.Count );

        if ( pageCount > options.MaxPages )
            throw new InvalidDataException( $"The PDF document contains {pageCount} pages, exceeding the configured limit of {options.MaxPages}." );

        int nodeCount = 0;
        long textLength = document.Title?.Length ?? 0;

        ValidateTextLength( textLength, options );
        NormalizeFonts( document.Fonts, options, cancellationToken, ref nodeCount );

        HashSet<PdfElementDefinition> ancestors = new( ReferenceEqualityComparer.Instance );

        for ( int pageIndex = document.Pages.Count - 1; pageIndex >= 0; pageIndex-- )
        {
            cancellationToken.ThrowIfCancellationRequested();
            PdfPageDefinition page = document.Pages[pageIndex];

            CountNode( ref nodeCount, options );
            page.Size = NormalizeEnum( page.Size, PdfPageSize.Custom );
            page.Orientation = NormalizeEnum( page.Orientation, document.Orientation );

            double customPageWidth = NormalizePageDimension( page.Width, document.PageWidth );
            double customPageHeight = NormalizePageDimension( page.Height, document.PageHeight );
            (double pageWidth, double pageHeight) = PdfPageMetrics.Resolve( page.Size, page.Orientation, customPageWidth, customPageHeight );
            page.Width = NormalizePageDimension( pageWidth, document.PageWidth );
            page.Height = NormalizePageDimension( pageHeight, document.PageHeight );
            page.Elements ??= [];

            for ( int elementIndex = page.Elements.Count - 1; elementIndex >= 0; elementIndex-- )
            {
                if ( !NormalizeElement( page.Elements[elementIndex], $"Pages[{pageIndex}].Elements[{elementIndex}]", options, ancestors, 0, cancellationToken, ref nodeCount, ref textLength ) )
                    page.Elements.RemoveAt( elementIndex );
            }
        }
    }

    private static void NormalizeOptions( PdfGenerationOptions options )
    {
        if ( options.MaxPages <= 0 )
            options.MaxPages = PdfGenerationOptions.DefaultMaxPages;

        if ( options.MaxDefinitionNodes <= 0 )
            options.MaxDefinitionNodes = PdfGenerationOptions.DefaultMaxDefinitionNodes;

        if ( options.MaxTextLength <= 0 )
            options.MaxTextLength = PdfGenerationOptions.DefaultMaxTextLength;

        if ( options.MaxResourceSize <= 0 || options.MaxResourceSize > int.MaxValue )
            options.MaxResourceSize = PdfGenerationOptions.DefaultMaxResourceSize;

        if ( options.MaxTotalResourceSize <= 0 )
            options.MaxTotalResourceSize = PdfGenerationOptions.DefaultMaxTotalResourceSize;

        if ( options.MaxImagePixels <= 0 )
            options.MaxImagePixels = PdfGenerationOptions.DefaultMaxImagePixels;
    }

    private static void NormalizeFonts( List<FontFamily> fonts, PdfGenerationOptions options, CancellationToken cancellationToken, ref int nodeCount )
    {
        for ( int fontIndex = fonts.Count - 1; fontIndex >= 0; fontIndex-- )
        {
            cancellationToken.ThrowIfCancellationRequested();
            FontFamily font = fonts[fontIndex];

            if ( font is null || string.IsNullOrWhiteSpace( font.Name ) || !IsValidFontSource( font.Regular, options, cancellationToken, ref nodeCount ) )
            {
                fonts.RemoveAt( fontIndex );
                continue;
            }

            CountNode( ref nodeCount, options );

            if ( !IsValidFontSource( font.Bold, options, cancellationToken, ref nodeCount ) )
                font.Bold = null;

            if ( !IsValidFontSource( font.Italic, options, cancellationToken, ref nodeCount ) )
                font.Italic = null;

            if ( !IsValidFontSource( font.BoldItalic, options, cancellationToken, ref nodeCount ) )
                font.BoldItalic = null;
        }
    }

    private static bool IsValidFontSource( FontSource source, PdfGenerationOptions options, CancellationToken cancellationToken, ref int nodeCount )
    {
        if ( source is null )
            return false;

        cancellationToken.ThrowIfCancellationRequested();
        CountNode( ref nodeCount, options );

        if ( source.Format is not FontFormat.TrueType and not FontFormat.OpenType )
            return false;

        if ( source.Data is { Length: > 0 } data )
        {
            if ( data.LongLength > options.MaxResourceSize )
                throw new InvalidDataException( $"A PDF font source exceeds the configured resource limit of {options.MaxResourceSize} bytes." );

            return true;
        }

        return !string.IsNullOrWhiteSpace( source.FileName ) || !string.IsNullOrWhiteSpace( source.Url );
    }

    private static bool NormalizeElement( PdfElementDefinition element, string path, PdfGenerationOptions options, HashSet<PdfElementDefinition> ancestors, int depth, CancellationToken cancellationToken, ref int nodeCount, ref long textLength )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ( element is null || depth >= MaxElementDepth || !ancestors.Add( element ) )
            return false;

        try
        {
            CountNode( ref nodeCount, options );
            element.Type = NormalizeEnum( element.Type, PdfElementType.Text );
            element.Orientation = NormalizeEnum( element.Orientation, Orientation.Horizontal );
            element.ImageFit = NormalizeEnum( element.ImageFit, PdfImageFit.Fill );
            element.X = NormalizeCoordinate( element.X );
            element.Y = NormalizeCoordinate( element.Y );
            element.Width = NormalizeElementDimension( element.Width );
            element.Height = NormalizeElementDimension( element.Height );
            AddTextLength( element.Text, options, ref textLength );

            if ( element.Type == PdfElementType.Image )
            {
                if ( string.IsNullOrWhiteSpace( element.Source ) )
                    return false;

                ValidateDataUriSize( element.Source, $"{path}.Source", options.MaxResourceSize );
            }

            element.Font ??= new();
            element.Border ??= new();
            element.Appearance ??= new();
            element.Rows ??= [];

            NormalizeFont( element.Font );
            NormalizeBorder( element.Border );
            element.Appearance.BackgroundColor = NormalizeColor( element.Appearance.BackgroundColor, null );

            if ( element.Type != PdfElementType.Table )
            {
                element.Rows.Clear();
                return true;
            }

            for ( int rowIndex = element.Rows.Count - 1; rowIndex >= 0; rowIndex-- )
            {
                cancellationToken.ThrowIfCancellationRequested();
                PdfTableRowDefinition row = element.Rows[rowIndex];

                if ( row is null )
                {
                    element.Rows.RemoveAt( rowIndex );
                    continue;
                }

                CountNode( ref nodeCount, options );
                row.Height = NormalizePositiveDimension( row.Height, DefaultRowHeight );
                row.Cells ??= [];

                for ( int cellIndex = row.Cells.Count - 1; cellIndex >= 0; cellIndex-- )
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    PdfTableCellDefinition cell = row.Cells[cellIndex];

                    if ( cell is null )
                    {
                        row.Cells.RemoveAt( cellIndex );
                        continue;
                    }

                    CountNode( ref nodeCount, options );
                    cell.Width = NormalizePositiveDimension( cell.Width, DefaultCellWidth );
                    cell.Elements ??= [];

                    for ( int childIndex = cell.Elements.Count - 1; childIndex >= 0; childIndex-- )
                    {
                        if ( !NormalizeElement( cell.Elements[childIndex], $"{path}.Rows[{rowIndex}].Cells[{cellIndex}].Elements[{childIndex}]", options, ancestors, depth + 1, cancellationToken, ref nodeCount, ref textLength ) )
                            cell.Elements.RemoveAt( childIndex );
                    }
                }
            }

            return true;
        }
        finally
        {
            ancestors.Remove( element );
        }
    }

    private static void NormalizeFont( PdfFontDefinition font )
    {
        if ( string.IsNullOrWhiteSpace( font.Family ) )
            font.Family = "Helvetica";

        font.Size = NormalizePositiveDimension( font.Size, DefaultFontSize );
        font.Color = NormalizeColor( font.Color, "#000000" );
        font.Alignment = NormalizeEnum( font.Alignment, TextAlignment.Default );
        font.VerticalAlignment = NormalizeEnum( font.VerticalAlignment, VerticalAlignment.Default );
    }

    private static void NormalizeBorder( PdfBorderDefinition border )
    {
        border.Width = NormalizeElementDimension( border.Width );
        border.Color = NormalizeColor( border.Color, "#000000" );
        border.Style = NormalizeEnum( border.Style, PdfBorderStyle.Solid );
    }

    private static string NormalizeColor( string color, string fallback )
    {
        return string.IsNullOrWhiteSpace( color ) || !SimplePdfRenderProvider.IsValidColor( color )
            ? fallback
            : color;
    }

    private static void ValidateDataUriSize( string source, string path, long maxResourceSize )
    {
        if ( source?.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) != true )
            return;

        int commaIndex = source.IndexOf( ',' );

        if ( commaIndex < 0 )
            return;

        long encodedLength = 0;

        for ( int index = commaIndex + 1; index < source.Length; index++ )
        {
            if ( !char.IsWhiteSpace( source[index] ) )
                encodedLength++;
        }

        long maxEncodedLength = ( ( maxResourceSize + 2 ) / 3 ) * 4;

        if ( encodedLength > maxEncodedLength )
            throw new InvalidDataException( $"The PDF image source {path} exceeds the configured resource limit of {maxResourceSize} bytes." );
    }

    private static double NormalizePageDimension( double value, double fallback )
    {
        return double.IsFinite( value ) && value > 0 && value <= MaxPageDimension
            ? value
            : fallback;
    }

    private static double NormalizePositiveDimension( double value, double fallback )
    {
        return double.IsFinite( value ) && value > 0 && value <= MaxCoordinate
            ? value
            : fallback;
    }

    private static double NormalizeElementDimension( double value )
    {
        return double.IsFinite( value ) && value >= 0 && value <= MaxCoordinate
            ? value
            : 0;
    }

    private static double NormalizeCoordinate( double value )
    {
        return double.IsFinite( value ) && Math.Abs( value ) <= MaxCoordinate
            ? value
            : 0;
    }

    private static T NormalizeEnum<T>( T value, T fallback )
        where T : struct, Enum
    {
        return Enum.IsDefined( value ) ? value : fallback;
    }

    private static void CountNode( ref int nodeCount, PdfGenerationOptions options )
    {
        nodeCount++;

        if ( nodeCount > options.MaxDefinitionNodes )
            throw new InvalidDataException( $"The PDF definition contains more than the configured limit of {options.MaxDefinitionNodes} nodes." );
    }

    private static void AddTextLength( string text, PdfGenerationOptions options, ref long textLength )
    {
        if ( string.IsNullOrEmpty( text ) )
            return;

        textLength += text.Length;
        ValidateTextLength( textLength, options );
    }

    private static void ValidateTextLength( long textLength, PdfGenerationOptions options )
    {
        if ( textLength > options.MaxTextLength )
            throw new InvalidDataException( $"The PDF document contains more than the configured limit of {options.MaxTextLength} text characters." );
    }

    #endregion
}