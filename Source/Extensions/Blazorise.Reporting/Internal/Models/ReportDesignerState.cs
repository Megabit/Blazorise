using Blazorise.Reporting;

namespace Blazorise.Reporting.Internal;

internal sealed class ReportDesignerState
{
    internal ReportState State { get; set; } = new();
}