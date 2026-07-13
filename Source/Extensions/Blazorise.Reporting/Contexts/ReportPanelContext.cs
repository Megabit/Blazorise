namespace Blazorise.Reporting;

/// <summary>
/// Provides the current declarative report panel to its child elements.
/// </summary>
internal sealed class ReportPanelContext : IReportElementContainerContext
{
    #region Methods

    public void AddElement( ReportElementDefinition element )
    {
        if ( element is not null )
            Definition?.Elements.Add( element );
    }

    #endregion

    #region Properties

    internal ReportPanelElementDefinition Definition { get; set; }

    #endregion
}