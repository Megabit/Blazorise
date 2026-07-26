namespace Blazorise.Reporting.Internal;

internal sealed class ReportTableCellDropTarget
{
    internal ReportTableElementDefinition Table { get; set; }

    internal ReportTableCellDefinition Cell { get; set; }

    internal double X { get; set; }

    internal double Y { get; set; }
}