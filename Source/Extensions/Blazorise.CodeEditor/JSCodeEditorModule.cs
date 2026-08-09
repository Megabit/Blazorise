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

    /// <summary>Creates a Monaco editor and connects its .NET callbacks.</summary>
    public ValueTask Initialize( DotNetObjectReference<CodeEditor> dotNetObjectRef, ElementReference elementRef, string elementId, CodeEditorJSOptions options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );

    /// <summary>Disposes the Monaco model and editor attached to an element.</summary>
    public ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    /// <summary>Applies changed editor and accessibility settings.</summary>
    public ValueTask UpdateOptions( ElementReference elementRef, string elementId, CodeEditorJSOptions options )
        => InvokeSafeVoidAsync( "updateOptions", elementRef, elementId, options );

    /// <summary>Replaces diagnostic markers displayed in the source model.</summary>
    public ValueTask SetDiagnostics( ElementReference elementRef, string elementId, IReadOnlyList<CodeEditorDiagnostic> diagnostics )
        => InvokeSafeVoidAsync( "setDiagnostics", elementRef, elementId, diagnostics );

    /// <summary>Reads the diagnostic markers currently available for the source model.</summary>
    public ValueTask<IReadOnlyList<CodeEditorDiagnostic>> GetDiagnostics( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<IReadOnlyList<CodeEditorDiagnostic>>( "getDiagnostics", elementRef, elementId );

    /// <summary>Registers or replaces custom Monaco language definitions.</summary>
    public ValueTask SetLanguages( ElementReference elementRef, string elementId, IReadOnlyList<CodeEditorLanguageDefinition> languages )
        => InvokeSafeVoidAsync( "setLanguages", elementRef, elementId, languages );

    /// <summary>Connects a completion provider to the active editor language.</summary>
    public ValueTask SetCompletionProvider( ElementReference elementRef, string elementId, CodeEditorCompletionProvider completionProvider )
        => InvokeSafeVoidAsync( "setCompletionProvider", elementRef, elementId, completionProvider );

    /// <summary>Connects a document formatter to the active editor language.</summary>
    public ValueTask SetFormattingProvider( ElementReference elementRef, string elementId, CodeEditorDocumentFormattingProvider formattingProvider )
        => InvokeSafeVoidAsync( "setFormattingProvider", elementRef, elementId, formattingProvider );

    /// <summary>Replaces the complete source model value.</summary>
    public ValueTask SetValue( ElementReference elementRef, string elementId, string value )
        => InvokeSafeVoidAsync( "setValue", elementRef, elementId, value );

    /// <summary>Reads the complete source model value.</summary>
    public ValueTask<string> GetValue( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<string>( "getValue", elementRef, elementId );

    /// <summary>Moves keyboard focus into the editor and optionally reveals it.</summary>
    public ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement )
        => InvokeSafeVoidAsync( "focus", elementRef, elementId, scrollToElement );

    /// <summary>Recalculates Monaco layout from the current container dimensions.</summary>
    public ValueTask Resize( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "resize", elementRef, elementId );

    /// <summary>Runs the registered document formatter when one is available.</summary>
    public ValueTask<bool> FormatDocument( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<bool>( "formatDocument", elementRef, elementId );

    /// <summary>Scrolls a one-based source line into the viewport.</summary>
    public ValueTask RevealLine( ElementReference elementRef, string elementId, int lineNumber )
        => InvokeSafeVoidAsync( "revealLine", elementRef, elementId, lineNumber );

    /// <summary>Changes tokenization and language services for the source model.</summary>
    public ValueTask SetLanguage( ElementReference elementRef, string elementId, string language )
        => InvokeSafeVoidAsync( "setLanguage", elementRef, elementId, language );

    /// <summary>Switches the global Monaco color theme.</summary>
    public ValueTask SetTheme( ElementReference elementRef, string elementId, string theme )
        => InvokeSafeVoidAsync( "setTheme", elementRef, elementId, theme );

    /// <summary>Moves the editor selection to the specified source range.</summary>
    public ValueTask SetSelection( ElementReference elementRef, string elementId, CodeEditorSelection selection )
        => InvokeSafeVoidAsync( "setSelection", elementRef, elementId, selection );

    /// <summary>Reads the active source selection and cursor range.</summary>
    public ValueTask<CodeEditorSelection> GetSelection( ElementReference elementRef, string elementId )
        => InvokeSafeAsync<CodeEditorSelection>( "getSelection", elementRef, elementId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.CodeEditor/codeeditor.js?v={VersionProvider.Version}";

    #endregion
}