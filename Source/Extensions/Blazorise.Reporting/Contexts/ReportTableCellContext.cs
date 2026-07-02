namespace Blazorise.Reporting;

internal sealed class ReportTableCellContext( ReportTableElementDefinition tableDefinition, ReportTableCellDefinition definition )
{
    #region Methods

    internal void AddElement( ReportElementDefinition element )
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