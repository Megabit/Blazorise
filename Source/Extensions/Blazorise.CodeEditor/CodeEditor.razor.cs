#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
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

    private readonly List<CodeEditorCustomLanguage> customLanguages = new();

    private bool jsInitialized;

    private string minHeight = "300px";

    private string maxHeight;

    private ComponentParameterInfo<string> paramLanguage;

    private ComponentParameterInfo<string> paramTheme;

    private ComponentParameterInfo<CodeEditorOptions> paramEditorOptions;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorDiagnostic>> paramDiagnostics;

    private ComponentParameterInfo<string> paramConfigureEditorMethod;

    private ComponentParameterInfo<CodeEditorLanguageDefinition> paramLanguageDefinition;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorLanguageDefinition>> paramLanguages;

    private ComponentParameterInfo<CodeEditorCompletionProvider> paramCompletionProvider;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorCompletionItem>> paramCompletionItems;

    private ComponentParameterInfo<IReadOnlyList<string>> paramCompletionTriggerCharacters;

    private ComponentParameterInfo<string> paramConfigureCompletionProviderMethod;

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
        parameters.TryGetParameter( LanguageDefinition, newLanguageDefinition => ReferenceEquals( newLanguageDefinition, LanguageDefinition ), out paramLanguageDefinition );
        parameters.TryGetParameter( Languages, newLanguages => ReferenceEquals( newLanguages, Languages ), out paramLanguages );
        parameters.TryGetParameter( CompletionProvider, newCompletionProvider => ReferenceEquals( newCompletionProvider, CompletionProvider ), out paramCompletionProvider );
        parameters.TryGetParameter( CompletionItems, newCompletionItems => ReferenceEquals( newCompletionItems, CompletionItems ), out paramCompletionItems );
        parameters.TryGetParameter( CompletionTriggerCharacters, newCompletionTriggerCharacters => ReferenceEquals( newCompletionTriggerCharacters, CompletionTriggerCharacters ), out paramCompletionTriggerCharacters );
        parameters.TryGetParameter( ConfigureCompletionProviderMethod, out paramConfigureCompletionProviderMethod );
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

        if ( paramLanguageDefinition.Changed
             || paramLanguages.Changed )
        {
            ExecuteAfterRender( async () => await JSModule.SetLanguages( ElementRef, ElementId, CreateLanguageDefinitions() ) );
        }

        if ( paramLanguage.Changed
             || paramCompletionProvider.Changed
             || paramCompletionItems.Changed
             || paramCompletionTriggerCharacters.Changed
             || paramConfigureCompletionProviderMethod.Changed )
        {
            ExecuteAfterRender( async () => await JSModule.SetCompletionProvider( ElementRef, ElementId, CreateCompletionProvider() ) );
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
    /// Sets custom language definitions.
    /// </summary>
    /// <param name="languages">Custom language definitions.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetLanguagesAsync( IReadOnlyList<CodeEditorLanguageDefinition> languages )
    {
        Languages = languages;

        if ( jsInitialized )
            await JSModule.SetLanguages( ElementRef, ElementId, CreateLanguageDefinitions( languages ) );
    }

    /// <summary>
    /// Sets a custom language definition.
    /// </summary>
    /// <param name="languageDefinition">Custom language definition.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetLanguageDefinitionAsync( CodeEditorLanguageDefinition languageDefinition )
    {
        LanguageDefinition = languageDefinition;

        if ( jsInitialized )
            await JSModule.SetLanguages( ElementRef, ElementId, CreateLanguageDefinitions() );
    }

    /// <summary>
    /// Sets the completion provider.
    /// </summary>
    /// <param name="completionProvider">Completion provider.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetCompletionProviderAsync( CodeEditorCompletionProvider completionProvider )
    {
        CompletionProvider = completionProvider;
        CompletionItems = null;

        if ( jsInitialized )
            await JSModule.SetCompletionProvider( ElementRef, ElementId, CreateCompletionProvider( completionProvider ) );
    }

    /// <summary>
    /// Sets the completion items.
    /// </summary>
    /// <param name="completionItems">Completion items.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetCompletionItemsAsync( IReadOnlyList<CodeEditorCompletionItem> completionItems )
    {
        CompletionProvider = null;
        CompletionItems = completionItems;

        if ( jsInitialized )
            await JSModule.SetCompletionProvider( ElementRef, ElementId, CreateCompletionProvider() );
    }

    /// <summary>
    /// Sets diagnostic markers.
    /// </summary>
    /// <param name="diagnostics">Diagnostic markers.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetDiagnosticsAsync( IReadOnlyList<CodeEditorDiagnostic> diagnostics )
    {
        Diagnostics = diagnostics;

        if ( jsInitialized )
            await JSModule.SetDiagnostics( ElementRef, ElementId, diagnostics );
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
        {
            await JSModule.SetLanguage( ElementRef, ElementId, language );
            await JSModule.SetCompletionProvider( ElementRef, ElementId, CreateCompletionProvider() );
        }
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

    internal void NotifyLanguageInitialized( CodeEditorCustomLanguage customLanguage )
    {
        if ( !customLanguages.Contains( customLanguage ) )
            customLanguages.Add( customLanguage );

        NotifyLanguageChanged();
    }

    internal void NotifyLanguageRemoved( CodeEditorCustomLanguage customLanguage )
    {
        if ( customLanguages.Remove( customLanguage ) )
            NotifyLanguageChanged();
    }

    internal void NotifyLanguageChanged()
    {
        if ( jsInitialized )
            ExecuteAfterRender( async () => await JSModule.SetLanguages( ElementRef, ElementId, CreateLanguageDefinitions() ) );
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
            AdditionalOptions = editorOptions.AdditionalOptions,
            Languages = CreateLanguageDefinitions(),
            CompletionProvider = CreateCompletionProvider()
        };
    }

    private IReadOnlyList<CodeEditorLanguageDefinition> CreateLanguageDefinitions( IReadOnlyList<CodeEditorLanguageDefinition> languages = null )
    {
        List<CodeEditorLanguageDefinition> languageDefinitions = new();

        if ( GlobalOptions.Languages is not null )
            languageDefinitions.AddRange( GlobalOptions.Languages.Where( x => x is not null && !string.IsNullOrWhiteSpace( x.Id ) ) );

        if ( languages is not null )
            languageDefinitions.AddRange( languages.Where( x => x is not null && !string.IsNullOrWhiteSpace( x.Id ) ) );
        else if ( Languages is not null )
            languageDefinitions.AddRange( Languages.Where( x => x is not null && !string.IsNullOrWhiteSpace( x.Id ) ) );

        if ( LanguageDefinition is not null && !string.IsNullOrWhiteSpace( LanguageDefinition.Id ) )
            languageDefinitions.Add( LanguageDefinition );

        languageDefinitions.AddRange( customLanguages.Select( x => x.ToDefinition() ).Where( x => x is not null && !string.IsNullOrWhiteSpace( x.Id ) ) );

        return languageDefinitions;
    }

    private CodeEditorCompletionProvider CreateCompletionProvider( CodeEditorCompletionProvider completionProvider = null )
    {
        CodeEditorCompletionProvider provider = completionProvider ?? CompletionProvider;

        if ( provider is not null )
        {
            return new CodeEditorCompletionProvider
            {
                Language = string.IsNullOrWhiteSpace( provider.Language ) ? Language : provider.Language,
                TriggerCharacters = provider.TriggerCharacters,
                Items = provider.Items,
                ProviderMethod = provider.ProviderMethod
            };
        }

        if ( CompletionItems is null && string.IsNullOrWhiteSpace( ConfigureCompletionProviderMethod ) )
            return null;

        return new CodeEditorCompletionProvider
        {
            Language = Language,
            Items = CompletionItems,
            TriggerCharacters = CompletionTriggerCharacters,
            ProviderMethod = ConfigureCompletionProviderMethod
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
    /// Gets or sets a custom language definition.
    /// </summary>
    [Parameter] public CodeEditorLanguageDefinition LanguageDefinition { get; set; }

    /// <summary>
    /// Gets or sets custom language definitions.
    /// </summary>
    [Parameter] public IReadOnlyList<CodeEditorLanguageDefinition> Languages { get; set; }

    /// <summary>
    /// Gets or sets the completion provider.
    /// </summary>
    [Parameter] public CodeEditorCompletionProvider CompletionProvider { get; set; }

    /// <summary>
    /// Gets or sets completion items.
    /// </summary>
    [Parameter] public IReadOnlyList<CodeEditorCompletionItem> CompletionItems { get; set; }

    /// <summary>
    /// Gets or sets the characters that trigger completion.
    /// </summary>
    [Parameter] public IReadOnlyList<string> CompletionTriggerCharacters { get; set; }

    /// <summary>
    /// Gets or sets the custom JavaScript method used to provide completion items.
    /// </summary>
    [Parameter] public string ConfigureCompletionProviderMethod { get; set; }

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