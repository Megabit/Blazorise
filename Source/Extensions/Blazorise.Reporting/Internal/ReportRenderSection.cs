#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportRenderSection
{
    #region Properties

    internal int SectionIndex { get; set; }

    internal int InstanceIndex { get; set; }

    internal ReportSectionDefinition Section { get; set; }

    internal object Item { get; set; }

    internal IReadOnlyDictionary<string, object> RunningTotals { get; set; }

    internal bool RenderElements { get; set; } = true;

    #endregion
}