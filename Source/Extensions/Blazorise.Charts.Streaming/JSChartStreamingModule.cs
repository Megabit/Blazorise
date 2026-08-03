#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming;

/// <summary>
/// Controls the browser-side Chart.js streaming plugin.
/// </summary>
public class JSChartStreamingModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSChartStreamingModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Enables realtime behavior on a chart canvas.
    /// </summary>
    public virtual ValueTask Initialize( DotNetObjectReference<ChartStreamingAdapter> dotNetObjectReference, ElementReference canvasRef, string canvasId, bool vertical, ChartStreamingOptions options )
        => InvokeSafeVoidAsync( "initialize",
            dotNetObjectReference,
            canvasRef,
            canvasId,
            vertical,
            options );

    /// <summary>
    /// Detaches realtime processing from a chart canvas.
    /// </summary>
    public virtual ValueTask Destroy( ElementReference canvasRef, string canvasId )
        => InvokeSafeVoidAsync( "destroy", canvasRef, canvasId );

    /// <summary>
    /// Appends a value to one dataset in the live chart.
    /// </summary>
    public virtual ValueTask AddData( string canvasId, int datasetIndex, object data )
        => InvokeSafeVoidAsync( "addData", canvasId, datasetIndex, data );

    /// <summary>
    /// Suspends chart scrolling while optionally animating the transition.
    /// </summary>
    public virtual ValueTask Pause( string canvasId, bool animate )
        => InvokeSafeVoidAsync( "pause", canvasId, animate );

    /// <summary>
    /// Resumes chart scrolling while optionally animating the transition.
    /// </summary>
    public virtual ValueTask Play( string canvasId, bool animate )
        => InvokeSafeVoidAsync( "play", canvasId, animate );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Streaming/charts.streaming.js?v={VersionProvider.Version}";

    #endregion
}