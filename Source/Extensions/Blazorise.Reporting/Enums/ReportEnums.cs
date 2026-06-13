using System;

namespace Blazorise.Reporting;

/// <summary>
/// Defines the active report studio surface.
/// </summary>
public enum ReportStudioMode
{
    /// <summary>
    /// Shows the interactive designer surface.
    /// </summary>
    Design,

    /// <summary>
    /// Shows the rendered report output.
    /// </summary>
    Preview
}

/// <summary>
/// Defines the preview formats supported by a report viewer.
/// </summary>
[Flags]
public enum ReportPreviewFormat
{
    /// <summary>
    /// Renders the report as browser HTML.
    /// </summary>
    Html = 1,

    /// <summary>
    /// Renders the report as PDF content.
    /// </summary>
    Pdf = 2
}

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

/// <summary>
/// Defines how report bands are shown in the designer surface.
/// </summary>
public enum ReportBandMode
{
    /// <summary>
    /// Shows a left rail with collapsible band labels.
    /// </summary>
    Rail,

    /// <summary>
    /// Shows section separators inside the report page.
    /// </summary>
    Separator,

    /// <summary>
    /// Shows a compact band presentation.
    /// </summary>
    Compact
}

/// <summary>
/// Defines layout behavior for elements inside a report section.
/// </summary>
public enum ReportLayout
{
    /// <summary>
    /// Positions elements by absolute X and Y coordinates.
    /// </summary>
    Absolute,

    /// <summary>
    /// Arranges elements in document order.
    /// </summary>
    Stack,

    /// <summary>
    /// Arranges elements using flex layout rules.
    /// </summary>
    Flex,

    /// <summary>
    /// Arranges elements using grid layout rules.
    /// </summary>
    Grid
}

/// <summary>
/// Defines common report page sizes.
/// </summary>
public enum ReportPageSize
{
    /// <summary>
    /// ISO A4 page size.
    /// </summary>
    A4 = 0,

    /// <summary>
    /// North American letter page size.
    /// </summary>
    Letter = 1,

    /// <summary>
    /// Explicit page dimensions supplied by the report definition.
    /// </summary>
    Custom = 2,

    /// <summary>
    /// ISO A3 page size.
    /// </summary>
    A3 = 3,

    /// <summary>
    /// ISO A5 page size.
    /// </summary>
    A5 = 4,

    /// <summary>
    /// North American legal page size.
    /// </summary>
    Legal = 5
}

/// <summary>
/// Defines the report page orientation.
/// </summary>
public enum ReportOrientation
{
    /// <summary>
    /// Taller than it is wide.
    /// </summary>
    Portrait,

    /// <summary>
    /// Wider than it is tall.
    /// </summary>
    Landscape
}

/// <summary>
/// Defines the role of a report band.
/// </summary>
public enum ReportSectionType
{
    /// <summary>
    /// Legacy report header alias.
    /// </summary>
    Header = 0,

    /// <summary>
    /// Repeated band used for rows from a data source.
    /// </summary>
    Detail = 1,

    /// <summary>
    /// Legacy report footer alias.
    /// </summary>
    Footer = 2,

    /// <summary>
    /// Legacy grouping alias.
    /// </summary>
    Group = 3,

    /// <summary>
    /// Band rendered once at the beginning of the report.
    /// </summary>
    ReportHeader = Header,

    /// <summary>
    /// Band rendered at the top of each page.
    /// </summary>
    PageHeader = 4,

    /// <summary>
    /// Band rendered before a grouped detail section.
    /// </summary>
    GroupHeader = 5,

    /// <summary>
    /// Band rendered after a grouped detail section.
    /// </summary>
    GroupFooter = 6,

    /// <summary>
    /// Band rendered at the bottom of each page.
    /// </summary>
    PageFooter = 7,

    /// <summary>
    /// Band rendered once at the end of the report.
    /// </summary>
    ReportFooter = Footer
}

/// <summary>
/// Defines the report elements supported by the designer.
/// </summary>
public enum ReportElementType
{
    /// <summary>
    /// Static text content.
    /// </summary>
    Text,

    /// <summary>
    /// Data-bound field content.
    /// </summary>
    Field,

    /// <summary>
    /// Table layout container.
    /// </summary>
    Table,

    /// <summary>
    /// Image element.
    /// </summary>
    Image,

    /// <summary>
    /// Horizontal or vertical line element.
    /// </summary>
    Line,

    /// <summary>
    /// Rectangular shape element.
    /// </summary>
    Rectangle,

    /// <summary>
    /// Explicit page break marker.
    /// </summary>
    PageBreak
}

/// <summary>
/// Defines aggregate functions available for data-bound report fields.
/// </summary>
public enum ReportAggregateFunction
{
    /// <summary>
    /// Counts resolved field values.
    /// </summary>
    Count,

    /// <summary>
    /// Sums numeric field values.
    /// </summary>
    Sum,

    /// <summary>
    /// Calculates the average of numeric field values.
    /// </summary>
    Average,

    /// <summary>
    /// Resolves the smallest comparable field value.
    /// </summary>
    Minimum,

    /// <summary>
    /// Resolves the largest comparable field value.
    /// </summary>
    Maximum
}

/// <summary>
/// Defines commands that can be issued from report toolbars.
/// </summary>
public enum ReportCommand
{
    /// <summary>
    /// Switches to design mode.
    /// </summary>
    Design,

    /// <summary>
    /// Switches to the default preview mode.
    /// </summary>
    Preview,

    /// <summary>
    /// Switches to HTML preview mode.
    /// </summary>
    PreviewHtml,

    /// <summary>
    /// Switches to PDF preview mode.
    /// </summary>
    PreviewPdf,

    /// <summary>
    /// Opens the data source connection dialog.
    /// </summary>
    ConnectDataSource,

    /// <summary>
    /// Removes the selected element and places it on the report clipboard.
    /// </summary>
    Cut,

    /// <summary>
    /// Copies the selected element to the report clipboard.
    /// </summary>
    Copy,

    /// <summary>
    /// Inserts the report clipboard element.
    /// </summary>
    Paste,

    /// <summary>
    /// Removes the current report selection.
    /// </summary>
    Delete,

    /// <summary>
    /// Reverts the previous designer command.
    /// </summary>
    Undo,

    /// <summary>
    /// Reapplies the previously undone designer command.
    /// </summary>
    Redo,

    /// <summary>
    /// Restores the report to its seed definition.
    /// </summary>
    Reset
}

/// <summary>
/// Defines the kind of object selected in the report designer.
/// </summary>
public enum ReportSelectionType
{
    /// <summary>
    /// The report surface is selected.
    /// </summary>
    Report,

    /// <summary>
    /// A report band is selected.
    /// </summary>
    Section,

    /// <summary>
    /// A report element is selected.
    /// </summary>
    Element
}