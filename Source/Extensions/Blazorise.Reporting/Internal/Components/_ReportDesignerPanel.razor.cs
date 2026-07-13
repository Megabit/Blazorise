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

    [Parameter] public object Data { get; set; }

    [Parameter] public ReportDefinition Definition { get; set; }

    [Parameter] public ReportBandDefinition Section { get; set; }

    [Parameter] public int SectionIndex { get; set; }

    [Parameter] public object Item { get; set; }

    [Parameter] public IReadOnlyDictionary<string, object> RunningTotals { get; set; }

    [Parameter] public int RenderMutationVersion { get; set; }

    [Parameter] public ReportPanelElementDefinition Element { get; set; }

    [Parameter] public bool DesignMode { get; set; }

    [Parameter] public bool Editable { get; set; }

    [Parameter] public bool LayoutLocked { get; set; }

    [Parameter] public int SelectionVersion { get; set; }

    [Parameter] public bool TextEditingActive { get; set; }

    [Parameter] public string EditingElementKey { get; set; }

    [Parameter] public string SelectedCellKey { get; set; }

    [Parameter] public Func<string, bool> IsElementSelected { get; set; }

    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> ElementClicked { get; set; }

    [Parameter] public Func<string, MouseEventArgs, Task> ElementDoubleClicked { get; set; }

    [Parameter] public Func<string, MouseEventArgs, Task> ElementContextMenu { get; set; }

    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> TableCellClicked { get; set; }

    [Parameter] public Func<int, string, MouseEventArgs, Task> TableCellContextMenu { get; set; }

    [Parameter] public Func<string, string, Task> ElementTextEditCommitted { get; set; }

    [Parameter] public Func<string, Task> ElementTextEditCancelled { get; set; }

    [Parameter] public Func<string, string, ReportTableResizeKind, int, PointerEventArgs, Task> TableResizeStarted { get; set; }

    [Parameter] public Func<string, PointerEventArgs, Task> ElementPointerDown { get; set; }

    [Parameter] public Func<string, int, PointerEventArgs, Task> ElementResizeStarted { get; set; }

    #endregion
}