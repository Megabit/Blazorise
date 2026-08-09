namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Reusable fluent constants used to avoid repeated fluent object allocations in Web API data source components.
/// </summary>
internal static class FluentConstants
{
    internal static readonly IFluentColumn ColumnSizeIs3 = ColumnSize.Is3;
    internal static readonly IFluentColumn ColumnSizeIs9 = ColumnSize.Is9;
}