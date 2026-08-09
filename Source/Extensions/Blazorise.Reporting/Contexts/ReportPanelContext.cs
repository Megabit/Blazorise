namespace Blazorise.Reporting;

/// <summary>
/// Provides the current declarative report panel to its child elements.
/// </summary>
internal sealed class ReportPanelContext : IReportElementContainerContext
{
    private readonly ReportRegistrationCollection<ReportElementDefinition> elements = new();

    #region Methods

    public void RegisterElement( object owner, ReportElementDefinition element )
    {
        if ( element is null )
            return;

        elements.Set( owner, element );

        if ( Definition is not null )
            Definition.Elements = [.. elements.Values];

        NotifyDefinitionChanged();
    }

    public void UnregisterElement( object owner )
    {
        if ( !elements.Remove( owner ) )
            return;

        if ( Definition is not null )
            Definition.Elements = [.. elements.Values];

        NotifyDefinitionChanged();
    }

    public void NotifyDefinitionChanged()
    {
        DefinitionChanged?.Invoke();
    }

    #endregion

    #region Properties

    internal ReportPanelElementDefinition Definition { get; set; }

    internal System.Action DefinitionChanged { get; set; }

    #endregion
}