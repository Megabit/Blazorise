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
    /// Unit used by designer property editors when displaying and editing report geometry.
    /// </summary>
    public ReportMeasurementUnit MeasurementUnit { get; set; } = ReportMeasurementUnit.Centimeter;

    /// <summary>
    /// Page orientation applied to the selected page size.
    /// </summary>
    public ReportOrientation Orientation { get; set; } = ReportOrientation.Portrait;

    /// <summary>
    /// Page width in points.
    /// </summary>
    public double Width { get; set; } = 595.2755905511812d;

    /// <summary>
    /// Page height in points.
    /// </summary>
    public double Height { get; set; } = 841.8897637795276d;

    /// <summary>
    /// Printable page margins in points.
    /// </summary>
    public ReportPageMarginsDefinition Margins { get; set; } = new();
}

/// <summary>
/// Describes the printable area inset from each page edge.
/// </summary>
public sealed class ReportPageMarginsDefinition
{
    /// <summary>
    /// Left page margin in points.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Top page margin in points.
    /// </summary>
    public double Top { get; set; }

    /// <summary>
    /// Right page margin in points.
    /// </summary>
    public double Right { get; set; }

    /// <summary>
    /// Bottom page margin in points.
    /// </summary>
    public double Bottom { get; set; }
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
    /// Provider type used to resolve designer schema and runtime data.
    /// </summary>
    public string Type { get; set; } = ObjectReportDataSourceProvider.ProviderType;

    /// <summary>
    /// Source object or enumerable used when resolving report fields and data source paths.
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// Provider-specific settings stored with the report definition.
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = [];

    /// <summary>
    /// Field schema exposed by providers that cannot be reflected from a live object.
    /// </summary>
    public ReportDataSourceSchema Schema { get; set; }
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
    /// Static suppress value used by designer interactions.
    /// </summary>
    public bool Suppressed
    {
        get => Suppress?.Value ?? false;
        set => Suppress = ReportValue.Create( value, Suppress?.Formula );
    }

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
    public bool KeepTogether { get; set; }

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
    /// Horizontal position within the containing band, in points.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Vertical position within the containing band, in points.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Element width in points.
    /// </summary>
    public double Width { get; set; } = 90;

    /// <summary>
    /// Element height in points.
    /// </summary>
    public double Height { get; set; } = 18;

    /// <summary>
    /// Allows text content to expand the element vertically when rendered.
    /// </summary>
    public ReportValue<bool> CanGrow { get; set; } = false;

    /// <summary>
    /// Prevents the element from being edited on the designer surface and rendered in preview output.
    /// </summary>
    public ReportValue<bool> Suppress { get; set; } = false;

    /// <summary>
    /// Overrides the report-level snap-to-grid behavior for this element. A null value inherits the report setting.
    /// </summary>
    public ReportValue<bool?> SnapToGrid { get; set; } = (bool?)null;

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
    /// Font settings applied to text rendered by the element.
    /// </summary>
    public ReportFontDefinition Font { get; set; } = new();

    /// <summary>
    /// Fill and opacity settings applied by the designer appearance editor.
    /// </summary>
    public ReportAppearanceDefinition Appearance { get; set; } = new();

    /// <summary>
    /// Border settings applied around the element.
    /// </summary>
    public ReportBorderDefinition Border { get; set; } = new();

    /// <summary>
    /// CSS classes applied when the element is rendered.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Inline style applied when the element is rendered.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// Aggregate function applied when the field is rendered as a summary value.
    /// </summary>
    public ReportAggregateDefinition Aggregate { get; set; }

    /// <summary>
    /// Column definitions used by table elements.
    /// </summary>
    public List<ReportTableColumnDefinition> Columns { get; set; } = [];
}

/// <summary>
/// Describes an aggregate operation applied to a report field element.
/// </summary>
public sealed class ReportAggregateDefinition
{
    /// <summary>
    /// Aggregate function used to calculate the field summary value.
    /// </summary>
    public ReportAggregateFunction Function { get; set; }
}

/// <summary>
/// Describes font settings for report elements that render text.
/// </summary>
public sealed class ReportFontDefinition
{
    /// <summary>
    /// Font family applied to text rendered by the element.
    /// </summary>
    public string Family { get; set; }

    /// <summary>
    /// Font size applied to text rendered by the element.
    /// </summary>
    public double? Size { get; set; }

    /// <summary>
    /// Text color applied by the designer text editor.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Semantic Blazorise text color name applied to text rendered by the element.
    /// </summary>
    public string TextColor { get; set; }

    /// <summary>
    /// Enables bold text rendering.
    /// </summary>
    public bool Bold { get; set; }

    /// <summary>
    /// Enables italic text rendering.
    /// </summary>
    public bool Italic { get; set; }

    /// <summary>
    /// Enables underline text rendering.
    /// </summary>
    public bool Underline { get; set; }

    /// <summary>
    /// Text alignment applied inside the element box.
    /// </summary>
    public TextAlignment Alignment { get; set; } = TextAlignment.Default;
}

/// <summary>
/// Describes fill and opacity settings for report elements.
/// </summary>
public sealed class ReportAppearanceDefinition
{
    /// <summary>
    /// Background color applied by the designer appearance editor.
    /// </summary>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Semantic Blazorise background color name applied to the element fill.
    /// </summary>
    public string Background { get; set; }

    /// <summary>
    /// Element opacity from 0 to 1.
    /// </summary>
    public double? Opacity { get; set; }
}

/// <summary>
/// Describes border settings for report elements.
/// </summary>
public sealed class ReportBorderDefinition
{
    /// <summary>
    /// Border color applied by the designer appearance editor.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Border width applied around the element.
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// Border radius applied to the element corners.
    /// </summary>
    public double? Radius { get; set; }
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
    /// Column width in points.
    /// </summary>
    public double Width { get; set; } = 90;
}