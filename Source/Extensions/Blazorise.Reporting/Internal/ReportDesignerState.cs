using Blazorise.Reporting;

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerState
{
    public ReportState State { get; set; } = new();
}