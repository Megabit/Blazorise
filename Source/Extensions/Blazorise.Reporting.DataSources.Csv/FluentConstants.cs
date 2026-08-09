namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Reusable fluent constants used to avoid repeated fluent object allocations in CSV data source components.
/// </summary>
internal static class FluentConstants
{
    internal static readonly IFluentColumn ColumnSizeIs3 = ColumnSize.Is3;
    internal static readonly IFluentColumn ColumnSizeIs9 = ColumnSize.Is9;
}