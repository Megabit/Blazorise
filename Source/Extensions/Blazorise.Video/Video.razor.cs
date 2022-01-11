#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Video
{
    /// <summary>
    /// Video 
    /// </summary>
    public partial class Video : BaseComponent, IAsyncDisposable
    {
        #region Members



        #endregion

        #region Methods

        protected override Task OnInitializedAsync()
        {
            if ( JSModule == null )
            {
                DotNetObjectRef ??= DotNetObjectReference.Create( this );

                JSModule = new JSVideoModule( JSRuntime, VersionProvider );
            }

            return base.OnInitializedAsync();
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await JSModule.Initialize( DotNetObjectRef, ElementRef, ElementId, new
                {
                    Controls,
                    AutomaticallyHideControls,
                    AutoPlay,
                    AutoPause,
                    Muted,
                    Source,
                    Poster,
                    StreamingLibrary = StreamingLibrary.ToStreamingLibrary(),
                    SeekTime,
                    Volume,
                    ClickToPlay,
                    DisableContextMenu,
                    ResetOnEnd,
                    Ratio,
                    InvertTime,
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

        [JSInvokable]
        public Task NotifyProgress( double buffered )
        {
            Console.WriteLine( "Playinc" );
            if ( Progress != null )
                return Progress.Invoke( buffered );

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyPlaying()
        {
            Console.WriteLine( "Playinc" );
            if ( Playing != null )
                return Playing.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyPlay()
        {
            Console.WriteLine( "Play" );
            if ( Play != null )
                return Play.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyPause()
        {
            Console.WriteLine( "Pause" );
            if ( Pause != null )
                return Pause.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyTimeUpdate( double currentTime )
        {
            Console.WriteLine( "TimeUpdate:" + currentTime );
            if ( TimeUpdate != null )
                return TimeUpdate.Invoke( currentTime );

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyVolumeChange( double volume, bool muted )
        {
            Console.WriteLine( "VolumeChange:" + volume + muted );
            if ( VolumeChange != null )
                return VolumeChange.Invoke( volume, muted );

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifySeeking()
        {
            Console.WriteLine( "NotifySeeking" );
            if ( Seeking != null )
                return Seeking.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifySeeked()
        {
            Console.WriteLine( "NotifySeeked" );
            if ( Seeked != null )
                return Seeked.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyRateChange( double speed )
        {
            Console.WriteLine( "NotifyRateChange: " + speed );
            if ( RateChange != null )
                return RateChange.Invoke( speed );

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyEnded()
        {
            Console.WriteLine( "NotifyEnded" );
            if ( Ended != null )
                return Ended.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyEnterFullScreen()
        {
            Console.WriteLine( "NotifyEnterFullScreen" );
            if ( EnterFullScreen != null )
                return EnterFullScreen.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyExitFullScreen()
        {
            Console.WriteLine( "NotifyExitFullScreen" );
            if ( ExitFullScreen != null )
                return ExitFullScreen.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyCaptionsEnabled()
        {
            Console.WriteLine( "NotifyCaptionsEnabled" );
            if ( CaptionsEnabled != null )
                return CaptionsEnabled.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyCaptionsDisabled()
        {
            Console.WriteLine( "NotifyCaptionsDisabled" );
            if ( CaptionsDisabled != null )
                return CaptionsDisabled.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyLanguageChange( string language )
        {
            Console.WriteLine( "NotifyLanguageChange" );
            if ( LanguageChange != null )
                return LanguageChange.Invoke( language );

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyControlsHidden()
        {
            Console.WriteLine( "NotifyControlsHidden" );
            if ( ControlsHidden != null )
                return ControlsHidden.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyControlsShown()
        {
            Console.WriteLine( "NotifyControlsShown" );
            if ( ControlsShown != null )
                return ControlsShown.Invoke();

            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task NotifyReady()
        {
            Console.WriteLine( "NotifyReady" );
            if ( Ready != null )
                return Ready.Invoke();

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        protected DotNetObjectReference<Video> DotNetObjectRef { get; set; }

        protected JSVideoModule JSModule { get; private set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the controls visibility of the player.
        /// </summary>
        [Parameter] public bool Controls { get; set; } = true;

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
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Gets or sets the current poster image for the player. The setter accepts a string; the URL for the updated poster image.
        /// </summary>
        [Parameter] public string Poster { get; set; }

        /// <summary>
        /// If defined the video will run in streaming mode.
        /// </summary>
        [Parameter] public StreamingLibrary StreamingLibrary { get; set; } = StreamingLibrary.None;

        /// <summary>
        /// The time, in seconds, to seek when a user hits fast forward or rewind.
        /// </summary>
        [Parameter] public int SeekTime { get; set; } = 10;

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
        /// Sent periodically to inform interested parties of progress downloading the media. Information about the current amount of the media that has been downloaded is available in the media element's buffered attribute.
        /// </summary>
        [Parameter] public Func<double, Task> Progress { get; set; }

        /// <summary>
        /// Sent when the media begins to play (either for the first time, after having been paused, or after ending and then restarting).
        /// </summary>
        [Parameter] public Func<Task> Playing { get; set; }

        /// <summary>
        /// Sent when playback of the media starts after having been paused; that is, when playback is resumed after a prior pause event.
        /// </summary>
        [Parameter] public Func<Task> Play { get; set; }

        /// <summary>
        /// Sent when playback is paused.
        /// </summary>
        [Parameter] public Func<Task> Pause { get; set; }

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
        [Parameter] public Func<Task> Seeking { get; set; }

        /// <summary>
        /// Sent when a seek operation completes.
        /// </summary>
        [Parameter] public Func<Task> Seeked { get; set; }

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
        [Parameter] public Func<Task> EnterFullScreen { get; set; }

        /// <summary>
        /// Sent when the player exits fullscreen mode.
        /// </summary>
        [Parameter] public Func<Task> ExitFullScreen { get; set; }

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
        /// Specifies the content to be rendered inside this <see cref="Video"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
