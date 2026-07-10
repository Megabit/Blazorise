#region Using directives
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
    /// Renders a PDF document.
    /// </summary>
    /// <param name="document">The PDF document definition.</param>
    /// <param name="options">The generation options.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The generated PDF result.</returns>
    Task<PdfGenerationResult> Render( PdfDocumentDefinition document, PdfGenerationOptions options, CancellationToken cancellationToken = default );

    #endregion
}