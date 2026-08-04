#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Resolves image and font resources used during PDF generation.
/// </summary>
public interface IPdfResourceResolver
{
    #region Methods

    /// <summary>
    /// Resolves an image source.
    /// </summary>
    /// <param name="source">Image source.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resolved image content.</returns>
    Task<PdfResourceContent> ResolveImageAsync( string source, CancellationToken cancellationToken = default );

    /// <summary>
    /// Resolves a font source.
    /// </summary>
    /// <param name="source">Font source.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Resolved font content.</returns>
    Task<PdfResourceContent> ResolveFontAsync( FontSource source, CancellationToken cancellationToken = default );

    #endregion
}