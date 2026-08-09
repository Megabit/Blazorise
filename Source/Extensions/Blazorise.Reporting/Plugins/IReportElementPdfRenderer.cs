#region Using directives
using System.Collections.Generic;
using Blazorise.Pdf;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Renders a custom report element into PDF element definitions.
/// </summary>
public interface IReportElementPdfRenderer
{
    /// <summary>
    /// Creates PDF elements positioned relative to the custom element.
    /// </summary>
    IEnumerable<PdfElementDefinition> Render( ReportElementPdfRenderContext context );
}
