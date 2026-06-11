#region Using directives
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.CodeEditor;

/// <summary>
/// Component that allows users to display and edit code.
/// </summary>
public partial class CodeEditor : BaseInputComponent<string>, IAsyncDisposable
{
    #region Members

    private DotNetObjectReference<CodeEditor> dotNetObjectRef;

    private bool jsInitialized;

    private string minHeight = "300px";

    private string maxHeight;

    private ComponentParameterInfo<string> paramLanguage;

    private ComponentParameterInfo<string> paramTheme;

    private ComponentParameterInfo<CodeEditorOptions> paramEditorOptions;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorDiagnostic>> paramDiagnostics;

    private ComponentParameterInfo<string> paramConfigureEditorMethod;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( Language, out paramLanguage );
        parameters.TryGetParameter( Theme, out paramTheme );
        parameters.TryGetParameter( EditorOptions, newOptions => ReferenceEquals( newOptions, EditorOptions ), out paramEditorOptions );
        parameters.TryGetParameter( Diagnostics, newDiagnostics => ReferenceEquals( newDiagnostics, Diagnostics ), out paramDiagnostics );
        parameters.TryGetParameter( ConfigureEditorMethod, out paramConfigureEditorMethod );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        await base.OnAfterSetParametersAsync( parameters );

        if ( !jsInitialized )
            return;

        if ( paramValue.Defined && paramValue.Changed )
        {
            var value = paramValue.Value ?? string.Empty;

            ExecuteAfterRender( async () => await JSModule.SetValue( ElementRef, ElementId, value ) );
        }

        if ( paramLanguage.Changed
             || paramEditorOptions.Changed
             || paramConfigureEditorMethod.Changed )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, CreateJSOptions() ) );
        }

        if ( paramTheme.Changed )
        {
            ExecuteAfterRender( async () => await JSModule.SetTheme( ElementRef, ElementId, Theme ) );
        }

        if ( paramDiagnostics.Changed )
        {
            ExecuteAfterRender( async () => await JSModule.SetDiagnostics( ElementRef, ElementId, Diagnostics ) );
        }
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= DotNetObjectReference.Create( this );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, CreateJSOptions() );
        await JSModule.SetDiagnostics( ElementRef, ElementId, Diagnostics );

        jsInitialized = true;

        await Ready.InvokeAsync( new CodeEditorReadyEventArgs( ElementId, ElementRef ) );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool shouldUpdateOptions = false;

        if ( Rendered )
        {
            if ( ( parameters.TryGetParameter( ReadOnly, out var readOnlyParameter ) && readOnlyParameter.Changed )
                 || ( parameters.TryGetParameter( Disabled, out var disabledParameter ) && disabledParameter.Changed ) )
            {
                shouldUpdateOptions = true;
            }
        }

        await base.SetParametersAsync( parameters );

        if ( jsInitialized && shouldUpdateOptions )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, CreateJSOptions() ) );
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            dotNetObjectRef?.Dispose();
            dotNetObjectRef = null;
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    public override Task Focus( bool scrollToElement = true )
    {
        if ( jsInitialized )
            return JSModule.Focus( ElementRef, ElementId ).AsTask();

        ExecuteAfterRender( () => JSModule.Focus( ElementRef, ElementId ).AsTask() );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the current editor value.
    /// </summary>
    /// <returns>Current editor value.</returns>
    public async Task<string> GetValueAsync()
    {
        if ( jsInitialized )
            return await JSModule.GetValue( ElementRef, ElementId );

        return await ExecuteAfterRenderAsync( async () => await JSModule.GetValue( ElementRef, ElementId ) );
    }

    /// <summary>
    /// Sets the current editor value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetValueAsync( string value )
    {
        value ??= string.Empty;

        if ( !value.IsEqual( Value ) )
            await CurrentValueHandler( value );

        if ( jsInitialized )
        {
            await JSModule.SetValue( ElementRef, ElementId, value );
            return;
        }

        ExecuteAfterRender( async () => await JSModule.SetValue( ElementRef, ElementId, value ) );
    }

    /// <summary>
    /// Refreshes the editor layout.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task LayoutAsync()
    {
        if ( jsInitialized )
            return JSModule.Layout( ElementRef, ElementId ).AsTask();

        ExecuteAfterRender( () => JSModule.Layout( ElementRef, ElementId ).AsTask() );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Formats the current document.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task FormatDocumentAsync()
    {
        if ( jsInitialized )
            return JSModule.FormatDocument( ElementRef, ElementId ).AsTask();

        ExecuteAfterRender( () => JSModule.FormatDocument( ElementRef, ElementId ).AsTask() );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Reveals the specified line.
    /// </summary>
    /// <param name="lineNumber">Line number.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task RevealLineAsync( int lineNumber )
    {
        if ( jsInitialized )
            return JSModule.RevealLine( ElementRef, ElementId, lineNumber ).AsTask();

        ExecuteAfterRender( () => JSModule.RevealLine( ElementRef, ElementId, lineNumber ).AsTask() );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the editor language.
    /// </summary>
    /// <param name="language">Language identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetLanguageAsync( string language )
    {
        Language = language;

        if ( jsInitialized )
            await JSModule.SetLanguage( ElementRef, ElementId, language );
    }

    /// <summary>
    /// Sets the editor theme.
    /// </summary>
    /// <param name="theme">Theme identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetThemeAsync( string theme )
    {
        Theme = theme;

        if ( jsInitialized )
            await JSModule.SetTheme( ElementRef, ElementId, theme );
    }

    /// <summary>
    /// Sets the current editor selection.
    /// </summary>
    /// <param name="selection">Selection range.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SetSelectionAsync( CodeEditorSelection selection )
    {
        if ( jsInitialized )
            return JSModule.SetSelection( ElementRef, ElementId, selection ).AsTask();

        ExecuteAfterRender( () => JSModule.SetSelection( ElementRef, ElementId, selection ).AsTask() );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets the current editor selection.
    /// </summary>
    /// <returns>Current selection.</returns>
    public async Task<CodeEditorSelection> GetSelectionAsync()
    {
        if ( jsInitialized )
            return await JSModule.GetSelection( ElementRef, ElementId );

        return await ExecuteAfterRenderAsync( async () => await JSModule.GetSelection( ElementRef, ElementId ) );
    }

    /// <summary>
    /// Updates the internal editor value. This method should only be called internally.
    /// </summary>
    /// <param name="value">New value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task UpdateInternalValue( string value )
    {
        value ??= string.Empty;

        if ( value.IsEqual( Value ) )
            return Task.CompletedTask;

        return InvokeAsync( async () =>
        {
            await CurrentValueHandler( value );
            await ContentChanged.InvokeAsync( value );
        } );
    }

    /// <summary>
    /// Javascript callback for when editor gets focus.
    /// </summary>
    [JSInvokable]
    public Task OnEditorFocus()
    {
        return EditorFocus.InvokeAsync( true );
    }

    /// <summary>
    /// Javascript callback for when editor loses focus.
    /// </summary>
    [JSInvokable]
    public async Task OnEditorBlur()
    {
        await EditorBlur.InvokeAsync( true );
        await ValidateOnBlurAsync();
    }

    /// <summary>
    /// Executes given action after the rendering is done.
    /// </summary>
    protected async Task<T> ExecuteAfterRenderAsync<T>( Func<Task<T>> action, CancellationToken token = default )
    {
        var source = new TaskCompletionSource<T>();

        token.Register( () => source.TrySetCanceled() );

        ExecuteAfterRender( async () =>
        {
            try
            {
                var result = await action();
                source.TrySetResult( result );
            }
            catch ( TaskCanceledException )
            {
                source.TrySetCanceled();
            }
            catch ( Exception e )
            {
                source.TrySetException( e );
            }
        } );

        return await source.Task.ConfigureAwait( false );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-code-editor" );
        builder.Append( "b-code-editor-disabled", Disabled );
        builder.Append( ClassProvider.MemoInputValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( !string.IsNullOrWhiteSpace( MinHeight ) )
        {
            builder.Append( $"min-height:{MinHeight}" );
        }

        if ( !string.IsNullOrWhiteSpace( MaxHeight ) )
        {
            builder.Append( $"max-height:{MaxHeight}" );
        }

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value ?? string.Empty, null ) );
    }

    private CodeEditorJSOptions CreateJSOptions()
    {
        var editorOptions = EditorOptions ?? new CodeEditorOptions();

        return new CodeEditorJSOptions
        {
            AssetsPath = GlobalOptions.AssetsPath,
            Value = Value ?? string.Empty,
            Language = string.IsNullOrWhiteSpace( Language ) ? CodeEditorLanguage.PlainText : Language,
            Theme = string.IsNullOrWhiteSpace( Theme ) ? CodeEditorTheme.VisualStudio : Theme,
            ReadOnly = ReadOnly,
            Disabled = Disabled,
            AutomaticLayout = editorOptions.AutomaticLayout,
            Minimap = editorOptions.Minimap,
            LineNumbers = editorOptions.LineNumbers,
            WordWrap = editorOptions.WordWrap,
            TabSize = editorOptions.TabSize,
            InsertSpaces = editorOptions.InsertSpaces,
            FormatOnPaste = editorOptions.FormatOnPaste,
            FormatOnType = editorOptions.FormatOnType,
            RenderWhitespace = editorOptions.RenderWhitespace,
            ScrollBeyondLastLine = editorOptions.ScrollBeyondLastLine,
            FontFamily = editorOptions.FontFamily,
            FontSize = editorOptions.FontSize,
            ConfigureEditorMethod = ConfigureEditorMethod,
            AdditionalOptions = editorOptions.AdditionalOptions
        };
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string DefaultValue => string.Empty;

    /// <inheritdoc/>
    protected override bool UsesAutomaticAriaLabelledBy => true;

    /// <inheritdoc/>
    protected override string FieldLabelTargetElementId => null;

    /// <summary>
    /// Gets or sets the JS module.
    /// </summary>
    [Inject] protected JSCodeEditorModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets global code editor options.
    /// </summary>
    [Inject] protected CodeEditorExtensionOptions GlobalOptions { get; set; }

    /// <summary>
    /// Gets or sets the editor language.
    /// </summary>
    [Parameter] public string Language { get; set; } = CodeEditorLanguage.PlainText;

    /// <summary>
    /// Gets or sets the editor theme.
    /// </summary>
    [Parameter] public string Theme { get; set; } = CodeEditorTheme.VisualStudio;

    /// <summary>
    /// Gets or sets additional editor options.
    /// </summary>
    [Parameter] public CodeEditorOptions EditorOptions { get; set; }

    /// <summary>
    /// Gets or sets diagnostic markers.
    /// </summary>
    [Parameter] public IReadOnlyList<CodeEditorDiagnostic> Diagnostics { get; set; }

    /// <summary>
    /// Gets or sets the minimum editor height.
    /// </summary>
    [Parameter]
    public string MinHeight
    {
        get => minHeight;
        set
        {
            if ( minHeight == value )
                return;

            minHeight = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets the maximum editor height.
    /// </summary>
    [Parameter]
    public string MaxHeight
    {
        get => maxHeight;
        set
        {
            if ( maxHeight == value )
                return;

            maxHeight = value;

            DirtyStyles();
        }
    }

    /// <summary>
    /// Gets or sets the custom JavaScript method used to configure editor options before initialization.
    /// </summary>
    [Parameter] public string ConfigureEditorMethod { get; set; }

    /// <summary>
    /// Notifies when the editor is initialized.
    /// </summary>
    [Parameter] public EventCallback<CodeEditorReadyEventArgs> Ready { get; set; }

    /// <summary>
    /// Notifies when editor content changes.
    /// </summary>
    [Parameter] public EventCallback<string> ContentChanged { get; set; }

    /// <summary>
    /// Notifies when the editor gains focus.
    /// </summary>
    [Parameter] public EventCallback EditorFocus { get; set; }

    /// <summary>
    /// Notifies when the editor loses focus.
    /// </summary>
    [Parameter] public EventCallback EditorBlur { get; set; }

    #endregion
}