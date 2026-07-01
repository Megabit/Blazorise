namespace Blazorise.Reporting.Internal;

internal static class ReportElementDefinitionFactory
{
    internal static ReportElementDefinition Create( ReportElementType elementType )
    {
        return elementType switch
        {
            ReportElementType.Text => new ReportTextElementDefinition(),
            ReportElementType.Field => new ReportFieldElementDefinition(),
            ReportElementType.Image => new ReportImageElementDefinition(),
            ReportElementType.Line => new ReportLineElementDefinition(),
            ReportElementType.Rectangle => new ReportRectangleElementDefinition(),
            ReportElementType.Table => new ReportTableElementDefinition(),
            ReportElementType.PageBreak => new ReportPageBreakElementDefinition(),
            _ => throw new System.ArgumentOutOfRangeException( nameof( elementType ), elementType, null ),
        };
    }
}