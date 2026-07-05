namespace Blazorise.Reporting;

/// <summary>
/// Defines the role of a report band.
/// </summary>
public enum ReportSectionType
{
    /// <summary>
    /// Band rendered once at the beginning of the report.
    /// </summary>
    ReportHeader = 0,

    /// <summary>
    /// Repeated band used for rows from a data source.
    /// </summary>
    Detail = 1,

    /// <summary>
    /// Band rendered once at the end of the report.
    /// </summary>
    ReportFooter = 2,

    /// <summary>
    /// Legacy grouping alias.
    /// </summary>
    Group = 3,

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
    PageFooter = 7
}
