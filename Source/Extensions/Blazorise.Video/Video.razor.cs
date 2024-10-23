#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Video.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Video;

/// <summary>
/// The video component embeds a media player which supports video playback into the document.
/// </summary>
public partial class Video : BaseComponent, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var sourceChanged = parameters.TryGetValue<VideoSource>( nameof( Source ), out var paramSource ) && !Source.Equals( paramSource );

            var protectionTypeChanged = parameters.TryGetValue<VideoProtectionType>( nameof( ProtectionType ), out var paramProtectionType ) && !ProtectionType.IsEqual( paramProtectionType );
            var protectionDataChanged = parameters.TryGetValue<object>( nameof( ProtectionData ), out var paramProtectionData ) && !ProtectionData.IsEqual( paramProtectionData );
            var protectionServerUrlChanged = parameters.TryGetValue<string>( nameof( ProtectionServerUrl ), out var paramProtectionServerUrl ) && !ProtectionServerUrl.IsEqual( paramProtectionServerUrl );
            var protectionServerCertificateUrlChanged = parameters.TryGetValue<string>( nameof( ProtectionServerCertificateUrl ), out var paramProtectionServerCertificateUrl ) && !ProtectionServerCertificateUrl.IsEqual( paramProtectionServerCertificateUrl );
            var protectionHttpRequestHeadersChanged = parameters.TryGetValue<string>( nameof( ProtectionHttpRequestHeaders ), out var paramProtectionHttpRequestHeaders ) && !ProtectionHttpRequestHeaders.IsEqual( paramProtectionHttpRequestHeaders );

            var currentTimeChanged = parameters.TryGetValue<double>( nameof( CurrentTime ), out var paramCurrentTime ) && !CurrentTime.IsEqual( paramCurrentTime );
            var volumeChanged = parameters.TryGetValue<double>( nameof( Volume ), out var paramVolume ) && !Volume.IsEqual( paramVolume );

            if ( sourceChanged || currentTimeChanged || volumeChanged )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
                {
                    Source = new { Changed = sourceChanged, Value = paramSource },
                    ProtectionType = new { Changed = protectionTypeChanged, Value = paramProtectionType },
                    ProtectionData = new { Changed = protectionDataChanged, Value = paramProtectionData },
                    ProtectionServerUrl = new { Changed = protectionServerUrlChanged, Value = paramProtectionServerUrl },
                    ProtectionServerCertificateUrl = new { Changed = protectionServerCertificateUrlChanged, Value = paramProtectionServerCertificateUrl },
                    ProtectionHttpRequestHeaders = new { Changed = protectionHttpRequestHeadersChanged, Value = paramProtectionHttpRequestHeaders },
                    CurrentTime = new { Changed = currentTimeChanged, Value = paramCurrentTime },
                    Volume = new { Changed = volumeChanged, Value = paramVolume },
                } ) );
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( JSModule == null )
        {
            DotNetObjectRef ??= DotNetObjectReference.Create( this );

            JSModule = new JSVideoModule( JSRuntime, VersionProvider, BlazoriseOptions );
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new VideoJSOptions
            {
                Controls = Controls,
                ControlsDelay = ControlsDelay,
                ControlsList = ControlsList,
                SettingsList = SettingsList,
                AutomaticallyHideControls = AutomaticallyHideControls,
                AutoPlay = AutoPlay,
                AutoPause = AutoPause,
                Muted = Muted,
                Source = Source,
                Poster = Poster,
                Thumbnails = Thumbnails,
                StreamingLibrary = StreamingLibrary.ToStreamingLibrary(),
                SeekTime = SeekTime,
                CurrentTime = CurrentTime,
                Volume = Volume,
                ClickToPlay = ClickToPlay,
                DisableContextMenu = DisableContextMenu,
                ResetOnEnd = ResetOnEnd,
                AspectRatio = VideoParsers.ParseAspectRatio( Ratio ),
                InvertTime = InvertTime,
                DefaultQuality = new VideoJSQualityOptions( DefaultQuality ),
                AvailableQualities = AvailableQualities?.Select( x => new VideoJSQualityOptions( x ) )?.ToArray(),
                Protection = ProtectionType != VideoProtectionType.None ? new VideoJSProtectionOptions
                (
                    data: ProtectionData,
                    type: ProtectionType.ToVideoProtectionType(),
                    serverUrl: ProtectionServerUrl,
                    serverCertificateUrl: ProtectionServerCertificateUrl,
                    httpRequestHeaders: ProtectionHttpRequestHeaders
                ) : null,
                DoubleClickToFullscreen = DoubleClickToFullscreen,
            } );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( DotNetObjectRef != null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Updates the media source.
    /// </summary>
    /// <param name="source">New media source.</param>
    /// <param name="protectionType"></param>
    /// <param name="protectionData"></param>
    /// <param name="protectionServerUrl"></param>
    /// <param name="protectionHttpRequestHeaders"></param>
    /// <param name="protectionServerCertificateUrl"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateSource( VideoSource source, VideoProtectionType protectionType = VideoProtectionType.None, object protectionData = null, string protectionServerUrl = null, string protectionHttpRequestHeaders = null, string protectionServerCertificateUrl = null )
    {
        if ( Rendered )
        {
            Source = source;
            ProtectionData = protectionData;
            ProtectionType = protectionType;
            ProtectionServerUrl = protectionServerUrl;
            ProtectionServerCertificateUrl = protectionServerCertificateUrl;
            ProtectionHttpRequestHeaders = protectionHttpRequestHeaders;

            await JSModule.UpdateSource( ElementRef, ElementId, source: Source, protection: ProtectionType != VideoProtectionType.None ? new
            {
                Data = protectionData,
                Type = ProtectionType.ToVideoProtectionType(),
                ServerUrl = ProtectionServerUrl,
                ServerCertificateUrl = ProtectionServerCertificateUrl,
                HttpRequestHeaders = ProtectionHttpRequestHeaders
            } : null );
        }
    }

    /// <summary>
    /// Start playback.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Play() => JSModule.Play( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Pause playback.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Pause() => JSModule.Pause( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Toggle playback, if no parameters are passed, it will toggle based on current status.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task TogglePlay() => JSModule.TogglePlay( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Stop playback and reset to start.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Stop() => JSModule.Stop( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Restart playback.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Restart() => JSModule.Restart( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Rewind playback by the specified seek time.
    /// </summary>
    /// <param name="seekTime"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Rewind( double seekTime = 10 ) => JSModule.Rewind( ElementRef, ElementId, seekTime ).AsTask();

    /// <summary>
    /// Fast forward by the specified seek time. If no parameter is passed, the default seek time will be used.
    /// </summary>
    /// <param name="seekTime"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Forward( double seekTime = 10 ) => JSModule.Forward( ElementRef, ElementId, seekTime ).AsTask();

    /// <summary>
    /// Increase volume by the specified step. If no parameter is passed, the default step will be used.
    /// </summary>
    /// <param name="step">Volume step to increase.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task IncreaseVolume( double step = 0.1 ) => JSModule.IncreaseVolume( ElementRef, ElementId, step ).AsTask();

    /// <summary>
    /// Decrease volume by the specified step. If no parameter is passed, the default step will be used.
    /// </summary>
    /// <param name="step">Volume step to decrease.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task DecreaseVolume( double step = 0.1 ) => JSModule.DecreaseVolume( ElementRef, ElementId, step ).AsTask();

    /// <summary>
    /// Toggle captions display.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ToggleCaptions() => JSModule.ToggleCaptions( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Enter fullscreen. If fullscreen is not supported, a fallback "full window/viewport" is used instead.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task EnterFullscreen() => JSModule.EnterFullscreen( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Exit fullscreen.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ExitFullscreen() => JSModule.ExitFullscreen( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Toggle fullscreen.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ToggleFullscreen() => JSModule.ToggleFullscreen( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Trigger the airplay dialog on supported devices.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task Airplay() => JSModule.Airplay( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Toggle the controls (video only). Takes optional truthy value to force it on/off.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ToggleControls() => JSModule.ToggleControls( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Show the track from the list of tracks.
    /// </summary>
    /// <param name="textTrackId">Index of the track to be shown.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ShowTextTrack( int textTrackId ) => JSModule.ShowTextTrack( ElementRef, ElementId, textTrackId ).AsTask();

    /// <summary>
    /// Hide the track from the list of tracks.
    /// </summary>
    /// <param name="textTrackId">Index of the track to be hidden.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task HideTextTrack( int textTrackId ) => JSModule.HideTextTrack( ElementRef, ElementId, textTrackId ).AsTask();

    /// <summary>
    /// Adds a new text track to the list of tracks.
    /// </summary>
    /// <param name="track">Track to be added to the list of tracks.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AddTextTrack( VideoTrack track ) => JSModule.AddTextTrack( ElementRef, ElementId, track ).AsTask();

    /// <summary>
    /// Clear all the text tracks.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ClearTextTracks() => JSModule.ClearTextTracks( ElementRef, ElementId ).AsTask();

    /// <summary>
    /// Dispatch an event to change the media playback rate.
    /// </summary>
    /// <param name="playbackRate">A double representing the new playback rate. A value of 1.0 represents normal speed, 0.5 is half speed, and 2.0 is double speed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task SetPlaybackRate( double playbackRate ) => JSModule.SetPlaybackRate( ElementRef, ElementId, playbackRate ).AsTask();

    #region Events

    /// <summary>
    /// Notifies the video component of the media progress so far. Should not be called directly by the user!
    /// </summary>
    /// <param name="buffered">How much of media is buffered so far.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyProgress( double buffered )
    {
        if ( Progress != null )
            return Progress.Invoke( buffered );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media is playing. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyPlaying()
    {
        if ( Playing != null )
            return Playing.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media is started to play. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyPlay()
    {
        if ( Played != null )
            return Played.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media is paused. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyPause()
    {
        if ( Paused != null )
            return Paused.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component how much time has passed since the media started playing. Should not be called directly by the user!
    /// </summary>
    /// <param name="currentTime">Current time in seconds.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyTimeUpdate( double currentTime )
    {
        if ( TimeUpdate != null )
            return TimeUpdate.Invoke( currentTime );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the volume has changed. Should not be called directly by the user!
    /// </summary>
    /// <param name="volume">Volume value.</param>
    /// <param name="muted">True if the media is muted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyVolumeChange( double volume, bool muted )
    {
        if ( VolumeChange != null )
            return VolumeChange.Invoke( volume, muted );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media is in seeking mode. Should not be called directly by the user!
    /// </summary>
    /// <param name="currentTime">Current time in seconds.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifySeeking( double currentTime )
    {
        if ( Seeking != null )
            return Seeking.Invoke( currentTime );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media has ended seeking. Should not be called directly by the user!
    /// </summary>
    /// <param name="currentTime">Current time in seconds.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifySeeked( double currentTime )
    {
        if ( Seeked != null )
            return Seeked.Invoke( currentTime );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media speed rate has changed. Should not be called directly by the user!
    /// </summary>
    /// <param name="speed">Media speed rate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyRateChange( double speed )
    {
        if ( RateChange != null )
            return RateChange.Invoke( speed );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the media has ended. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyEnded()
    {
        if ( Ended != null )
            return Ended.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the video is entered the fullscreen mode. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyFullScreenEntered()
    {
        if ( FullScreenEntered != null )
            return FullScreenEntered.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the video is exited from fullscreen mode. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyFullScreenExited()
    {
        if ( FullScreenExited != null )
            return FullScreenExited.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the video has enabled captions. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyCaptionsEnabled()
    {
        if ( CaptionsEnabled != null )
            return CaptionsEnabled.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the video has disabled captions. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyCaptionsDisabled()
    {
        if ( CaptionsDisabled != null )
            return CaptionsDisabled.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the language has changed. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyLanguageChange( string language )
    {
        if ( LanguageChange != null )
            return LanguageChange.Invoke( language );

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the controls are now hidded. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyControlsHidden()
    {
        if ( ControlsHidden != null )
            return ControlsHidden.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the controls are now visible. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyControlsShown()
    {
        if ( ControlsShown != null )
            return ControlsShown.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the player is now ready. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyReady()
    {
        if ( Ready != null )
            return Ready.Invoke();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Notifies the video component that the quality has changed. Should not be called directly by the user!
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyQualityChange( int? quality )
    {
        if ( QualityChanged != null )
            return QualityChanged.Invoke( quality );

        return Task.CompletedTask;
    }

    #endregion

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Reference to the object that should be accessed through JSInterop.
    /// </summary>
    protected DotNetObjectReference<Video> DotNetObjectRef { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="JSVideoModule"/> instance.
    /// </summary>
    protected JSVideoModule JSModule { get; private set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Gets or sets the controls visibility of the player.
    /// </summary>
    [Parameter] public bool Controls { get; set; } = true;

    /// <summary>
    /// The default amount of delay in milliseconds while media playback is progressing without user activity to indicate an idle state and hide controls.
    /// </summary>
    [Parameter] public double ControlsDelay { get; set; } = 2000;

    /// <summary>
    /// Hide video controls automatically after 2s of no mouse or focus movement, on control element blur (tab out), on playback
    /// start or entering fullscreen. As soon as the mouse is moved, a control element is focused or playback is paused, the
    /// controls reappear instantly.
    /// </summary>
    [Parameter] public bool AutomaticallyHideControls { get; set; }

    /// <summary>
    /// Gets or sets the autoplay state of the player.
    /// </summary>
    [Parameter] public bool AutoPlay { get; set; }

    /// <summary>
    /// Only allow one player playing at once.
    /// </summary>
    [Parameter] public bool AutoPause { get; set; } = true;

    /// <summary>
    /// Whether to start playback muted.
    /// </summary>
    [Parameter] public bool Muted { get; set; }

    /// <summary>
    /// Gets or sets the current source for the player.
    /// </summary>
    [Parameter] public VideoSource Source { get; set; }

    /// <summary>
    /// Gets or sets the the URL of media poster or thumbnail image, generally before playback begins.
    /// </summary>
    [Parameter] public string Poster { get; set; }

    /// <summary>
    /// Gets or sets the URL of thumbnails which will be used to display preview images when interacting with the time slider and in the chapters menu.
    /// </summary>
    [Parameter] public string Thumbnails { get; set; }

    /// <summary>
    /// If defined the video will run in streaming mode.
    /// </summary>
    [Parameter] public StreamingLibrary StreamingLibrary { get; set; } = StreamingLibrary.None;

    /// <summary>
    /// The time, in seconds, to seek when a user hits fast forward or rewind.
    /// </summary>
    [Parameter] public int SeekTime { get; set; } = 10;

    /// <summary>
    /// Gets or sets the currentTime for the player.
    /// </summary>
    [Parameter] public double CurrentTime { get; set; }

    /// <summary>
    /// A number, between 0 and 1, representing the initial volume of the player.
    /// </summary>
    [Parameter] public double Volume { get; set; } = 1;

    /// <summary>
    /// Click (or tap) of the video container will toggle play/pause.
    /// </summary>
    [Parameter] public bool ClickToPlay { get; set; } = true;

    /// <summary>
    /// Disable right click menu on video to help as very primitive obfuscation to prevent downloads of content.
    /// </summary>
    [Parameter] public bool DisableContextMenu { get; set; } = true;

    /// <summary>
    /// Reset the playback to the start once playback is complete.
    /// </summary>
    [Parameter] public bool ResetOnEnd { get; set; }

    /// <summary>
    /// Force an aspect ratio for all videos. The format is 'w:h' - e.g. '16:9' or '4:3'. If this is not specified
    /// then the default for HTML5 and Vimeo is to use the native resolution of the video. As dimensions are not
    /// available from YouTube via SDK, 16:9 is forced as a sensible default.
    /// </summary>
    [Parameter] public string Ratio { get; set; }

    /// <summary>
    /// Display the current time as a countdown rather than an incremental counter.
    /// </summary>
    [Parameter] public bool InvertTime { get; set; } = true;

    /// <summary>
    /// Defines the encoding type used for the DRM protection.
    /// </summary>
    [Parameter] public VideoProtectionType ProtectionType { get; set; }

    /// <summary>
    /// Defines the manual structure of the protection data. If defined, it will override the usage of <see cref="ProtectionServerUrl"/> and <see cref="ProtectionHttpRequestHeaders"/>.
    /// </summary>
    [Parameter] public object ProtectionData { get; set; }

    /// <summary>
    /// Defines the server url of the DRM protection.
    /// </summary>
    [Parameter] public string ProtectionServerUrl { get; set; }

    /// <summary>
    /// Defines the server certificate url of the DRM protection (currently used only with FairPlay).
    /// </summary>
    [Parameter] public string ProtectionServerCertificateUrl { get; set; }

    /// <summary>
    /// Defines the protection token for the http header that is sent to the server.
    /// </summary>
    [Parameter] public string ProtectionHttpRequestHeaders { get; set; }

    /// <summary>
    /// Defines the customized list of player controls. 
    /// </summary>
    [Parameter] public string[] ControlsList { get; set; } = new string[] { VideoControlsType.PlayLarge, VideoControlsType.Play, VideoControlsType.Progress, VideoControlsType.CurrentTime, VideoControlsType.Mute, VideoControlsType.Volume, VideoControlsType.Captions, VideoControlsType.Settings, VideoControlsType.Pip, VideoControlsType.Airplay, VideoControlsType.Fullscreen };

    /// <summary>
    /// If the default controls are used, you can specify which settings to show in the menu.
    /// </summary>
    [Parameter] public VideoSettingsType[] SettingsList { get; set; } = new VideoSettingsType[] { VideoSettingsType.Captions, VideoSettingsType.Quality, VideoSettingsType.Speed, VideoSettingsType.Loop };

    /// <summary>
    /// Sent periodically to inform interested parties of progress downloading the media. Information about the current amount of the media that has been downloaded is available in the media element's buffered attribute.
    /// </summary>
    [Parameter] public Func<double, Task> Progress { get; set; }

    /// <summary>
    /// Gets or sets the default quality for the player.
    /// </summary>
    [Parameter] public int? DefaultQuality { get; set; }

    /// <summary>
    /// Defines the list of available quality options. Defaults to [4320, 2880, 2160, 1440, 1080, 720, 576, 480, 360, 240].
    /// </summary>
    [Parameter] public int[] AvailableQualities { get; set; }

    /// <summary>
    /// Sent when the media begins to play (either for the first time, after having been paused, or after ending and then restarting).
    /// </summary>
    [Parameter] public Func<Task> Playing { get; set; }

    /// <summary>
    /// Sent when playback of the media starts after having been paused; that is, when playback is resumed after a prior pause event.
    /// </summary>
    [Parameter] public Func<Task> Played { get; set; }

    /// <summary>
    /// Sent when playback is paused.
    /// </summary>
    [Parameter] public Func<Task> Paused { get; set; }

    /// <summary>
    /// The time indicated by the element's currentTime attribute has changed.
    /// </summary>
    [Parameter] public Func<double, Task> TimeUpdate { get; set; }

    /// <summary>
    /// Sent when the audio volume changes (both when the volume is set and when the muted state is changed).
    /// </summary>
    [Parameter] public Func<double, bool, Task> VolumeChange { get; set; }

    /// <summary>
    /// Sent when a seek operation begins.
    /// </summary>
    [Parameter] public Func<double, Task> Seeking { get; set; }

    /// <summary>
    /// Sent when a seek operation completes.
    /// </summary>
    [Parameter] public Func<double, Task> Seeked { get; set; }

    /// <summary>
    /// Sent when the playback speed changes.
    /// </summary>
    [Parameter] public Func<double, Task> RateChange { get; set; }

    /// <summary>
    /// Sent when playback completes. Note: This does not fire if autoplay is true.
    /// </summary>
    [Parameter] public Func<Task> Ended { get; set; }

    /// <summary>
    /// Sent when the player enters fullscreen mode (either the proper fullscreen or full-window fallback for older browsers).
    /// </summary>
    [Parameter] public Func<Task> FullScreenEntered { get; set; }

    /// <summary>
    /// Sent when the player exits fullscreen mode.
    /// </summary>
    [Parameter] public Func<Task> FullScreenExited { get; set; }

    /// <summary>
    /// Sent when captions are enabled.
    /// </summary>
    [Parameter] public Func<Task> CaptionsEnabled { get; set; }

    /// <summary>
    /// Sent when captions are disabled.
    /// </summary>
    [Parameter] public Func<Task> CaptionsDisabled { get; set; }

    /// <summary>
    /// Sent when the caption language is changed.
    /// </summary>
    [Parameter] public Func<string, Task> LanguageChange { get; set; }

    /// <summary>
    /// Sent when the controls are hidden.
    /// </summary>
    [Parameter] public Func<Task> ControlsHidden { get; set; }

    /// <summary>
    /// Sent when the controls are shown.
    /// </summary>
    [Parameter] public Func<Task> ControlsShown { get; set; }

    /// <summary>
    /// Triggered when the instance is ready for API calls.
    /// </summary>
    [Parameter] public Func<Task> Ready { get; set; }

    /// <summary>
    /// The quality of playback has changed. 
    /// <para>
    /// If the quality argument is <c>null</c> it is considered that a default quality value was selected.
    /// </para>
    /// </summary>
    [Parameter] public Func<int?, Task> QualityChanged { get; set; }

    /// <summary>
    /// If defined the player will go fullscreen when the video is double-clicked.
    /// </summary>
    [Parameter] public bool DoubleClickToFullscreen { get; set; } = true;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Video"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}