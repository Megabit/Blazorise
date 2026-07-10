namespace Blazorise.Reporting;

internal sealed class ReportSectionContext
{
    public ReportSectionContext( ReportBandDefinition definition )
    {
        Definition = definition;
    }

    public ReportBandDefinition Definition { get; }
}