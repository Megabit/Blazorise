#region Using directives
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Generates PDF documents from PDF document definitions.
/// </summary>
public interface IPdfGenerator
{
    #region Methods

    /// <summary>
    /// Generates a PDF document in memory.
    /// </summary>
    /// <param name="document">The PDF document definition.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The generated PDF result.</returns>
    Task<PdfGenerationResult> GenerateAsync( PdfDocumentDefinition document, PdfGenerationOptions options = null, CancellationToken cancellationToken = default );

    /// <summary>
    /// Generates a PDF document and writes it to a stream.
    /// </summary>
    /// <param name="document">The PDF document definition.</param>
    /// <param name="stream">The destination stream.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <remarks>The destination stream remains open after generation.</remarks>
    Task GenerateToStreamAsync( PdfDocumentDefinition document, Stream stream, PdfGenerationOptions options = null, CancellationToken cancellationToken = default );

    #endregion
}