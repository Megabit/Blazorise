namespace Blazorise.Reporting;

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