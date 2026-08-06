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

    internal const int MaxElementDepth = 64;

    private const double DefaultFontSize = 12;

    private const double DefaultRowHeight = 24;

    private const double DefaultCellWidth = 90;

    #endregion

    #region Methods

    internal static IReadOnlyList<string> Validate( PdfDocumentDefinition document, PdfGenerationOptions options, CancellationToken cancellationToken )
    {
        List<string> diagnostics = [];

        cancellationToken.ThrowIfCancellationRequested();
        NormalizeOptions( options, diagnostics );

        document.Pages = NormalizeCollection( document.Pages, "Pages", diagnostics );
        document.Fonts = NormalizeCollection( document.Fonts, "Fonts", diagnostics );
        document.PageSize = NormalizeEnum( document.PageSize, PdfPageSize.A4, "PageSize", diagnostics );
        document.Orientation = NormalizeEnum( document.Orientation, PdfOrientation.Portrait, "Orientation", diagnostics );

        double customDocumentWidth = NormalizePageDimension( document.PageWidth, PdfPageMetrics.A4Width, "PageWidth", diagnostics );
        double customDocumentHeight = NormalizePageDimension( document.PageHeight, PdfPageMetrics.A4Height, "PageHeight", diagnostics );
        (double documentWidth, double documentHeight) = PdfPageMetrics.Resolve( document.PageSize, document.Orientation, customDocumentWidth, customDocumentHeight );
        document.PageWidth = NormalizePageDimension( documentWidth, PdfPageMetrics.A4Width, "PageWidth", diagnostics );
        document.PageHeight = NormalizePageDimension( documentHeight, PdfPageMetrics.A4Height, "PageHeight", diagnostics );

        for ( int pageIndex = document.Pages.Count - 1; pageIndex >= 0; pageIndex-- )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ( document.Pages[pageIndex] is null )
            {
                document.Pages.RemoveAt( pageIndex );
                diagnostics.Add( $"Pages[{pageIndex}] was null and was removed." );
            }
        }

        int pageCount = Math.Max( 1, document.Pages.Count );

        if ( document.Pages.Count == 0 )
            diagnostics.Add( "Pages was empty and a default page was generated." );

        if ( pageCount > options.MaxPages )
            throw new InvalidDataException( $"The PDF document contains {pageCount} pages, exceeding the configured limit of {options.MaxPages}." );

        int nodeCount = 0;
        long textLength = document.Title?.Length ?? 0;

        ValidateTextLength( textLength, options );
        NormalizeFonts( document.Fonts, options, diagnostics, cancellationToken, ref nodeCount );

        HashSet<PdfElementDefinition> ancestors = new( ReferenceEqualityComparer.Instance );

        for ( int pageIndex = document.Pages.Count - 1; pageIndex >= 0; pageIndex-- )
        {
            cancellationToken.ThrowIfCancellationRequested();
            PdfPageDefinition page = document.Pages[pageIndex];

            CountNode( ref nodeCount, options );
            string pagePath = $"Pages[{pageIndex}]";
            page.Size = NormalizeEnum( page.Size, PdfPageSize.Custom, $"{pagePath}.Size", diagnostics );
            page.Orientation = NormalizeEnum( page.Orientation, document.Orientation, $"{pagePath}.Orientation", diagnostics );

            double customPageWidth = NormalizePageDimension( page.Width, document.PageWidth, $"{pagePath}.Width", diagnostics );
            double customPageHeight = NormalizePageDimension( page.Height, document.PageHeight, $"{pagePath}.Height", diagnostics );
            (double pageWidth, double pageHeight) = PdfPageMetrics.Resolve( page.Size, page.Orientation, customPageWidth, customPageHeight );
            page.Width = NormalizePageDimension( pageWidth, document.PageWidth, $"{pagePath}.Width", diagnostics );
            page.Height = NormalizePageDimension( pageHeight, document.PageHeight, $"{pagePath}.Height", diagnostics );
            page.Elements = NormalizeCollection( page.Elements, $"{pagePath}.Elements", diagnostics );

            for ( int elementIndex = page.Elements.Count - 1; elementIndex >= 0; elementIndex-- )
            {
                if ( !NormalizeElement( page.Elements[elementIndex], $"{pagePath}.Elements[{elementIndex}]", options, diagnostics, ancestors, 0, cancellationToken, ref nodeCount, ref textLength ) )
                    page.Elements.RemoveAt( elementIndex );
            }
        }

        return diagnostics;
    }

    private static void NormalizeOptions( PdfGenerationOptions options, ICollection<string> diagnostics )
    {
        if ( options.MaxPages <= 0 )
        {
            options.MaxPages = PdfGenerationOptions.DefaultMaxPages;
            diagnostics.Add( $"Options.MaxPages was invalid and was normalized to {options.MaxPages}." );
        }

        if ( options.MaxDefinitionNodes <= 0 )
        {
            options.MaxDefinitionNodes = PdfGenerationOptions.DefaultMaxDefinitionNodes;
            diagnostics.Add( $"Options.MaxDefinitionNodes was invalid and was normalized to {options.MaxDefinitionNodes}." );
        }

        if ( options.MaxTextLength <= 0 )
        {
            options.MaxTextLength = PdfGenerationOptions.DefaultMaxTextLength;
            diagnostics.Add( $"Options.MaxTextLength was invalid and was normalized to {options.MaxTextLength}." );
        }

        if ( options.MaxResourceSize <= 0 || options.MaxResourceSize > int.MaxValue )
        {
            options.MaxResourceSize = PdfGenerationOptions.DefaultMaxResourceSize;
            diagnostics.Add( $"Options.MaxResourceSize was invalid and was normalized to {options.MaxResourceSize}." );
        }

        if ( options.MaxTotalResourceSize <= 0 )
        {
            options.MaxTotalResourceSize = PdfGenerationOptions.DefaultMaxTotalResourceSize;
            diagnostics.Add( $"Options.MaxTotalResourceSize was invalid and was normalized to {options.MaxTotalResourceSize}." );
        }

        if ( options.MaxImagePixels <= 0 )
        {
            options.MaxImagePixels = PdfGenerationOptions.DefaultMaxImagePixels;
            diagnostics.Add( $"Options.MaxImagePixels was invalid and was normalized to {options.MaxImagePixels}." );
        }
    }

    private static void NormalizeFonts( List<FontFamily> fonts, PdfGenerationOptions options, ICollection<string> diagnostics, CancellationToken cancellationToken, ref int nodeCount )
    {
        for ( int fontIndex = fonts.Count - 1; fontIndex >= 0; fontIndex-- )
        {
            cancellationToken.ThrowIfCancellationRequested();
            FontFamily font = fonts[fontIndex];
            string fontPath = $"Fonts[{fontIndex}]";

            if ( font is null || string.IsNullOrWhiteSpace( font.Name ) || !IsValidFontSource( font.Regular, options, cancellationToken, ref nodeCount ) )
            {
                fonts.RemoveAt( fontIndex );
                diagnostics.Add( $"{fontPath} was invalid and was removed." );
                continue;
            }

            CountNode( ref nodeCount, options );

            if ( font.Bold is not null && !IsValidFontSource( font.Bold, options, cancellationToken, ref nodeCount ) )
            {
                font.Bold = null;
                diagnostics.Add( $"{fontPath}.Bold was invalid and was removed." );
            }

            if ( font.Italic is not null && !IsValidFontSource( font.Italic, options, cancellationToken, ref nodeCount ) )
            {
                font.Italic = null;
                diagnostics.Add( $"{fontPath}.Italic was invalid and was removed." );
            }

            if ( font.BoldItalic is not null && !IsValidFontSource( font.BoldItalic, options, cancellationToken, ref nodeCount ) )
            {
                font.BoldItalic = null;
                diagnostics.Add( $"{fontPath}.BoldItalic was invalid and was removed." );
            }
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

    private static bool NormalizeElement( PdfElementDefinition element, string path, PdfGenerationOptions options, ICollection<string> diagnostics, HashSet<PdfElementDefinition> ancestors, int depth, CancellationToken cancellationToken, ref int nodeCount, ref long textLength )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ( element is null )
        {
            diagnostics.Add( $"{path} was null and was removed." );
            return false;
        }

        if ( depth >= MaxElementDepth )
        {
            diagnostics.Add( $"{path} exceeded the maximum nesting depth and was removed." );
            return false;
        }

        if ( !ancestors.Add( element ) )
        {
            diagnostics.Add( $"{path} created a circular reference and was removed." );
            return false;
        }

        try
        {
            CountNode( ref nodeCount, options );
            element.Type = NormalizeEnum( element.Type, PdfElementType.Text, $"{path}.Type", diagnostics );
            element.Orientation = NormalizeEnum( element.Orientation, Orientation.Horizontal, $"{path}.Orientation", diagnostics );
            element.ImageFit = NormalizeEnum( element.ImageFit, PdfImageFit.Fill, $"{path}.ImageFit", diagnostics );
            element.X = NormalizeCoordinate( element.X, $"{path}.X", diagnostics );
            element.Y = NormalizeCoordinate( element.Y, $"{path}.Y", diagnostics );
            element.Width = NormalizeElementDimension( element.Width, $"{path}.Width", diagnostics );
            element.Height = NormalizeElementDimension( element.Height, $"{path}.Height", diagnostics );
            AddTextLength( element.Text, options, ref textLength );

            if ( element.Type == PdfElementType.Image )
            {
                if ( string.IsNullOrWhiteSpace( element.Source ) )
                {
                    diagnostics.Add( $"{path} did not define an image source and was removed." );
                    return false;
                }

                ValidateDataUriSize( element.Source, $"{path}.Source", options.MaxResourceSize );
            }

            if ( element.Font is null )
            {
                element.Font = new();
                diagnostics.Add( $"{path}.Font was null and was replaced with default settings." );
            }

            if ( element.Border is null )
            {
                element.Border = new();
                diagnostics.Add( $"{path}.Border was null and was replaced with default settings." );
            }

            if ( element.Appearance is null )
            {
                element.Appearance = new();
                diagnostics.Add( $"{path}.Appearance was null and was replaced with default settings." );
            }

            element.Rows = NormalizeCollection( element.Rows, $"{path}.Rows", diagnostics );

            NormalizeFont( element.Font, $"{path}.Font", diagnostics );
            NormalizeBorder( element.Border, $"{path}.Border", diagnostics );
            element.Appearance.BackgroundColor = NormalizeColor( element.Appearance.BackgroundColor, null, $"{path}.Appearance.BackgroundColor", diagnostics );

            if ( element.Type != PdfElementType.Table )
            {
                if ( element.Rows.Count > 0 )
                    diagnostics.Add( $"{path}.Rows was ignored because the element is not a table." );

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
                    diagnostics.Add( $"{path}.Rows[{rowIndex}] was null and was removed." );
                    continue;
                }

                CountNode( ref nodeCount, options );
                string rowPath = $"{path}.Rows[{rowIndex}]";
                row.Height = NormalizePositiveDimension( row.Height, DefaultRowHeight, $"{rowPath}.Height", diagnostics );
                row.Cells = NormalizeCollection( row.Cells, $"{rowPath}.Cells", diagnostics );

                for ( int cellIndex = row.Cells.Count - 1; cellIndex >= 0; cellIndex-- )
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    PdfTableCellDefinition cell = row.Cells[cellIndex];

                    if ( cell is null )
                    {
                        row.Cells.RemoveAt( cellIndex );
                        diagnostics.Add( $"{rowPath}.Cells[{cellIndex}] was null and was removed." );
                        continue;
                    }

                    CountNode( ref nodeCount, options );
                    string cellPath = $"{rowPath}.Cells[{cellIndex}]";
                    cell.Width = NormalizePositiveDimension( cell.Width, DefaultCellWidth, $"{cellPath}.Width", diagnostics );
                    cell.Elements = NormalizeCollection( cell.Elements, $"{cellPath}.Elements", diagnostics );

                    for ( int childIndex = cell.Elements.Count - 1; childIndex >= 0; childIndex-- )
                    {
                        if ( !NormalizeElement( cell.Elements[childIndex], $"{cellPath}.Elements[{childIndex}]", options, diagnostics, ancestors, depth + 1, cancellationToken, ref nodeCount, ref textLength ) )
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

    private static void NormalizeFont( PdfFontDefinition font, string path, ICollection<string> diagnostics )
    {
        if ( string.IsNullOrWhiteSpace( font.Family ) )
        {
            font.Family = "Helvetica";
            diagnostics.Add( $"{path}.Family was empty and was normalized to Helvetica." );
        }

        font.Size = NormalizePositiveDimension( font.Size, DefaultFontSize, $"{path}.Size", diagnostics );
        font.Color = NormalizeColor( font.Color, "#000000", $"{path}.Color", diagnostics );
        font.Alignment = NormalizeEnum( font.Alignment, TextAlignment.Default, $"{path}.Alignment", diagnostics );
        font.VerticalAlignment = NormalizeEnum( font.VerticalAlignment, VerticalAlignment.Default, $"{path}.VerticalAlignment", diagnostics );
    }

    private static void NormalizeBorder( PdfBorderDefinition border, string path, ICollection<string> diagnostics )
    {
        border.Width = NormalizeElementDimension( border.Width, $"{path}.Width", diagnostics );
        border.Color = NormalizeColor( border.Color, "#000000", $"{path}.Color", diagnostics );
        border.Style = NormalizeEnum( border.Style, PdfBorderStyle.Solid, $"{path}.Style", diagnostics );
    }

    private static string NormalizeColor( string color, string fallback, string path, ICollection<string> diagnostics )
    {
        if ( !string.IsNullOrWhiteSpace( color ) && SimplePdfRenderProvider.IsValidColor( color ) )
            return color;

        if ( color != fallback )
            diagnostics.Add( $"{path} was invalid and was normalized to {fallback ?? "no color"}." );

        return fallback;
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

    private static List<T> NormalizeCollection<T>( List<T> collection, string path, ICollection<string> diagnostics )
    {
        if ( collection is not null )
            return collection;

        diagnostics.Add( $"{path} was null and was replaced with an empty collection." );

        return [];
    }

    private static double NormalizePageDimension( double value, double fallback, string path, ICollection<string> diagnostics )
    {
        if ( double.IsFinite( value ) && value > 0 && value <= MaxPageDimension )
            return value;

        diagnostics.Add( $"{path} was invalid and was normalized to {fallback}." );

        return fallback;
    }

    private static double NormalizePositiveDimension( double value, double fallback, string path, ICollection<string> diagnostics )
    {
        if ( double.IsFinite( value ) && value > 0 && value <= MaxCoordinate )
            return value;

        diagnostics.Add( $"{path} was invalid and was normalized to {fallback}." );

        return fallback;
    }

    private static double NormalizeElementDimension( double value, string path, ICollection<string> diagnostics )
    {
        if ( double.IsFinite( value ) && value >= 0 && value <= MaxCoordinate )
            return value;

        diagnostics.Add( $"{path} was invalid and was normalized to 0." );

        return 0;
    }

    private static double NormalizeCoordinate( double value, string path, ICollection<string> diagnostics )
    {
        if ( double.IsFinite( value ) && Math.Abs( value ) <= MaxCoordinate )
            return value;

        diagnostics.Add( $"{path} was invalid and was normalized to 0." );

        return 0;
    }

    private static T NormalizeEnum<T>( T value, T fallback, string path, ICollection<string> diagnostics )
        where T : struct, Enum
    {
        if ( Enum.IsDefined( value ) )
            return value;

        diagnostics.Add( $"{path} was invalid and was normalized to {fallback}." );

        return fallback;
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