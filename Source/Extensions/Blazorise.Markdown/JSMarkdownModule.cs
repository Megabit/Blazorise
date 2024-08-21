#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Markdown;

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

    public async ValueTask Initialize( DotNetObjectReference<Markdown> dotNetObjectRef, ElementReference elementRef, string elementId, object options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );
    }

    public async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
    }

    public async ValueTask SetValue( string elementId, string value )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setValue", elementId, value );
    }

    public async ValueTask<string> GetValue( string elementId )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<string>( "getValue", elementId );
    }

    public async ValueTask NotifyImageUploadSuccess( string elementId, string imageUrl )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "notifyImageUploadSuccess", elementId, imageUrl );
    }

    public async ValueTask NotifyImageUploadError( string elementId, string errorMessage )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "notifyImageUploadError", elementId, errorMessage );
    }

    public async ValueTask Focus( string elementId, bool scrollToElement )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "focus", elementId, scrollToElement );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Markdown/markdown.js?v={VersionProvider.Version}";

    #endregion
}