#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report explorer tree in the designer panel.
/// </summary>
public partial class _ReportDesignerReportExplorer
{
    #region Members

    private string searchText;

    #endregion

    #region Properties

    /// <summary>
    /// Report definition represented by the explorer tree.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Indicates that the root report node is selected.
    /// </summary>
    [Parameter] public bool ReportSelected { get; set; }

    /// <summary>
    /// Index of the selected section.
    /// </summary>
    [Parameter] public int? SelectedSectionIndex { get; set; }

    /// <summary>
    /// Key of the selected primary element.
    /// </summary>
    [Parameter] public string SelectedElementKey { get; set; }

    /// <summary>
    /// Key of the selected table cell.
    /// </summary>
    [Parameter] public string SelectedCellKey { get; set; }

    /// <summary>
    /// Determines whether an element key belongs to the current selection.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementSelected { get; set; }

    /// <summary>
    /// Indicates that subreport elements are shown in the explorer tree.
    /// </summary>
    [Parameter] public bool AllowSubreport { get; set; } = true;

    /// <summary>
    /// Raised when an explorer node is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeClicked { get; set; }

    /// <summary>
    /// Raised when an explorer node context menu is requested.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeMouseEventArgs> NodeContextMenu { get; set; }

    #endregion
}