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

    private static readonly CodeEditorOptions defaultEditorOptions = new();

    private DotNetObjectReference<CodeEditor> dotNetObjectRef;

    private readonly List<CodeEditorCustomLanguage> customLanguages = new();

    private bool jsInitialized;

    private string minHeight = "300px";

    private string maxHeight;

    private ComponentParameterInfo<string> paramLanguage;

    private ComponentParameterInfo<string> paramTheme;

    private ComponentParameterInfo<CodeEditorOptions> paramEditorOptions;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorDiagnostic>> paramDiagnostics;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorLanguageDefinition>> paramLanguages;

    private ComponentParameterInfo<CodeEditorCompletionProvider> paramCompletionProvider;

    private ComponentParameterInfo<IReadOnlyList<CodeEditorCompletionItem>> paramCompletionItems;

    private ComponentParameterInfo<IReadOnlyList<string>> paramCompletionTriggerCharacters;

    private ComponentParameterInfo<CodeEditorDocumentFormattingProvider> paramFormattingProvider;

    private ComponentParameterInfo<bool> paramDebounce;

    private ComponentParameterInfo<int?> paramDebounceInterval;

    private ComponentParameterInfo<bool> paramImmediate;

    private ComponentParameterInfo<int?> paramTabIndex;

    private ComponentParameterInfo<bool> paramReadOnly;

    private ComponentParameterInfo<bool> paramDisabled;

    private ComponentParameterInfo<Dictionary<string, object>> paramAttributes;

    private bool editorOptionsUpdateScheduled;

    private bool languagesUpdateScheduled;

    private bool completionProviderUpdateScheduled;

    private bool formattingProviderUpdateScheduled;

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
        parameters.TryGetParameter( Languages, newLanguages => ReferenceEquals( newLanguages, Languages ), out paramLanguages );
        parameters.TryGetParameter( CompletionProvider, newCompletionProvider => ReferenceEquals( newCompletionProvider, CompletionProvider ), out paramCompletionProvider );
        parameters.TryGetParameter( CompletionItems, newCompletionItems => ReferenceEquals( newCompletionItems, CompletionItems ), out paramCompletionItems );
        parameters.TryGetParameter( CompletionTriggerCharacters, newCompletionTriggerCharacters => ReferenceEquals( newCompletionTriggerCharacters, CompletionTriggerCharacters ), out paramCompletionTriggerCharacters );
        parameters.TryGetParameter( FormattingProvider, newFormattingProvider => ReferenceEquals( newFormattingProvider, FormattingProvider ), out paramFormattingProvider );
        parameters.TryGetParameter( Debounce, out paramDebounce );
        parameters.TryGetParameter( DebounceInterval, out paramDebounceInterval );
        parameters.TryGetParameter( Immediate, out paramImmediate );
        parameters.TryGetParameter( TabIndex, out paramTabIndex );
        parameters.TryGetParameter( ReadOnly, out paramReadOnly );
        parameters.TryGetParameter( Disabled, out paramDisabled );
        parameters.TryGetParameter( Attributes, newAttributes => ReferenceEquals( newAttributes, Attributes ), out paramAttributes );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterSetParametersAsync( ParameterView parameters )
    {
        await base.OnAfterSetParametersAsync( parameters );

        if ( !jsInitialized )
            return;

        if ( paramValue.Defined && paramValue.Changed )
        {
            string value = paramValue.Value ?? string.Empty;

            ExecuteAfterRender( () => JSModule.SetValue( ElementRef, ElementId, value ).AsTask() );
        }

        if ( paramEditorOptions.Changed
             || paramImmediate.Changed
             || paramDebounce.Changed
             || paramDebounceInterval.Changed
             || paramTabIndex.Changed
             || paramReadOnly.Changed
             || paramDisabled.Changed
             || paramAttributes.Changed
             || paramAriaInvalid.Changed
             || paramAriaRequired.Changed
             || paramAriaDescribedBy.Changed
             || paramAriaLabelledBy.Changed )
        {
            ScheduleEditorOptionsUpdate();
        }

        if ( paramLanguages.Changed )
        {
            ScheduleLanguagesUpdate();
        }

        if ( paramLanguage.Changed )
        {
            ExecuteAfterRender( () => JSModule.SetLanguage( ElementRef, ElementId, ResolveLanguage() ).AsTask() );
        }

        if ( paramTheme.Changed )
        {
            ExecuteAfterRender( () => JSModule.SetTheme( ElementRef, ElementId, ResolveTheme() ).AsTask() );
        }

        if ( paramDiagnostics.Changed )
        {
            ExecuteAfterRender( () => JSModule.SetDiagnostics( ElementRef, ElementId, Diagnostics ).AsTask() );
        }

        if ( paramLanguage.Changed
             || paramCompletionProvider.Changed
             || paramCompletionItems.Changed
             || paramCompletionTriggerCharacters.Changed )
        {
            ScheduleCompletionProviderUpdate();
        }

        if ( paramLanguage.Changed
             || paramFormattingProvider.Changed )
        {
            ScheduleFormattingProviderUpdate();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= DotNetObjectReference.Create( this );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, CreateJSOptions() );
        await JSModule.SetDiagnostics( ElementRef, ElementId, Diagnostics );

        jsInitialized = true;

        await Ready.InvokeAsync( new CodeEditorReadyEventArgs( this ) );

        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            jsInitialized = false;

            await JSModule.SafeDestroy( ElementRef, ElementId );
        }

        dotNetObjectRef?.Dispose();
        dotNetObjectRef = null;

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

        return await ExecuteAfterRenderAsync( () => JSModule.GetValue( ElementRef, ElementId ).AsTask() );
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

        ExecuteAfterRender( () => JSModule.SetValue( ElementRef, ElementId, value ).AsTask() );
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
    /// <returns>
    /// A task that represents the asynchronous operation. The result is <see langword="true"/> when a formatter
    /// was available; otherwise, <see langword="false"/>.
    /// </returns>
    public async Task<bool> FormatDocumentAsync()
    {
        if ( jsInitialized )
            return await JSModule.FormatDocument( ElementRef, ElementId );

        return await ExecuteAfterRenderAsync( () => JSModule.FormatDocument( ElementRef, ElementId ).AsTask() );
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
    /// Sets the document formatting provider.
    /// </summary>
    /// <param name="formattingProvider">Document formatting provider.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetFormattingProviderAsync( CodeEditorDocumentFormattingProvider formattingProvider )
    {
        FormattingProvider = formattingProvider;

        if ( jsInitialized )
            await JSModule.SetFormattingProvider( ElementRef, ElementId, CreateFormattingProvider( formattingProvider ) );
    }

    /// <summary>
    /// Gets the diagnostic markers for the current editor model.
    /// </summary>
    /// <returns>Diagnostic markers reported for the current editor model.</returns>
    public async Task<IReadOnlyList<CodeEditorDiagnostic>> GetDiagnostics()
    {
        if ( jsInitialized )
            return await JSModule.GetDiagnostics( ElementRef, ElementId );

        return await ExecuteAfterRenderAsync( () => JSModule.GetDiagnostics( ElementRef, ElementId ).AsTask() );
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
        Language = string.IsNullOrWhiteSpace( language ) ? CodeEditorLanguage.PlainText : language;

        if ( jsInitialized )
        {
            await JSModule.SetLanguage( ElementRef, ElementId, Language );
            await JSModule.SetCompletionProvider( ElementRef, ElementId, CreateCompletionProvider() );
            await JSModule.SetFormattingProvider( ElementRef, ElementId, CreateFormattingProvider() );
        }
    }

    /// <summary>
    /// Sets the editor theme.
    /// </summary>
    /// <param name="theme">Theme identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SetThemeAsync( string theme )
    {
        Theme = string.IsNullOrWhiteSpace( theme ) ? CodeEditorTheme.VisualStudio : theme;

        if ( jsInitialized )
            await JSModule.SetTheme( ElementRef, ElementId, Theme );
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

        return await ExecuteAfterRenderAsync( () => JSModule.GetSelection( ElementRef, ElementId ).AsTask() );
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
        return EditorFocus.InvokeAsync();
    }

    /// <summary>
    /// Javascript callback for when editor loses focus.
    /// </summary>
    [JSInvokable]
    public async Task OnEditorBlur()
    {
        await EditorBlur.InvokeAsync();
        await ValidateOnBlurAsync();
    }

    /// <summary>
    /// Formats the specified document value using the configured .NET formatter.
    /// This method should only be called internally.
    /// </summary>
    /// <param name="value">Document value to format.</param>
    /// <returns>Formatted document value.</returns>
    [JSInvokable]
    public Task<string> NotifyDocumentFormatting( string value )
    {
        Func<string, Task<string>> formatter = FormattingProvider?.Formatter;

        return formatter is null
            ? Task.FromResult<string>( null )
            : formatter.Invoke( value ?? string.Empty );
    }

    /// <summary>
    /// Executes given action after the rendering is done.
    /// </summary>
    protected async Task<T> ExecuteAfterRenderAsync<T>( Func<Task<T>> action, CancellationToken token = default )
    {
        TaskCompletionSource<T> source = new( TaskCreationOptions.RunContinuationsAsynchronously );
        using CancellationTokenRegistration registration = token.Register( () => source.TrySetCanceled( token ) );

        ExecuteAfterRender( async () =>
        {
            try
            {
                T result = await action();
                source.TrySetResult( result );
            }
            catch ( OperationCanceledException exception )
            {
                source.TrySetCanceled( exception.CancellationToken );
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
        {
            customLanguages.Add( customLanguage );
        }

        NotifyLanguageChanged();
    }

    internal void NotifyLanguageRemoved( CodeEditorCustomLanguage customLanguage )
    {
        if ( customLanguages.Remove( customLanguage ) )
            NotifyLanguageChanged();
    }

    internal void NotifyLanguageChanged()
    {
        ScheduleLanguagesUpdate();
    }

    private void ScheduleEditorOptionsUpdate()
    {
        if ( !jsInitialized || editorOptionsUpdateScheduled )
        {
            return;
        }

        editorOptionsUpdateScheduled = true;

        ExecuteAfterRender( async () =>
        {
            editorOptionsUpdateScheduled = false;

            if ( jsInitialized )
            {
                await JSModule.UpdateOptions( ElementRef, ElementId, CreateJSOptions() );
            }
        } );
    }

    private void ScheduleLanguagesUpdate()
    {
        if ( !jsInitialized || languagesUpdateScheduled )
        {
            return;
        }

        languagesUpdateScheduled = true;

        ExecuteAfterRender( async () =>
        {
            languagesUpdateScheduled = false;

            if ( jsInitialized )
            {
                await JSModule.SetLanguages( ElementRef, ElementId, CreateLanguageDefinitions() );
            }
        } );
    }

    private void ScheduleCompletionProviderUpdate()
    {
        if ( !jsInitialized || completionProviderUpdateScheduled )
        {
            return;
        }

        completionProviderUpdateScheduled = true;

        ExecuteAfterRender( async () =>
        {
            completionProviderUpdateScheduled = false;

            if ( jsInitialized )
            {
                await JSModule.SetCompletionProvider( ElementRef, ElementId, CreateCompletionProvider() );
            }
        } );
    }

    private void ScheduleFormattingProviderUpdate()
    {
        if ( !jsInitialized || formattingProviderUpdateScheduled )
        {
            return;
        }

        formattingProviderUpdateScheduled = true;

        ExecuteAfterRender( async () =>
        {
            formattingProviderUpdateScheduled = false;

            if ( jsInitialized )
            {
                await JSModule.SetFormattingProvider( ElementRef, ElementId, CreateFormattingProvider() );
            }
        } );
    }

    private CodeEditorJSOptions CreateJSOptions()
    {
        CodeEditorOptions editorOptions = EditorOptions ?? defaultEditorOptions;

        return new CodeEditorJSOptions
        {
            AssetsPath = GlobalOptions.AssetsPath,
            Value = Value ?? string.Empty,
            Language = ResolveLanguage(),
            Theme = ResolveTheme(),
            ReadOnly = ReadOnly,
            Disabled = Disabled,
            TabIndex = Disabled ? -1 : TabIndex,
            AriaInvalid = ResolvedAriaInvalid,
            AriaRequired = ResolvedAriaRequired,
            AriaDescribedBy = ResolvedAriaDescribedBy,
            AriaLabelledBy = ResolvedAriaLabelledBy,
            Immediate = paramImmediate.GetValueOrDefault( Options?.Immediate ?? true ),
            Debounce = paramDebounce.GetValueOrDefault( Options?.Debounce ?? false ),
            DebounceInterval = Math.Max( 0, DebounceInterval.GetValueOrDefault( Options?.DebounceInterval ?? 300 ) ),
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
            AdditionalOptions = editorOptions.AdditionalOptions,
            Languages = CreateLanguageDefinitions(),
            CompletionProvider = CreateCompletionProvider(),
            FormattingProvider = CreateFormattingProvider()
        };
    }

    private IReadOnlyList<CodeEditorLanguageDefinition> CreateLanguageDefinitions( IReadOnlyList<CodeEditorLanguageDefinition> languages = null )
    {
        Dictionary<string, CodeEditorLanguageDefinition> languageDefinitions = new( StringComparer.Ordinal );

        if ( GlobalOptions.Languages is not null )
        {
            AddLanguageDefinitions( languageDefinitions, GlobalOptions.Languages );
        }

        if ( languages is not null )
        {
            AddLanguageDefinitions( languageDefinitions, languages );
        }
        else if ( Languages is not null )
        {
            AddLanguageDefinitions( languageDefinitions, Languages );
        }

        AddLanguageDefinitions( languageDefinitions, customLanguages.Select( customLanguage => customLanguage.ToDefinition() ) );

        return languageDefinitions.Values.ToArray();
    }

    private static void AddLanguageDefinitions( Dictionary<string, CodeEditorLanguageDefinition> target, IEnumerable<CodeEditorLanguageDefinition> languages )
    {
        foreach ( CodeEditorLanguageDefinition language in languages )
        {
            if ( language is not null && !string.IsNullOrWhiteSpace( language.Id ) )
            {
                target[language.Id] = language;
            }
        }
    }

    private CodeEditorCompletionProvider CreateCompletionProvider( CodeEditorCompletionProvider completionProvider = null )
    {
        CodeEditorCompletionProvider provider = completionProvider ?? CompletionProvider;

        if ( provider is not null )
        {
            return new CodeEditorCompletionProvider
            {
                Language = string.IsNullOrWhiteSpace( provider.Language ) ? ResolveLanguage() : provider.Language,
                TriggerCharacters = provider.TriggerCharacters,
                Items = provider.Items,
                ProviderMethod = provider.ProviderMethod
            };
        }

        if ( CompletionItems is null )
            return null;

        return new CodeEditorCompletionProvider
        {
            Language = ResolveLanguage(),
            Items = CompletionItems,
            TriggerCharacters = CompletionTriggerCharacters
        };
    }

    private CodeEditorDocumentFormattingProvider CreateFormattingProvider( CodeEditorDocumentFormattingProvider formattingProvider = null )
    {
        CodeEditorDocumentFormattingProvider provider = formattingProvider ?? FormattingProvider;

        if ( provider is null
             || ( provider.Formatter is null && string.IsNullOrWhiteSpace( provider.ProviderMethod ) ) )
        {
            return null;
        }

        return new CodeEditorDocumentFormattingProvider
        {
            Language = string.IsNullOrWhiteSpace( provider.Language ) ? ResolveLanguage() : provider.Language,
            Formatter = provider.Formatter,
            ProviderMethod = provider.ProviderMethod
        };
    }

    private string ResolveLanguage()
    {
        return string.IsNullOrWhiteSpace( Language )
            ? CodeEditorLanguage.PlainText
            : Language;
    }

    private string ResolveTheme()
    {
        return string.IsNullOrWhiteSpace( Theme )
            ? CodeEditorTheme.VisualStudio
            : Theme;
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
    /// <remarks>
    /// Monaco themes are global. Changing the theme updates every Monaco editor on the page.
    /// </remarks>
    [Parameter] public string Theme { get; set; } = CodeEditorTheme.VisualStudio;

    /// <summary>
    /// Gets or sets additional editor options.
    /// </summary>
    [Parameter] public CodeEditorOptions EditorOptions { get; set; }

    /// <summary>
    /// Gets or sets whether user-originated value updates are sent to .NET while typing.
    /// </summary>
    /// <remarks>
    /// When supplied, this value overrides the global Blazorise immediate option. When disabled, the value is sent on blur.
    /// </remarks>
    [Parameter] public bool Immediate { get; set; }

    /// <summary>
    /// Gets or sets whether user-originated value updates are debounced before being sent to .NET.
    /// </summary>
    /// <remarks>
    /// When supplied, this value overrides the global Blazorise debounce option.
    /// </remarks>
    [Parameter] public bool Debounce { get; set; }

    /// <summary>
    /// Gets or sets the debounce interval in milliseconds.
    /// </summary>
    /// <remarks>
    /// When set, this value overrides the global Blazorise debounce interval.
    /// </remarks>
    [Parameter] public int? DebounceInterval { get; set; }

    /// <summary>
    /// Gets or sets diagnostic markers.
    /// </summary>
    [Parameter] public IReadOnlyList<CodeEditorDiagnostic> Diagnostics { get; set; }

    /// <summary>
    /// Gets or sets custom language definitions.
    /// </summary>
    /// <remarks>
    /// Monaco language registrations are global to the page. Use one definition per language identifier.
    /// </remarks>
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
    /// Gets or sets the document formatting provider.
    /// </summary>
    [Parameter] public CodeEditorDocumentFormattingProvider FormattingProvider { get; set; }

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
    /// Notifies when the editor is initialized.
    /// </summary>
    [Parameter] public EventCallback<CodeEditorReadyEventArgs> Ready { get; set; }

    /// <summary>
    /// Notifies when user-originated editor content changes.
    /// </summary>
    /// <remarks>
    /// Programmatic value updates do not raise this event.
    /// </remarks>
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