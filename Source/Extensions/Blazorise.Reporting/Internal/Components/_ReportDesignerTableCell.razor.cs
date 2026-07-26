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
/// Renders a table cell on the report designer surface.
/// </summary>
public partial class _ReportDesignerTableCell
{
    #region Methods

    private bool CanReceiveDesignerInteraction => DesignMode && Editable;

    private Task OnCellClicked( MouseEventArgs eventArgs )
    {
        if ( CanReceiveDesignerInteraction )
            return CellClicked.InvokeAsync( new( Cell.Id, eventArgs ) );

        return Task.CompletedTask;
    }

    private Task OnCellContextMenu( MouseEventArgs eventArgs )
    {
        if ( CanReceiveDesignerInteraction && CellContextMenu is not null )
            return CellContextMenu.Invoke( SectionIndex, Cell.Id, eventArgs );

        return Task.CompletedTask;
    }

    private Task OnElementDoubleClicked( string elementKey, MouseEventArgs eventArgs )
    {
        if ( CanReceiveDesignerInteraction && ElementDoubleClicked is not null )
            return ElementDoubleClicked.Invoke( elementKey, eventArgs );

        return Task.CompletedTask;
    }

    private Task OnElementContextMenu( string cellKey, MouseEventArgs eventArgs )
    {
        if ( CanReceiveDesignerInteraction && CellContextMenu is not null )
            return CellContextMenu.Invoke( SectionIndex, cellKey, eventArgs );

        return Task.CompletedTask;
    }

    private Task OnElementTextEditCommitted( string elementKey, string text )
    {
        if ( CanReceiveDesignerInteraction && ElementTextEditCommitted is not null )
            return ElementTextEditCommitted.Invoke( elementKey, text );

        return Task.CompletedTask;
    }

    private Task OnElementTextEditCancelled( string elementKey )
    {
        if ( CanReceiveDesignerInteraction && ElementTextEditCancelled is not null )
            return ElementTextEditCancelled.Invoke( elementKey );

        return Task.CompletedTask;
    }

    private Task OnColumnResizePointerDown( PointerEventArgs eventArgs )
    {
        if ( CanReceiveDesignerInteraction && ResizeStarted is not null )
            return ResizeStarted.Invoke( TableElementKey, Cell.Id, ReportTableResizeKind.Column, Cell.ColumnIndex + Math.Max( 1, Cell.ColumnSpan ) - 1, eventArgs );

        return Task.CompletedTask;
    }

    private Task OnRowResizePointerDown( PointerEventArgs eventArgs )
    {
        if ( CanReceiveDesignerInteraction && ResizeStarted is not null )
            return ResizeStarted.Invoke( TableElementKey, Cell.Id, ReportTableResizeKind.Row, Cell.RowIndex + Math.Max( 1, Cell.RowSpan ) - 1, eventArgs );

        return Task.CompletedTask;
    }

    private IEnumerable<ReportElementDefinition> GetRenderableElements()
    {
        if ( Cell?.Elements is null )
            return [];

        if ( DesignMode )
            return Cell.Elements;

        return Cell.Elements.Where( element => !ReportValueResolver.ResolveSuppress( element, Section, Definition, Data, Item ) );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Report data used when resolving nested field values.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Report definition that owns the table.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report band that owns the table.
    /// </summary>
    [Parameter] public ReportBandDefinition Section { get; set; }

    /// <summary>
    /// Report section index rendered on the designer surface.
    /// </summary>
    [Parameter] public int SectionIndex { get; set; }

    /// <summary>
    /// Current band item used for repeated detail rendering.
    /// </summary>
    [Parameter] public object Item { get; set; }

    /// <summary>
    /// Running total values available at the current render position.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, object> RunningTotals { get; set; }

    /// <summary>
    /// Identifier of the parent table element.
    /// </summary>
    [Parameter] public string TableElementKey { get; set; }

    /// <summary>
    /// Table cell rendered by this component.
    /// </summary>
    [Parameter] public ReportTableCellDefinition Cell { get; set; }

    /// <summary>
    /// CSS class for the table cell wrapper.
    /// </summary>
    [Parameter] public string CellClass { get; set; }

    /// <summary>
    /// Inline style for the table cell wrapper.
    /// </summary>
    [Parameter] public string CellStyle { get; set; }

    /// <summary>
    /// Indicates that the table is rendered on the designer surface.
    /// </summary>
    [Parameter] public bool DesignMode { get; set; }

    /// <summary>
    /// Allows the table cell to receive designer interactions.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Indicates that the table element is part of the current selection.
    /// </summary>
    [Parameter] public bool TableSelected { get; set; }

    /// <summary>
    /// Raised when a table cell is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> CellClicked { get; set; }

    /// <summary>
    /// Raised when a table cell context menu is requested.
    /// </summary>
    [Parameter] public Func<int, string, MouseEventArgs, Task> CellContextMenu { get; set; }

    /// <summary>
    /// Determines whether a nested table cell element is selected.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementSelected { get; set; }

    /// <summary>
    /// Determines whether a nested table cell element overlaps a sibling element.
    /// </summary>
    [Parameter] public Func<string, bool> IsElementColliding { get; set; }

    /// <summary>
    /// Raised when a nested table cell element is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportDesignerSelectionMouseEventArgs> ElementClicked { get; set; }

    /// <summary>
    /// Raised when a nested table cell element is double-clicked.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> ElementDoubleClicked { get; set; }

    /// <summary>
    /// Indicates that a text element is currently edited directly on the designer surface.
    /// </summary>
    [Parameter] public bool TextEditingActive { get; set; }

    /// <summary>
    /// Identifier of the nested table cell element currently edited.
    /// </summary>
    [Parameter] public string EditingElementKey { get; set; }

    /// <summary>
    /// Raised when inline text editing commits a nested table cell element value.
    /// </summary>
    [Parameter] public Func<string, string, Task> ElementTextEditCommitted { get; set; }

    /// <summary>
    /// Raised when inline text editing is cancelled for a nested table cell element.
    /// </summary>
    [Parameter] public Func<string, Task> ElementTextEditCancelled { get; set; }

    /// <summary>
    /// Raised when a table row or column resize starts.
    /// </summary>
    [Parameter] public Func<string, string, ReportTableResizeKind, int, PointerEventArgs, Task> ResizeStarted { get; set; }

    #endregion
}