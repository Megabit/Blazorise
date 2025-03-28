#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Default implementation of the video JS module.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class JSPdfViewerModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSPdfViewerModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    public virtual async ValueTask Initialize( DotNetObjectReference<PdfViewer> dotNetObjectReference, ElementReference elementRef, string elementId, PdfViewerJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
    }

    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
    }

    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, PdfViewerUpdateJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
    }

    public virtual async ValueTask PreviousPage( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "prevPage", elementRef, elementId );
    }

    public virtual async ValueTask NextPage( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "nextPage", elementRef, elementId );
    }

    public virtual async ValueTask GoToPage( ElementReference elementRef, string elementId, int pageNumber )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "goToPage", elementRef, elementId, pageNumber );
    }

    public virtual async ValueTask SetScale( ElementReference elementRef, string elementId, double scale )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setScale", elementRef, elementId, scale );
    }

    public virtual async ValueTask Print( string source )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "print", source );
    }

    public virtual async ValueTask Download( string source )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "download", source );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.PdfViewer/pdfviewer.js?v={VersionProvider.Version}";

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member