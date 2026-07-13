namespace Blazorise.Reporting;

/// <summary>
/// Registers declarative elements with their nearest report container.
/// </summary>
internal interface IReportElementContainerContext
{
    void AddElement( ReportElementDefinition element );
}