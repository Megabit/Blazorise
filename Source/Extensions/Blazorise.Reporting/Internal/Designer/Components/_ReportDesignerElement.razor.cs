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

    private string Class => ClassNames;

    private string Style => StyleNames;

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        var elementDefined = parameters.TryGetValue<ReportElementDefinition>( nameof( Element ), out _ );

        if ( elementDefined
             || ( parameters.TryGetValue<bool>( nameof( DesignMode ), out var paramDesignMode ) && paramDesignMode != DesignMode )
             || ( parameters.TryGetValue<bool>( nameof( Editable ), out var paramEditable ) && paramEditable != Editable )
             || ( parameters.TryGetValue<bool>( nameof( Selected ), out var paramSelected ) && paramSelected != Selected )
             || ( parameters.TryGetValue<bool>( nameof( Editing ), out var paramEditing ) && paramEditing != Editing )
             || ( parameters.TryGetValue<bool>( nameof( TextEditingActive ), out var paramTextEditingActive ) && paramTextEditingActive != TextEditingActive ) )
            DirtyClasses();

        bool definitionDefined = parameters.TryGetValue<ReportDefinition>( nameof( Definition ), out _ );
        bool sectionDefined = parameters.TryGetValue<ReportSectionDefinition>( nameof( Section ), out _ );
        bool dataDefined = parameters.TryGetValue<object>( nameof( Data ), out _ );

        if ( elementDefined || definitionDefined || sectionDefined || dataDefined )
            DirtyStyles();

        if ( parameters.TryGetValue<bool>( nameof( Editing ), out var editing ) && editing && editing != Editing )
        {
            if ( !parameters.TryGetValue<ReportElementDefinition>( nameof( Element ), out var element ) )
                element = Element;

            textEditValue = element?.Text;
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
        builder.Append( "disabled", DesignMode && !Editable );
        builder.Append( "active", DesignMode && Editable && Selected );
        builder.Append( "editing", DesignMode && Editable && Editing );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        ReportElementDefinitionHelper.BuildStyle( builder, Element, Definition, Data, Section );
    }

    private string GetFieldText()
    {
        if ( DesignMode )
            return ReportExpressionFormatter.FormatFieldExpression( Definition, Element );

        return ReportDataResolver.FormatValue( ReportExpressionResolver.ResolveFieldValue( Definition, Data, Item, Element ), Element.Format );
    }

    private string GetText()
    {
        return DesignMode
            ? Element.Text
            : ReportTextTemplateResolver.ResolveText( Definition, Data, Item, Element );
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
    /// Indicates that the element is part of the current selection.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Indicates that the text element is currently edited directly on the designer surface.
    /// </summary>
    [Parameter] public bool Editing { get; set; }

    /// <summary>
    /// Indicates that a text element is currently edited directly on the designer surface.
    /// </summary>
    [Parameter] public bool TextEditingActive { get; set; }

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
    [Parameter] public EventCallback<MouseEventArgs> ContextMenu { get; set; }

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