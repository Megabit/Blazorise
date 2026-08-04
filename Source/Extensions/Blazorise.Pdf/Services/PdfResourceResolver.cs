#region Using directives
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Resolves the built-in PDF image and font resource sources.
/// </summary>
public class PdfResourceResolver : IPdfResourceResolver
{
    #region Methods

    /// <inheritdoc />
    public virtual Task<PdfResourceContent> ResolveImageAsync( string source, CancellationToken cancellationToken = default )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ( !TryReadDataUri( source, out string mediaType, out byte[] data ) )
            throw new NotSupportedException( "The default PDF resource resolver supports images only as base64 data URIs. Call AddBlazorisePdfHttpResources during service registration to use image URLs." );

        return Task.FromResult( new PdfResourceContent( data, mediaType ) );
    }

    /// <inheritdoc />
    public virtual async Task<PdfResourceContent> ResolveFontAsync( FontSource source, CancellationToken cancellationToken = default )
    {
        ValidateFontSource( source );

        if ( source.Data is { Length: > 0 } )
            return new( source.Data, ResolveFontMediaType( source.Format ) );

        if ( !string.IsNullOrWhiteSpace( source.FileName ) )
        {
            if ( !File.Exists( source.FileName ) )
                throw new FileNotFoundException( "The PDF font file could not be found.", source.FileName );

            byte[] data = await File.ReadAllBytesAsync( source.FileName, cancellationToken );

            return new( data, ResolveFontMediaType( source.Format ) );
        }

        if ( !string.IsNullOrWhiteSpace( source.Url ) )
            throw new NotSupportedException( "The default PDF resource resolver does not load font URLs. Call AddBlazorisePdfHttpResources during service registration, or populate FontSource.Data." );

        throw new InvalidDataException( "The PDF font source does not contain data, a file name, or a URL." );
    }

    /// <summary>
    /// Validates a font source before it is resolved.
    /// </summary>
    /// <param name="source">Font source.</param>
    protected static void ValidateFontSource( FontSource source )
    {
        if ( source is null )
            throw new InvalidDataException( "The PDF font source is missing." );

        if ( source.Format is not FontFormat.TrueType and not FontFormat.OpenType )
            throw new NotSupportedException( $"The PDF renderer does not support the {source.Format} font format. Use a TrueType or OpenType source." );
    }

    private static bool TryReadDataUri( string source, out string mediaType, out byte[] data )
    {
        mediaType = null;
        data = null;

        if ( string.IsNullOrWhiteSpace( source ) || !source.StartsWith( "data:", StringComparison.OrdinalIgnoreCase ) )
            return false;

        int commaIndex = source.IndexOf( ',' );

        if ( commaIndex < 0 )
            throw new InvalidDataException( "The image data URI is missing its data separator." );

        string metadata = source.Substring( 5, commaIndex - 5 );

        if ( !metadata.Contains( ";base64", StringComparison.OrdinalIgnoreCase ) )
            throw new NotSupportedException( "The PDF renderer supports only base64 image data URIs." );

        mediaType = metadata.Split( ';', StringSplitOptions.RemoveEmptyEntries ).FirstOrDefault()?.Trim().ToLowerInvariant();

        if ( string.IsNullOrWhiteSpace( mediaType ) )
            throw new InvalidDataException( "The image data URI does not define a media type." );

        try
        {
            data = Convert.FromBase64String( source[( commaIndex + 1 )..] );
        }
        catch ( FormatException exception )
        {
            throw new InvalidDataException( "The image data URI contains invalid base64 data.", exception );
        }

        if ( data.Length == 0 )
            throw new InvalidDataException( "The image data URI does not contain image data." );

        return true;
    }

    private static string ResolveFontMediaType( FontFormat format )
    {
        return format == FontFormat.OpenType ? "font/otf" : "font/ttf";
    }

    #endregion
}