namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Reusable fluent constants used to avoid repeated fluent object allocations in SQL data source components.
/// </summary>
internal static class FluentConstants
{
    internal static readonly IFluentColumn ColumnSizeIs3 = ColumnSize.Is3;
    internal static readonly IFluentColumn ColumnSizeIs9 = ColumnSize.Is9;
}