#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a band in a band-based report layout.
/// </summary>
public sealed class ReportBandDefinition
{
    /// <summary>
    /// Stable identifier used by designer selection and persisted state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly band name shown in the designer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Band role in the report rendering pipeline.
    /// </summary>
    public ReportBandType Type { get; set; }

    /// <summary>
    /// Band height in points.
    /// </summary>
    public double Height { get; set; } = 60;

    /// <summary>
    /// Data source name or path used as the band field context. Detail bands repeat when this value resolves to a collection.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Field expression used by group header bands to split detail rows into groups.
    /// </summary>
    public string GroupBy { get; set; }

    /// <summary>
    /// CSS classes applied when the band is rendered.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Inline style applied when the band is rendered.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Indicates that the band came from the declarative seed and cannot be deleted.
    /// </summary>
    public bool Default { get; set; }

    /// <summary>
    /// Excludes the band from rendered output while keeping it visible in the designer.
    /// </summary>
    public ReportValue<bool> Suppress { get; set; } = false;

    /// <summary>
    /// Keeps the band height reserved when the band is suppressed.
    /// </summary>
    public bool ReserveSpaceWhenSuppressed { get; set; }

    /// <summary>
    /// Allows page footer bands to render on the first page.
    /// </summary>
    public bool PrintOnFirstPage { get; set; } = true;

    /// <summary>
    /// Allows page footer bands to render on the last page.
    /// </summary>
    public bool PrintOnLastPage { get; set; } = true;

    /// <summary>
    /// Allows page footer bands to repeat on every rendered page.
    /// </summary>
    public bool RepeatOnEveryPage { get; set; } = true;

    /// <summary>
    /// Keeps the band content together when pagination is applied.
    /// </summary>
    public ReportValue<bool> KeepTogether { get; set; } = false;

    /// <summary>
    /// Starts the band on a new page before rendering it.
    /// </summary>
    public ReportValue<bool> NewPageBefore { get; set; } = false;

    /// <summary>
    /// Starts a new page after the band is rendered.
    /// </summary>
    public ReportValue<bool> NewPageAfter { get; set; } = false;

    /// <summary>
    /// Fill and opacity settings applied to the band background.
    /// </summary>
    public ReportAppearanceDefinition Appearance { get; set; } = new();

    /// <summary>
    /// Border settings applied around the band.
    /// </summary>
    public ReportBorderDefinition Border { get; set; } = new();

    /// <summary>
    /// Ordered elements placed inside the band.
    /// </summary>
    public List<ReportElementDefinition> Elements { get; set; } = [];
}