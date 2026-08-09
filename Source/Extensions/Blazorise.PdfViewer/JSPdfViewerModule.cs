#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Coordinates PDF rendering and document actions in the browser.
/// </summary>
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

    /// <summary>
    /// Creates a browser viewer for the supplied PDF source and settings.
    /// </summary>
    public virtual async ValueTask Initialize( DotNetObjectReference<PdfViewer> dotNetObjectReference, ElementReference elementRef, string elementId, PdfViewerJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
    }

    /// <summary>
    /// Releases the viewer and its document resources.
    /// </summary>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
    }

    /// <summary>
    /// Applies changed viewer settings without recreating the component.
    /// </summary>
    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, PdfViewerUpdateJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
    }

    /// <summary>
    /// Navigates to the page preceding the current one.
    /// </summary>
    public virtual async ValueTask PreviousPage( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "prevPage", elementRef, elementId );
    }

    /// <summary>
    /// Navigates to the page following the current one.
    /// </summary>
    public virtual async ValueTask NextPage( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "nextPage", elementRef, elementId );
    }

    /// <summary>
    /// Displays a specific one-based document page.
    /// </summary>
    public virtual async ValueTask GoToPage( ElementReference elementRef, string elementId, int pageNumber )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "goToPage", elementRef, elementId, pageNumber );
    }

    /// <summary>
    /// Changes the magnification used to render PDF pages.
    /// </summary>
    public virtual async ValueTask SetScale( ElementReference elementRef, string elementId, double scale )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setScale", elementRef, elementId, scale );
    }

    /// <summary>
    /// Opens the browser print flow for the document source.
    /// </summary>
    public virtual async ValueTask Print( ElementReference elementRef, string elementId, string source )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "print", elementRef, elementId, source );
    }

    /// <summary>
    /// Downloads the document source under the requested filename.
    /// </summary>
    public virtual async ValueTask Download( ElementReference elementRef, string elementId, string source, string fileName )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "download", elementRef, elementId, source, fileName );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.PdfViewer/pdfviewer.js?v={VersionProvider.Version}";

    #endregion
}