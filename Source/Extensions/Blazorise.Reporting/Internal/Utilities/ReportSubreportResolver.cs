namespace Blazorise.Reporting.Internal;

internal static class ReportSubreportResolver
{
    internal static ReportDefinition ResolveDefinition( ReportSubreportElementDefinition element )
    {
        ReportDefinition definition = element?.Report;

        ReportDefinitionHelper.RemoveSubreportElements( definition );

        return definition;
    }

    internal static object ResolveData( ReportDefinition parentDefinition, object parentData, object parentItem, ReportSubreportElementDefinition element )
    {
        if ( element is null )
            return null;

        return ReportDataResolver.ResolveDataSourceValue( parentDefinition, parentData, element.DataSource, parentItem );
    }

    internal static string GetDisplayName( ReportSubreportElementDefinition element )
    {
        if ( element is null )
            return "Subreport";

        if ( !string.IsNullOrWhiteSpace( element.Name ) )
            return element.Name;

        if ( !string.IsNullOrWhiteSpace( element.Report?.Name ) )
            return element.Report.Name;

        return "Subreport";
    }
}