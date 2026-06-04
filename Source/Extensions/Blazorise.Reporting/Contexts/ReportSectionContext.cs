namespace Blazorise.Reporting;

internal sealed class ReportSectionContext
{
    public ReportSectionContext( ReportSectionDefinition definition )
    {
        Definition = definition;
    }

    public ReportSectionDefinition Definition { get; }
}