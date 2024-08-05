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

    public virtual async ValueTask Initialize( DotNetObjectReference<PdfViewer> dotNetObjectReference, ElementReference elementRef, string elementId, object options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
    }

    public virtual async ValueTask Destroy( ElementReference canvasRef, string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, canvasId );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.PdfViewer/pdfviewer.js?v={VersionProvider.Version}";

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member