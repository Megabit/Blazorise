#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Identifies the selected object inside the report designer.
/// </summary>
public sealed class ReportSelectionState
{
    /// <summary>
    /// Kind of report object currently selected.
    /// </summary>
    public ReportSelectionType Type { get; set; } = ReportSelectionType.Report;

    /// <summary>
    /// Selected band identifier when a band or band element is selected.
    /// </summary>
    public string BandId { get; set; }

    /// <summary>
    /// Primary selected element identifier when one or more elements are selected.
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    /// Selected table cell identifier when a table cell is selected.
    /// </summary>
    public string CellId { get; set; }

    /// <summary>
    /// Selected element identifiers when one or more elements are selected.
    /// </summary>
    public List<string> ElementIds { get; set; } = [];
}