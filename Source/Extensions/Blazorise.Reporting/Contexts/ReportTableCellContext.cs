namespace Blazorise.Reporting;

internal sealed class ReportTableCellContext : IReportElementContainerContext
{
    private readonly ReportRegistrationCollection<ReportElementDefinition> elements = new();

    internal void Attach( ReportTableRowContext rowContext, ReportTableCellDefinition definition )
    {
        RowContext = rowContext;
        Definition = definition;
        Definition.Elements = [.. elements.Values];
    }

    public void RegisterElement( object owner, ReportElementDefinition element )
    {
        if ( element is null )
            return;

        Internal.ReportDefinitionHelper.FitElementToTableCell( TableDefinition, Definition, element );
        elements.Set( owner, element );
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
        RowContext?.NotifyDefinitionChanged();
    }

    internal ReportTableElementDefinition TableDefinition => RowContext?.TableDefinition;

    internal ReportTableCellDefinition Definition { get; private set; }

    internal ReportTableRowContext RowContext { get; private set; }
}