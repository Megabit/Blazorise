namespace Blazorise.Reporting;

internal sealed class ReportSectionContext : IReportElementContainerContext
{
    private readonly ReportRegistrationCollection<ReportElementDefinition> elements = new();

    #region Constructors

    public ReportSectionContext( ReportBandDefinition definition )
    {
        Definition = definition;
    }

    #endregion

    #region Methods

    public void RegisterElement( object owner, ReportElementDefinition element )
    {
        if ( element is null )
            return;

        elements.Set( owner, element );
        Definition.Elements = [.. elements.Values];
        NotifyDefinitionChanged();
    }

    public void UnregisterElement( object owner )
    {
        if ( !elements.Remove( owner ) )
            return;

        Definition.Elements = [.. elements.Values];
        NotifyDefinitionChanged();
    }

    public void NotifyDefinitionChanged()
    {
        DefinitionChanged?.Invoke();
    }

    #endregion

    #region Properties

    public ReportBandDefinition Definition { get; }

    internal System.Action DefinitionChanged { get; set; }

    #endregion
}