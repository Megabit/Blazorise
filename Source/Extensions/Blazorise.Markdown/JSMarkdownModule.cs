#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Markdown;

/// <summary>
/// Controls EasyMDE editor instances and synchronizes their values with .NET.
/// </summary>
public class JSMarkdownModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSMarkdownModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods        

    /// <summary>
    /// Creates an EasyMDE editor with the supplied toolbar and input settings.
    /// </summary>
    public async ValueTask Initialize( DotNetObjectReference<Markdown> dotNetObjectRef, ElementReference elementRef, string elementId, MarkdownJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );
    }

    /// <summary>
    /// Releases the editor instance attached to an element.
    /// </summary>
    public async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <summary>
    /// Replaces the complete Markdown source in an editor.
    /// </summary>
    public async ValueTask SetValue( string elementId, string value )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setValue", elementId, value );
    }

    /// <summary>
    /// Reads the current Markdown source from an editor.
    /// </summary>
    public async ValueTask<string> GetValue( string elementId )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<string>( "getValue", elementId );
    }

    /// <summary>
    /// Inserts text at the current cursor or selection.
    /// </summary>
    public async ValueTask InsertText( string elementId, string text )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "insertText", elementId, text );
    }

    /// <summary>
    /// Removes the character or selection immediately before the cursor.
    /// </summary>
    public async ValueTask Backspace( string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "backspace", elementId );
    }

    /// <summary>
    /// Completes an image upload and inserts its resulting URL.
    /// </summary>
    public async ValueTask NotifyImageUploadSuccess( string elementId, string imageUrl )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "notifyImageUploadSuccess", elementId, imageUrl );
    }

    /// <summary>
    /// Reports an image upload failure to the editor UI.
    /// </summary>
    public async ValueTask NotifyImageUploadError( string elementId, string errorMessage )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "notifyImageUploadError", elementId, errorMessage );
    }

    /// <summary>
    /// Moves keyboard focus into the editor and optionally reveals it.
    /// </summary>
    public async ValueTask Focus( string elementId, bool scrollToElement )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "focus", elementId, scrollToElement );
    }

    /// <summary>
    /// Applies changed accessibility and input-state settings.
    /// </summary>
    public async ValueTask UpdateBaseInputOptions( string elementId, MarkdownBaseInputJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateBaseInputOptions", elementId, options );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Markdown/markdown.js?v={VersionProvider.Version}";

    #endregion
}