namespace Blazorise.Reporting;

internal sealed class ReportTableCellContext( ReportTableElementDefinition tableDefinition, ReportTableCellDefinition definition ) : IReportElementContainerContext
{
    #region Methods

    public void AddElement( ReportElementDefinition element )
    {
        if ( element is null )
            return;

        Internal.ReportDefinitionHelper.FitElementToTableCell( TableDefinition, Definition, element );
        Definition.Elements.Add( element );
    }

    #endregion

    #region Properties

    internal ReportTableElementDefinition TableDefinition { get; } = tableDefinition;

    internal ReportTableCellDefinition Definition { get; } = definition;

    #endregion
}