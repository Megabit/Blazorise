using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

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