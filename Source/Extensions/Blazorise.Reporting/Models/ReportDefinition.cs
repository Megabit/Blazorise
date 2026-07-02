using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes a complete report document, including page setup, data sources, and bands.
/// </summary>
public sealed class ReportDefinition
{
    /// <summary>
    /// Stable identifier used by the designer and persisted report state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly report name shown in designer surfaces.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Page setup used by preview and export renderers.
    /// </summary>
    public ReportPageDefinition Page { get; set; } = new();

    /// <summary>
    /// Data sources available to fields and detail bands.
    /// </summary>
    public List<ReportDataSourceDefinition> DataSources { get; set; } = [];

    /// <summary>
    /// Calculated fields that evaluate formulas at render time and can be placed like source fields.
    /// </summary>
    public List<ReportFormulaFieldDefinition> FormulaFields { get; set; } = [];

    /// <summary>
    /// Stateful summary fields that accumulate values while detail records are rendered.
    /// </summary>
    public List<ReportRunningTotalDefinition> RunningTotals { get; set; } = [];

    /// <summary>
    /// Ordered report bands that make up the document body.
    /// </summary>
    public List<ReportSectionDefinition> Sections { get; set; } = [];
}