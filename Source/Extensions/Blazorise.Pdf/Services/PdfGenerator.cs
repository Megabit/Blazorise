#region Using directives
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Default PDF generator implementation.
/// </summary>
public sealed class PdfGenerator : IPdfGenerator
{
    #region Members

    private readonly IPdfRenderProvider renderProvider;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the PDF generator.
    /// </summary>
    /// <param name="renderProvider">The PDF render provider.</param>
    public PdfGenerator( IPdfRenderProvider renderProvider )
    {
        this.renderProvider = renderProvider;
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public Task<PdfGenerationResult> GenerateAsync( PdfDocumentDefinition document, PdfGenerationOptions options = null, CancellationToken cancellationToken = default )
    {
        if ( document is null )
            throw new ArgumentNullException( nameof( document ) );

        options ??= new();

        return renderProvider.RenderAsync( document, options, cancellationToken );
    }

    /// <inheritdoc />
    public Task GenerateToStreamAsync( PdfDocumentDefinition document, Stream stream, PdfGenerationOptions options = null, CancellationToken cancellationToken = default )
    {
        if ( document is null )
            throw new ArgumentNullException( nameof( document ) );

        if ( stream is null )
            throw new ArgumentNullException( nameof( stream ) );

        if ( !stream.CanWrite )
            throw new ArgumentException( "The PDF destination stream must be writable.", nameof( stream ) );

        options ??= new();

        return renderProvider.RenderToStreamAsync( document, stream, options, cancellationToken );
    }

    #endregion
}