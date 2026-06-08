#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report explorer tree in the designer panel.
/// </summary>
public partial class _ReportDesignerReportExplorer
{
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
    /// Determines whether an element key belongs to the current selection.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementSelected { get; set; }

    /// <summary>
    /// Raised when an explorer node is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeClicked { get; set; }

    /// <summary>
    /// Raised when an explorer node context menu is requested.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeMouseEventArgs> NodeContextMenu { get; set; }
}