namespace Blazorise.Reporting;

internal sealed class ReportSectionContext : IReportElementContainerContext
{
    #region Constructors

    public ReportSectionContext( ReportBandDefinition definition )
    {
        Definition = definition;
    }

    #endregion

    #region Methods

    public void AddElement( ReportElementDefinition element )
    {
        if ( element is not null )
            Definition.Elements.Add( element );
    }

    #endregion

    #region Properties

    public ReportBandDefinition Definition { get; }

    #endregion
}