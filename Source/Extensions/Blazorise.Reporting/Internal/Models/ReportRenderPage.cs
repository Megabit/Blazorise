#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportRenderPage
{
    #region Properties

    internal ReportDefinition Definition { get; set; }

    internal int PageNumber { get; set; }

    internal IReadOnlyList<ReportRenderSection> HeaderSections { get; set; } = [];

    internal IReadOnlyList<ReportRenderSection> BodySections { get; set; } = [];

    internal IReadOnlyList<ReportRenderSection> FooterSections { get; set; } = [];

    #endregion
}