#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes one named design page, its page setup, and its bands.
/// </summary>
public sealed class ReportPageDefinition
{
    /// <summary>
    /// Stable identifier used by the designer and persisted report definition.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Friendly page name shown in designer surfaces.
    /// </summary>
    public string Name { get; set; }

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

    /// <summary>
    /// Ordered report bands that make up this design page.
    /// </summary>
    public List<ReportBandDefinition> Bands { get; set; } = [];
}