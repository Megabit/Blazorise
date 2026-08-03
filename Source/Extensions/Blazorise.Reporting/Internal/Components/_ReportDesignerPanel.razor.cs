#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders child elements inside a report panel.
/// </summary>
public partial class _ReportDesignerPanel
{
    #region Methods

    private IEnumerable<ReportElementDefinition> GetRenderableElements()
    {
        if ( Element?.Elements is null )
            return [];

        if ( DesignMode )
            return Element.Elements;

        return Element.Elements.Where( element => !ReportValueResolver.ResolveSuppress( element, Section, Definition, Data, Item ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Root data object used while rendering panel content.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Report definition that owns the panel.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Band containing the panel.
    /// </summary>
    [Parameter] public ReportBandDefinition Section { get; set; }

    /// <summary>
    /// Position of the containing band in the report.
    /// </summary>
    [Parameter] public int SectionIndex { get; set; }

    /// <summary>
    /// Current data item for expression evaluation.
    /// </summary>
    [Parameter] public object Item { get; set; }

    /// <summary>
    /// Running-total values available to child elements.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, object> RunningTotals { get; set; }

    /// <summary>
    /// Revision used to refresh mutated report content.
    /// </summary>
    [Parameter] public int RenderMutationVersion { get; set; }

    /// <summary>
    /// Panel definition being rendered.
    /// </summary>
    [Parameter] public ReportPanelElementDefinition Element { get; set; }

    /// <summary>
    /// Indicates whether designer interactions are active.
    /// </summary>
    [Parameter] public bool DesignMode { get; set; }

    /// <summary>
    /// Allows child elements to be edited.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Prevents child elements from being moved or resized.
    /// </summary>
    [Parameter] public bool LayoutLocked { get; set; }

    /// <summary>
    /// Revision used to refresh selection state.
    /// </summary>
    [Parameter] public int SelectionVersion { get; set; }

    /// <summary>
    /// Indicates that inline text editing is in progress.
    /// </summary>
    [Parameter] public bool TextEditingActive { get; set; }

    /// <summary>
    /// Identifies the element currently being edited.
    /// </summary>
    [Parameter] public string EditingElementKey { get; set; }

    /// <summary>
    /// Identifies the selected table cell.
    /// </summary>
    [Parameter] public string SelectedCellKey { get; set; }

    /// <summary>
    /// Resolves whether a child element is selected.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementSelected { get; set; }

    /// <summary>
    /// Resolves whether a child element overlaps a sibling.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementColliding { get; set; }

    /// <summary>
    /// Handles clicks on child elements.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> ElementClicked { get; set; }

    /// <summary>
    /// Handles double-clicks on child elements.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> ElementDoubleClicked { get; set; }

    /// <summary>
    /// Handles context-menu requests from child elements.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> ElementContextMenu { get; set; }

    /// <summary>
    /// Handles table-cell selection clicks.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> TableCellClicked { get; set; }

    /// <summary>
    /// Handles context-menu requests from table cells.
    /// </summary>
    [Parameter] public Func<int, string, MouseEventArgs, Task> TableCellContextMenu { get; set; }

    /// <summary>
    /// Commits inline text changes for a child element.
    /// </summary>
    [Parameter] public Func<string, string, Task> ElementTextEditCommitted { get; set; }

    /// <summary>
    /// Cancels inline text editing for a child element.
    /// </summary>
    [Parameter] public Func<string, Task> ElementTextEditCancelled { get; set; }

    /// <summary>
    /// Starts a table row or column resize operation.
    /// </summary>
    [Parameter] public Func<string, string, ReportTableResizeKind, int, PointerEventArgs, Task> TableResizeStarted { get; set; }

    /// <summary>
    /// Starts pointer dragging for a child element.
    /// </summary>
    [Parameter] public Func<string, PointerEventArgs, Task> ElementPointerDown { get; set; }

    /// <summary>
    /// Starts resizing a child element from a handle.
    /// </summary>
    [Parameter] public Func<string, int, PointerEventArgs, Task> ElementResizeStarted { get; set; }

    #endregion
}