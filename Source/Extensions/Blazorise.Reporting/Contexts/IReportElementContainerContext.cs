namespace Blazorise.Reporting;

/// <summary>
/// Registers declarative elements with their nearest report container.
/// </summary>
internal interface IReportElementContainerContext
{
    void RegisterElement( object owner, ReportElementDefinition element );

    void UnregisterElement( object owner );

    void NotifyDefinitionChanged();
}