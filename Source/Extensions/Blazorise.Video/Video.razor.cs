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
                    AutoPlay,
                    Source,
                    Streaming,
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
        /// Gets or sets the autoplay state of the player.
        /// </summary>
        [Parameter] public bool AutoPlay { get; set; }

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
        /// Specifies the content to be rendered inside this <see cref="Video"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
