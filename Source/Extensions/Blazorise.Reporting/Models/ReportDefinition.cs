using System;
using System.Collections.Generic;

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
    /// Ordered report bands that make up the document body.
    /// </summary>
    public List<ReportSectionDefinition> Sections { get; set; } = [];
}

/// <summary>
/// Describes the physical page used for report layout.
/// </summary>
public sealed class ReportPageDefinition
{
    /// <summary>
    /// Named page size used when explicit dimensions are not supplied.
    /// </summary>
    public ReportPageSize Size { get; set; } = ReportPageSize.A4;

    /// <summary>
    /// Page orientation applied to the selected page size.
    /// </summary>
    public ReportOrientation Orientation { get; set; } = ReportOrientation.Portrait;

    /// <summary>
    /// Page width in designer units.
    /// </summary>
    public double Width { get; set; } = 794;

    /// <summary>
    /// Page height in designer units.
    /// </summary>
    public double Height { get; set; } = 1123;
}

/// <summary>
/// Describes a named data source exposed to report fields.
/// </summary>
public sealed class ReportDataSourceDefinition
{
    /// <summary>
    /// Stable identifier used by persisted report state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Data source name used by bands and field expressions.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Source object or enumerable used when resolving report fields.
    /// </summary>
    public object Data { get; set; }
}

/// <summary>
/// Describes a band in a band-based report layout.
/// </summary>
public sealed class ReportSectionDefinition
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
    public ReportSectionType Type { get; set; }

    /// <summary>
    /// Layout mode used for elements inside the band.
    /// </summary>
    public ReportLayout Layout { get; set; } = ReportLayout.Absolute;

    /// <summary>
    /// Band height in designer units.
    /// </summary>
    public double Height { get; set; } = 80;

    /// <summary>
    /// Data source name used by detail bands and field resolution.
    /// </summary>
    public string DataSource { get; set; }

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
    public bool Suppressed { get; set; }

    /// <summary>
    /// Ordered elements placed inside the band.
    /// </summary>
    public List<ReportElementDefinition> Elements { get; set; } = [];
}

/// <summary>
/// Describes a single visual element placed on a report band.
/// </summary>
public sealed class ReportElementDefinition
{
    /// <summary>
    /// Stable identifier used by designer selection and persisted state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly element name shown in the designer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Element kind rendered by the designer and preview.
    /// </summary>
    public ReportElementType Type { get; set; }

    /// <summary>
    /// Horizontal position within the containing band.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Vertical position within the containing band.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Element width in designer units.
    /// </summary>
    public double Width { get; set; } = 120;

    /// <summary>
    /// Element height in designer units.
    /// </summary>
    public double Height { get; set; } = 24;

    /// <summary>
    /// Static text or alternate text associated with the element.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Data field expression rendered by field elements.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Format string applied to resolved field values.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Image source used by image elements.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Optional data source override for field and table content.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// CSS classes applied when the element is rendered.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Inline style applied when the element is rendered.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Column definitions used by table elements.
    /// </summary>
    public List<ReportTableColumnDefinition> Columns { get; set; } = [];
}

/// <summary>
/// Describes a column inside a report table element.
/// </summary>
public sealed class ReportTableColumnDefinition
{
    /// <summary>
    /// Stable identifier used by persisted table state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Header text displayed for the column.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Data field rendered in the column.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Format string applied to column values.
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// Column width in designer units.
    /// </summary>
    public double Width { get; set; } = 120;
}