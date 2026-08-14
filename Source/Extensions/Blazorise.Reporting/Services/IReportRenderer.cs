#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Pdf;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Renders persisted report definitions without requiring a mounted Report component.
/// </summary>
public interface IReportRenderer
{
    #region Methods

    /// <summary>
    /// Creates a PDF document definition from a report definition and its runtime data.
    /// </summary>
    /// <param name="definition">The persisted report definition.</param>
    /// <param name="options">Runtime data and rendering options.</param>
    /// <param name="cancellationToken">A token that cancels the render operation.</param>
    /// <returns>The PDF document definition.</returns>
    Task<PdfDocumentDefinition> RenderAsync( ReportDefinition definition, ReportRenderOptions options = null, CancellationToken cancellationToken = default );

    #endregion
}