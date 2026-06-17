#region Using directives
using System;
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
    public Task<PdfRenderResult> GenerateAsync( PdfDocumentDefinition document, PdfGenerateOptions options = null, CancellationToken cancellationToken = default )
    {
        if ( document is null )
            throw new ArgumentNullException( nameof( document ) );

        options ??= new();

        return renderProvider.RenderAsync( document, options, cancellationToken );
    }

    #endregion
}