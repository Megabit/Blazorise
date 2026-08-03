#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.CodeEditor;

/// <summary>
/// Default implementation of the code editor JS module.
/// </summary>
public class JSCodeEditorModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSCodeEditorModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    public ValueTask Initialize( DotNetObjectReference<CodeEditor> dotNetObjectRef, ElementReference elementRef, string elementId, CodeEditorJSOptions options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );

    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    public ValueTask UpdateOptions( ElementReference elementRef, string elementId, CodeEditorJSOptions options )
        => InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );

    public ValueTask SetDiagnostics( ElementReference elementRef, string elementId, IReadOnlyList<CodeEditorDiagnostic> diagnostics )
        => InvokeSafeVoidAsync( "setDiagnostics", elementRef, elementId, diagnostics );

    public ValueTask<IReadOnlyList<CodeEditorDiagnostic>> GetDiagnostics( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<IReadOnlyList<CodeEditorDiagnostic>>( "getDiagnostics", elementRef, elementId );

    public ValueTask SetLanguages( ElementReference elementRef, string elementId, IReadOnlyList<CodeEditorLanguageDefinition> languages )
        => InvokeSafeVoidAsync( "setLanguages", elementRef, elementId, languages );

    public ValueTask SetCompletionProvider( ElementReference elementRef, string elementId, CodeEditorCompletionProvider completionProvider )
        => InvokeSafeVoidAsync( "setCompletionProvider", elementRef, elementId, completionProvider );

    public ValueTask SetFormattingProvider( ElementReference elementRef, string elementId, CodeEditorDocumentFormattingProvider formattingProvider )
        => InvokeSafeVoidAsync( "setFormattingProvider", elementRef, elementId, formattingProvider );

    public ValueTask SetValue( ElementReference elementRef, string elementId, string value )
        => InvokeSafeVoidAsync( "setValue", elementRef, elementId, value );

    public ValueTask<string> GetValue( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<string>( "getValue", elementRef, elementId );

    public ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        => InvokeSafeVoidAsync( "focus", elementRef, elementId, scrollToElement );

    public ValueTask Resize( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "resize", elementRef, elementId );

    public ValueTask<bool> FormatDocument( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<bool>( "formatDocument", elementRef, elementId );

    public ValueTask RevealLine( ElementReference elementRef, string elementId, int lineNumber )
        => InvokeSafeVoidAsync( "revealLine", elementRef, elementId, lineNumber );

    public ValueTask SetLanguage( ElementReference elementRef, string elementId, string language )
        => InvokeSafeVoidAsync( "setLanguage", elementRef, elementId, language );

    public ValueTask SetTheme( ElementReference elementRef, string elementId, string theme )
        => InvokeSafeVoidAsync( "setTheme", elementRef, elementId, theme );

    public ValueTask SetSelection( ElementReference elementRef, string elementId, CodeEditorSelection selection )
        => InvokeSafeVoidAsync( "setSelection", elementRef, elementId, selection );

    public ValueTask<CodeEditorSelection> GetSelection( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<CodeEditorSelection>( "getSelection", elementRef, elementId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.CodeEditor/codeeditor.js?v={VersionProvider.Version}";

    #endregion
}