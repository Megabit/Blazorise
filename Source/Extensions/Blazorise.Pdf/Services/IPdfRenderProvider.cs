#region Using directives
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Renders PDF document definitions into PDF bytes.
/// </summary>
public interface IPdfRenderProvider
{
    #region Methods

    /// <summary>
    /// Renders a PDF document in memory.
    /// </summary>
    /// <param name="document">The PDF document definition.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The generated PDF result.</returns>
    Task<PdfGenerationResult> RenderAsync( PdfDocumentDefinition document, PdfGenerationOptions options, CancellationToken cancellationToken = default );

    /// <summary>
    /// Renders a PDF document to a stream.
    /// </summary>
    /// <param name="document">The PDF document definition.</param>
    /// <param name="stream">The destination stream.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <remarks>The destination stream remains open after rendering.</remarks>
    Task RenderToStreamAsync( PdfDocumentDefinition document, Stream stream, PdfGenerationOptions options, CancellationToken cancellationToken = default );

    #endregion
}