#region Using directives
using System.Reflection;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Video;

/// <summary>
/// Default implementation of the video JS module.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class JSVideoModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSVideoModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    public virtual async ValueTask Initialize( DotNetObjectReference<Video> dotNetObjectReference, ElementReference elementRef, string elementId, object options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
    }

    public virtual async ValueTask Destroy( ElementReference canvasRef, string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, canvasId );
    }

    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
    }

    public virtual async ValueTask UpdateSource( ElementReference elementRef, string elementId, object source, object protection )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateSource", elementRef, elementId, source, protection );
    }

    public virtual async ValueTask Play( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "play", elementRef, elementId );
    }

    public virtual async ValueTask Pause( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "pause", elementRef, elementId );
    }

    public virtual async ValueTask TogglePlay( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "togglePlay", elementRef, elementId );
    }

    public virtual async ValueTask Stop( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "stop", elementRef, elementId );
    }

    public virtual async ValueTask Restart( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "restart", elementRef, elementId );
    }

    public virtual async ValueTask Rewind( ElementReference elementRef, string elementId, double seekTime )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "rewind", elementRef, elementId, seekTime );
    }

    public virtual async ValueTask Forward( ElementReference elementRef, string elementId, double seekTime )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "forward", elementRef, elementId, seekTime );
    }

    public virtual async ValueTask IncreaseVolume( ElementReference elementRef, string elementId, double step )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "increaseVolume", elementRef, elementId, step );
    }

    public virtual async ValueTask DecreaseVolume( ElementReference elementRef, string elementId, double step )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "decreaseVolume", elementRef, elementId, step );
    }

    public virtual async ValueTask ToggleCaptions( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "toggleCaptions", elementRef, elementId );
    }

    public virtual async ValueTask EnterFullscreen( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "enterFullscreen", elementRef, elementId );
    }

    public virtual async ValueTask ExitFullscreen( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "exitFullscreen", elementRef, elementId );
    }

    public virtual async ValueTask ToggleFullscreen( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "toggleFullscreen", elementRef, elementId );
    }

    public virtual async ValueTask Airplay( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "airplay", elementRef, elementId );
    }

    public virtual async ValueTask ToggleControls( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "toggleControls", elementRef, elementId );
    }

    public virtual async ValueTask ShowTextTrack( ElementReference elementRef, string elementId, int textTrackId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "showTextTrack", elementRef, elementId, textTrackId );
    }

    public virtual async ValueTask HideTextTrack( ElementReference elementRef, string elementId, int textTrackId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "hideTextTrack", elementRef, elementId, textTrackId );
    }

    public virtual async ValueTask AddTextTrack( ElementReference elementRef, string elementId, VideoTrack track )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addTextTrack", elementRef, elementId, track );
    }

    public virtual async ValueTask ClearTextTracks( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "clearTextTracks", elementRef, elementId );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Video/video.js?v={VersionProvider.Version}";

    #endregion
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member