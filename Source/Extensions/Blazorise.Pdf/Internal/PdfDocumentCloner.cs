#region Using directives
using System.Collections.Generic;
using System.Threading;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

internal static class PdfDocumentCloner
{
    #region Methods

    internal static PdfDocumentDefinition Clone( PdfDocumentDefinition document, CancellationToken cancellationToken )
    {
        Dictionary<PdfElementDefinition, PdfElementDefinition> elements = new( ReferenceEqualityComparer.Instance );

        return new()
        {
            Title = document.Title,
            PageSize = document.PageSize,
            Orientation = document.Orientation,
            PageWidth = document.PageWidth,
            PageHeight = document.PageHeight,
            Pages = ClonePages( document.Pages, elements, cancellationToken ),
            Fonts = CloneFonts( document.Fonts, cancellationToken ),
        };
    }

    private static List<PdfPageDefinition> ClonePages( List<PdfPageDefinition> pages, Dictionary<PdfElementDefinition, PdfElementDefinition> elements, CancellationToken cancellationToken )
    {
        if ( pages is null )
            return null;

        List<PdfPageDefinition> clones = new( pages.Count );

        foreach ( PdfPageDefinition page in pages )
        {
            cancellationToken.ThrowIfCancellationRequested();

            clones.Add( page is null
                ? null
                : new()
                {
                    Size = page.Size,
                    Orientation = page.Orientation,
                    Width = page.Width,
                    Height = page.Height,
                    Elements = CloneElements( page.Elements, elements, 0, cancellationToken ),
                } );
        }

        return clones;
    }

    private static List<PdfElementDefinition> CloneElements( List<PdfElementDefinition> elements, Dictionary<PdfElementDefinition, PdfElementDefinition> clonedElements, int depth, CancellationToken cancellationToken )
    {
        if ( elements is null )
            return null;

        List<PdfElementDefinition> clones = new( elements.Count );

        foreach ( PdfElementDefinition element in elements )
            clones.Add( CloneElement( element, clonedElements, depth, cancellationToken ) );

        return clones;
    }

    private static PdfElementDefinition CloneElement( PdfElementDefinition element, Dictionary<PdfElementDefinition, PdfElementDefinition> elements, int depth, CancellationToken cancellationToken )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ( element is null )
            return null;

        if ( elements.TryGetValue( element, out PdfElementDefinition existingClone ) )
            return existingClone;

        if ( depth >= PdfDocumentValidator.MaxElementDepth )
            return element;

        PdfElementDefinition clone = new()
        {
            Type = element.Type,
            X = element.X,
            Y = element.Y,
            Width = element.Width,
            Height = element.Height,
            Orientation = element.Orientation,
            Text = element.Text,
            Wrap = element.Wrap,
            ClipContent = element.ClipContent,
            Source = element.Source,
            ImageFit = element.ImageFit,
            Font = CloneFont( element.Font ),
            Border = CloneBorder( element.Border ),
            Appearance = CloneAppearance( element.Appearance ),
        };

        elements.Add( element, clone );
        clone.Rows = CloneRows( element.Rows, elements, depth, cancellationToken );

        return clone;
    }

    private static List<PdfTableRowDefinition> CloneRows( List<PdfTableRowDefinition> rows, Dictionary<PdfElementDefinition, PdfElementDefinition> elements, int depth, CancellationToken cancellationToken )
    {
        if ( rows is null )
            return null;

        List<PdfTableRowDefinition> clones = new( rows.Count );

        foreach ( PdfTableRowDefinition row in rows )
        {
            cancellationToken.ThrowIfCancellationRequested();

            clones.Add( row is null
                ? null
                : new()
                {
                    Height = row.Height,
                    Cells = CloneCells( row.Cells, elements, depth, cancellationToken ),
                } );
        }

        return clones;
    }

    private static List<PdfTableCellDefinition> CloneCells( List<PdfTableCellDefinition> cells, Dictionary<PdfElementDefinition, PdfElementDefinition> elements, int depth, CancellationToken cancellationToken )
    {
        if ( cells is null )
            return null;

        List<PdfTableCellDefinition> clones = new( cells.Count );

        foreach ( PdfTableCellDefinition cell in cells )
        {
            cancellationToken.ThrowIfCancellationRequested();

            clones.Add( cell is null
                ? null
                : new()
                {
                    Width = cell.Width,
                    Elements = CloneElements( cell.Elements, elements, depth + 1, cancellationToken ),
                } );
        }

        return clones;
    }

    private static PdfFontDefinition CloneFont( PdfFontDefinition font )
        => font is null
            ? null
            : new()
            {
                Family = font.Family,
                Size = font.Size,
                Color = font.Color,
                Alignment = font.Alignment,
                VerticalAlignment = font.VerticalAlignment,
                Bold = font.Bold,
                Italic = font.Italic,
            };

    private static PdfBorderDefinition CloneBorder( PdfBorderDefinition border )
        => border is null
            ? null
            : new()
            {
                Color = border.Color,
                Width = border.Width,
                Style = border.Style,
            };

    private static PdfAppearanceDefinition CloneAppearance( PdfAppearanceDefinition appearance )
        => appearance is null
            ? null
            : new()
            {
                BackgroundColor = appearance.BackgroundColor,
            };

    private static List<FontFamily> CloneFonts( List<FontFamily> fonts, CancellationToken cancellationToken )
    {
        if ( fonts is null )
            return null;

        List<FontFamily> clones = new( fonts.Count );

        foreach ( FontFamily font in fonts )
        {
            cancellationToken.ThrowIfCancellationRequested();

            clones.Add( font is null
                ? null
                : new()
                {
                    Name = font.Name,
                    DisplayName = font.DisplayName,
                    CssFamily = font.CssFamily,
                    Regular = CloneFontSource( font.Regular ),
                    Bold = CloneFontSource( font.Bold ),
                    Italic = CloneFontSource( font.Italic ),
                    BoldItalic = CloneFontSource( font.BoldItalic ),
                    Visible = font.Visible,
                } );
        }

        return clones;
    }

    private static FontSource CloneFontSource( FontSource source )
        => source is null
            ? null
            : new()
            {
                Url = source.Url,
                Data = source.Data,
                FileName = source.FileName,
                Format = source.Format,
            };

    #endregion
}