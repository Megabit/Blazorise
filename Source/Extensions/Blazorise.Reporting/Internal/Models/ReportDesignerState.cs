#region Using directives
using Blazorise.Reporting;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerState
{
    internal ReportState State { get; set; } = new();

    internal ReportDesignerRefreshTarget RefreshTargets { get; set; }
}