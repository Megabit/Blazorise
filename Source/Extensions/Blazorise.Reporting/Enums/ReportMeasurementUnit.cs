namespace Blazorise.Reporting;

/// <summary>
/// Defines the measurement unit used when report geometry is edited in the designer.
/// </summary>
public enum ReportMeasurementUnit
{
    /// <summary>
    /// PostScript point, where 72 points equal one inch.
    /// </summary>
    Point,

    /// <summary>
    /// Inch measurement unit.
    /// </summary>
    Inch,

    /// <summary>
    /// Centimeter measurement unit.
    /// </summary>
    Centimeter,

    /// <summary>
    /// Millimeter measurement unit.
    /// </summary>
    Millimeter
}