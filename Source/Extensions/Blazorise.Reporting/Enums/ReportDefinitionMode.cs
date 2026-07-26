namespace Blazorise.Reporting;

/// <summary>
/// Controls how a declarative report definition is combined with a saved definition.
/// </summary>
public enum ReportDefinitionMode
{
    /// <summary>
    /// Uses the declarative report as the initial seed when no saved definition is supplied.
    /// </summary>
    SeedWhenEmpty,

    /// <summary>
    /// Rebuilds the report from declarative child components on every render.
    /// </summary>
    AlwaysUseDeclarative,

    /// <summary>
    /// Uses only the supplied report definition.
    /// </summary>
    UseDefinitionOnly
}