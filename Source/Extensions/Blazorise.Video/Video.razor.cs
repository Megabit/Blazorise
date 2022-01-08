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
                JSModule = new JSVideoModule( JSRuntime, VersionProvider );
            }

            return base.OnInitializedAsync();
        }

        /// <inheritdoc/>
        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await JSModule.Initialize( ElementRef, ElementId, new
                {
                    Controls,
                    AutomaticallyHideControls,
                    AutoPlay,
                    AutoPause,
                    Muted,
                    Source,
                    Poster,
                    Streaming,
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
            }

            await base.DisposeAsync( disposing );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

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
        /// If true, the video will be running in streaming mode.
        /// </summary>
        [Parameter] public bool Streaming { get; set; }

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
        /// Specifies the content to be rendered inside this <see cref="Video"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
