namespace Blazorise.Reporting;

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
    Element,

    /// <summary>
    /// A report table cell is selected.
    /// </summary>
    Cell
}