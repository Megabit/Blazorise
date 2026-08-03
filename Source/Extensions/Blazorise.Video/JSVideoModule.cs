#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Video;

/// <summary>
/// Default implementation of the video JS module.
/// </summary>
public class JSVideoModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSVideoModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates the browser player and connects its event callbacks.
    /// </summary>
    public virtual async ValueTask Initialize( DotNetObjectReference<Video> dotNetObjectReference, ElementReference elementRef, string elementId, VideoJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
    }

    /// <summary>
    /// Disposes the browser player attached to an element.
    /// </summary>
    public virtual async ValueTask Destroy( ElementReference canvasRef, string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, canvasId );
    }

    /// <summary>
    /// Applies changed player settings in place.
    /// </summary>
    public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, VideoUpdateJSOptions options )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
    }

    /// <summary>
    /// Loads new media and its optional protection configuration.
    /// </summary>
    public virtual async ValueTask UpdateSource( ElementReference elementRef, string elementId, object source, object protection )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "updateSource", elementRef, elementId, source, protection );
    }

    /// <summary>
    /// Starts or resumes media playback.
    /// </summary>
    public virtual async ValueTask Play( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "play", elementRef, elementId );
    }

    /// <summary>
    /// Pauses playback at the current position.
    /// </summary>
    public virtual async ValueTask Pause( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "pause", elementRef, elementId );
    }

    /// <summary>
    /// Switches between playing and paused states.
    /// </summary>
    public virtual async ValueTask TogglePlay( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "togglePlay", elementRef, elementId );
    }

    /// <summary>
    /// Stops playback and returns to the starting position.
    /// </summary>
    public virtual async ValueTask Stop( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "stop", elementRef, elementId );
    }

    /// <summary>
    /// Restarts playback from the beginning.
    /// </summary>
    public virtual async ValueTask Restart( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "restart", elementRef, elementId );
    }

    /// <summary>
    /// Moves playback backward by the requested interval.
    /// </summary>
    public virtual async ValueTask Rewind( ElementReference elementRef, string elementId, double seekTime )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "rewind", elementRef, elementId, seekTime );
    }

    /// <summary>
    /// Moves playback forward by the requested interval.
    /// </summary>
    public virtual async ValueTask Forward( ElementReference elementRef, string elementId, double seekTime )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "forward", elementRef, elementId, seekTime );
    }

    /// <summary>
    /// Raises audio volume by a relative step.
    /// </summary>
    public virtual async ValueTask IncreaseVolume( ElementReference elementRef, string elementId, double step )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "increaseVolume", elementRef, elementId, step );
    }

    /// <summary>
    /// Lowers audio volume by a relative step.
    /// </summary>
    public virtual async ValueTask DecreaseVolume( ElementReference elementRef, string elementId, double step )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "decreaseVolume", elementRef, elementId, step );
    }

    /// <summary>
    /// Switches caption display on or off.
    /// </summary>
    public virtual async ValueTask ToggleCaptions( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "toggleCaptions", elementRef, elementId );
    }

    /// <summary>
    /// Expands the player into fullscreen mode.
    /// </summary>
    public virtual async ValueTask EnterFullscreen( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "enterFullscreen", elementRef, elementId );
    }

    /// <summary>
    /// Returns the player from fullscreen mode.
    /// </summary>
    public virtual async ValueTask ExitFullscreen( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "exitFullscreen", elementRef, elementId );
    }

    /// <summary>
    /// Switches the player between inline and fullscreen display.
    /// </summary>
    public virtual async ValueTask ToggleFullscreen( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "toggleFullscreen", elementRef, elementId );
    }

    /// <summary>
    /// Opens the AirPlay device picker when the platform supports it.
    /// </summary>
    public virtual async ValueTask Airplay( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "airplay", elementRef, elementId );
    }

    /// <summary>
    /// Shows or hides the player's interactive controls.
    /// </summary>
    public virtual async ValueTask ToggleControls( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "toggleControls", elementRef, elementId );
    }

    /// <summary>
    /// Activates one caption or subtitle track.
    /// </summary>
    public virtual async ValueTask ShowTextTrack( ElementReference elementRef, string elementId, int textTrackId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "showTextTrack", elementRef, elementId, textTrackId );
    }

    /// <summary>
    /// Deactivates one caption or subtitle track.
    /// </summary>
    public virtual async ValueTask HideTextTrack( ElementReference elementRef, string elementId, int textTrackId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "hideTextTrack", elementRef, elementId, textTrackId );
    }

    /// <summary>
    /// Registers an additional timed-text track with the player.
    /// </summary>
    public virtual async ValueTask AddTextTrack( ElementReference elementRef, string elementId, VideoTrack track )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addTextTrack", elementRef, elementId, track );
    }

    /// <summary>
    /// Removes all dynamically registered timed-text tracks.
    /// </summary>
    public virtual async ValueTask ClearTextTracks( ElementReference elementRef, string elementId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "clearTextTracks", elementRef, elementId );
    }

    /// <summary>
    /// Changes the speed at which media is played.
    /// </summary>
    public virtual async ValueTask SetPlaybackRate( ElementReference elementRef, string elementId, double playbackRate )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setPlaybackRate", elementRef, elementId, playbackRate );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Video/video.js?v={VersionProvider.Version}";

    #endregion
}