#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a report element on the designer or viewer surface.
/// </summary>
public partial class _ReportDesignerElement
{
    private ElementReference textEditElement;
    private string textEditValue;
    private bool textEditCancelled;
    private bool focusTextEdit;
    private bool textExpressionTokenProtectionActive;
    private JSReportingModule reportingModule;

    private string ImageAlternativeText => Element.Text ?? Element.Name;

    private bool CanEditText => Element?.Type == ReportElementType.Text;

    private bool CanHandleDesignerPointerDown => CanReceiveDesignerInteraction && !Editing;

    private bool CanReceiveDesignerInteraction => DesignMode && Editable;

    private bool CanStartDesignerPointerDrag => CanHandleDesignerPointerDown && !TextEditingActive && !LayoutLocked;

    private bool CanStartInlineTextEdit => CanReceiveDesignerInteraction && !ElementSuppressed && CanEditText;

    private bool ShouldStopPointerDownPropagation => CanHandleDesignerPointerDown && !AllowPointerDragThrough;

    private bool ElementSuppressed => Element?.Suppress?.Value == true;

    private bool IsDesignerDisabled => DesignMode && ( !Editable || ElementSuppressed );

    private bool IsDesignerEditing => CanReceiveDesignerInteraction && !ElementSuppressed && Editing;

    private Func<MouseEventArgs, Task> NonRenderingContextMenu => EventUtil.AsNonRenderingEventHandler<MouseEventArgs>( OnContextMenuAsync );

    private bool ShowResizeHandles => CanReceiveDesignerInteraction && !ElementSuppressed && Selected && !Editing && !LayoutLocked;

    private string Class => ClassNames;

    private string Style => StyleNames;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportElementDefinition>( nameof( Element ), out _ ) )
        {
            DirtyClasses();
            DirtyStyles();
        }

        if ( ( parameters.TryGetValue<ReportDefinition>( nameof( Definition ), out ReportDefinition paramDefinition ) && paramDefinition != Definition )
             || ( parameters.TryGetValue<ReportSectionDefinition>( nameof( Section ), out ReportSectionDefinition paramSection ) && paramSection != Section )
             || ( parameters.TryGetValue<object>( nameof( Data ), out object paramData ) && paramData != Data )
             || ( parameters.TryGetValue<object>( nameof( Item ), out object paramItem ) && paramItem != Item )
             || ( parameters.TryGetValue<IReadOnlyDictionary<string, object>>( nameof( RunningTotals ), out IReadOnlyDictionary<string, object> paramRunningTotals ) && paramRunningTotals != RunningTotals ) )
        {
            DirtyStyles();
        }

        if ( ( parameters.TryGetValue<bool>( nameof( DesignMode ), out bool paramDesignMode ) && paramDesignMode != DesignMode )
             || ( parameters.TryGetValue<bool>( nameof( Editable ), out bool paramEditable ) && paramEditable != Editable )
             || ( parameters.TryGetValue<bool>( nameof( LayoutLocked ), out bool paramLayoutLocked ) && paramLayoutLocked != LayoutLocked )
             || ( parameters.TryGetValue<bool>( nameof( AllowPointerDragThrough ), out bool paramAllowPointerDragThrough ) && paramAllowPointerDragThrough != AllowPointerDragThrough )
             || ( parameters.TryGetValue<bool>( nameof( Selected ), out bool paramSelected ) && paramSelected != Selected )
             || ( parameters.TryGetValue<bool>( nameof( Editing ), out bool paramEditing ) && paramEditing != Editing )
             || ( parameters.TryGetValue<bool>( nameof( TextEditingActive ), out bool paramTextEditingActive ) && paramTextEditingActive != TextEditingActive )
             || ( parameters.TryGetValue<int>( nameof( SelectionVersion ), out int paramSelectionVersion ) && paramSelectionVersion != SelectionVersion ) )
        {
            DirtyClasses();
        }

        if ( ( parameters.TryGetValue<bool>( nameof( DesignMode ), out bool paramDesignModeForStyle ) && paramDesignModeForStyle != DesignMode )
             || ( parameters.TryGetValue<string>( nameof( ElementKey ), out string paramElementKey ) && paramElementKey != ElementKey )
             || ( parameters.TryGetValue<string>( nameof( SelectedCellKey ), out string paramSelectedCellKey ) && paramSelectedCellKey != SelectedCellKey )
             || ( parameters.TryGetValue<string>( nameof( ChildEditingElementKey ), out string paramChildEditingElementKey ) && paramChildEditingElementKey != ChildEditingElementKey ) )
        {
            DirtyStyles();
        }

        if ( parameters.TryGetValue<bool>( nameof( Editing ), out bool paramEditingForFocus ) && paramEditingForFocus && paramEditingForFocus != Editing )
        {
            if ( !parameters.TryGetValue<ReportElementDefinition>( nameof( Element ), out ReportElementDefinition editingElement ) )
                editingElement = Element;

            textEditValue = editingElement?.Text;
            textEditCancelled = false;
            focusTextEdit = true;
        }

        return base.SetParametersAsync( parameters );
    }

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( focusTextEdit )
        {
            focusTextEdit = false;
            await textEditElement.FocusAsync();
            await ProtectTextExpressionTokensAsync();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await ClearTextExpressionTokenProtectionAsync();

        if ( reportingModule is not null )
        {
            try
            {
                await reportingModule.DisposeAsync();
            }
            catch ( JSDisconnectedException )
            {
            }
        }
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-element" );
        builder.Append( $"b-report-element-{Element.Type.ToString().ToLowerInvariant()}" );
        builder.Append( Element.Class );

        builder.Append( "b-report-element-design", DesignMode );
        builder.Append( "suppressed", DesignMode && ElementSuppressed );
        builder.Append( "can-grow", !DesignMode && Element.CanGrow?.Value == true );
        builder.Append( "disabled", IsDesignerDisabled );
        builder.Append( "active", DesignMode && Selected );
        builder.Append( "editing", IsDesignerEditing );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        ReportElementDefinitionHelper.BuildStyle( builder, Element, Definition, Data, Item, Section, DesignMode );
    }

    private string GetFieldText()
    {
        if ( DesignMode )
            return ReportExpressionFormatter.FormatFieldExpression( Definition, Element );

        return ReportDataResolver.FormatValue( ReportExpressionResolver.ResolveFieldValue( Definition, Data, Item, Element, RunningTotals ), Element.Format );
    }

    private string GetText()
    {
        return DesignMode
            ? Element.Text
            : ReportTextTemplateResolver.ResolveText( Definition, Data, Item, Element, RunningTotals );
    }

    private async Task CompleteTextEditAsync()
    {
        if ( !Editing || textEditCancelled )
            return;

        await ClearTextExpressionTokenProtectionAsync();
        await TextEditCommitted.InvokeAsync( textEditValue );
    }

    private async Task HandleTextEditKeyDownAsync( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Key == "Escape" )
        {
            textEditCancelled = true;
            await ClearTextExpressionTokenProtectionAsync();
            await TextEditCancelled.InvokeAsync();
            return;
        }

        if ( eventArgs.Key == "Enter" && eventArgs.CtrlKey )
            await CompleteTextEditAsync();
    }

    private void OnTextEditInput( ChangeEventArgs eventArgs )
    {
        textEditValue = eventArgs.Value?.ToString();
    }

    private Task OnContextMenuAsync( MouseEventArgs eventArgs )
    {
        return ContextMenu?.Invoke( eventArgs ) ?? Task.CompletedTask;
    }

    private async Task ProtectTextExpressionTokensAsync()
    {
        EnsureReportingModule();
        await reportingModule.ProtectTextExpressionTokens( textEditElement );
        textExpressionTokenProtectionActive = true;
    }

    private async Task ClearTextExpressionTokenProtectionAsync()
    {
        if ( !textExpressionTokenProtectionActive || reportingModule is null )
            return;

        try
        {
            await reportingModule.ClearTextExpressionTokenProtection( textEditElement );
        }
        catch ( JSDisconnectedException )
        {
        }

        textExpressionTokenProtectionActive = false;
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Root report data used when resolving field values.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Report definition that owns the element.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report band that owns the element.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }

    /// <summary>
    /// Current band item used for repeated detail rendering.
    /// </summary>
    [Parameter] public object Item { get; set; }

    /// <summary>
    /// Running total values available at the current render position.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, object> RunningTotals { get; set; }

    /// <summary>
    /// Report element definition rendered on the surface.
    /// </summary>
    [Parameter] public ReportElementDefinition Element { get; set; }

    /// <summary>
    /// Stable element key used by the designer selection system.
    /// </summary>
    [Parameter] public string ElementKey { get; set; }

    /// <summary>
    /// Indicates that the element is rendered on the designer surface.
    /// </summary>
    [Parameter] public bool DesignMode { get; set; }

    /// <summary>
    /// Allows the element to receive designer interactions.
    /// </summary>
    [Parameter] public bool Editable { get; set; }

    /// <summary>
    /// Prevents designer drag and resize interactions while allowing selection and editing.
    /// </summary>
    [Parameter] public bool LayoutLocked { get; set; }

    /// <summary>
    /// Allows pointer drag starts to bubble to the parent element.
    /// </summary>
    [Parameter] public bool AllowPointerDragThrough { get; set; }

    /// <summary>
    /// Indicates that the element is part of the current selection.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Version that changes when designer selection changes.
    /// </summary>
    [Parameter] public int SelectionVersion { get; set; }

    /// <summary>
    /// Indicates that the text element is currently edited directly on the designer surface.
    /// </summary>
    [Parameter] public bool Editing { get; set; }

    /// <summary>
    /// Indicates that a text element is currently edited directly on the designer surface.
    /// </summary>
    [Parameter] public bool TextEditingActive { get; set; }

    /// <summary>
    /// Identifier of the child element currently edited inside this element.
    /// </summary>
    [Parameter] public string ChildEditingElementKey { get; set; }

    /// <summary>
    /// Identifier of the selected table cell.
    /// </summary>
    [Parameter] public string SelectedCellKey { get; set; }

    /// <summary>
    /// Raised when the element is clicked on the designer surface.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Raised when the element is double-clicked on the designer surface.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DoubleClicked { get; set; }

    /// <summary>
    /// Raised when the element context menu is requested.
    /// </summary>
    [Parameter] public Func<MouseEventArgs, Task> ContextMenu { get; set; }

    /// <summary>
    /// Raised when a table cell inside this element is clicked.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> TableCellClicked { get; set; }

    /// <summary>
    /// Raised when a table cell context menu is requested.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> TableCellContextMenu { get; set; }

    /// <summary>
    /// Determines whether a child element inside this element is selected.
    /// </summary>
    [Parameter] public Func<string, bool> IsChildElementSelected { get; set; }

    /// <summary>
    /// Raised when a child element inside this element is clicked.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> ChildElementClicked { get; set; }

    /// <summary>
    /// Raised when a child element inside this element is double-clicked.
    /// </summary>
    [Parameter] public Func<string, MouseEventArgs, Task> ChildElementDoubleClicked { get; set; }

    /// <summary>
    /// Raised when inline text editing commits a child element value.
    /// </summary>
    [Parameter] public Func<string, string, Task> ChildElementTextEditCommitted { get; set; }

    /// <summary>
    /// Raised when inline text editing is cancelled for a child element.
    /// </summary>
    [Parameter] public Func<string, Task> ChildElementTextEditCancelled { get; set; }

    /// <summary>
    /// Raised when a table row or column resize starts.
    /// </summary>
    [Parameter] public Func<string, string, ReportTableResizeKind, int, PointerEventArgs, Task> TableResizeStarted { get; set; }

    /// <summary>
    /// Raised when inline text editing commits a new value.
    /// </summary>
    [Parameter] public EventCallback<string> TextEditCommitted { get; set; }

    /// <summary>
    /// Raised when inline text editing is cancelled.
    /// </summary>
    [Parameter] public EventCallback TextEditCancelled { get; set; }

    /// <summary>
    /// Raised when pointer dragging starts on the element.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> PointerDown { get; set; }

    /// <summary>
    /// Raised when resizing starts from the north-west handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> NorthWestResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the north handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> NorthResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the north-east handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> NorthEastResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the east handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> EastResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the south-east handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> SouthEastResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the south handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> SouthResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the south-west handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> SouthWestResizeStarted { get; set; }

    /// <summary>
    /// Raised when resizing starts from the west handle.
    /// </summary>
    [Parameter] public EventCallback<PointerEventArgs> WestResizeStarted { get; set; }
}