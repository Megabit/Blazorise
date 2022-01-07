#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Video 
    /// </summary>
    public partial class Video : BaseComponent, IAsyncDisposable
    {
        #region Members

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            ExecuteAfterRender( async () =>
            {
                await JSModule.Initialize( ElementRef, ElementId, new
                {
                    Source = Source
                } );
            } );

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                await JSModule.SafeDestroy( ElementRef, ElementId );
            }

            await base.DisposeAsync( disposing );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets or sets the <see cref="IJSVideoModule"/> instance.
        /// </summary>
        [Inject] public IJSVideoModule JSModule { get; set; }

        /// <summary>
        /// Gets or sets the controls visibility of the player.
        /// </summary>
        [Parameter] public bool Controls { get; set; } = true;

        /// <summary>
        /// Gets or sets the autoplay state of the player.
        /// </summary>
        [Parameter] public bool AutoPlay { get; set; }

        /// <summary>
        /// Gets or sets the current source for the player. The setter accepts an object.
        /// </summary>
        [Parameter] public string Source { get; set; }

        /// <summary>
        /// Gets or sets the current poster image for the player. The setter accepts a string; the URL for the updated poster image.
        /// </summary>
        [Parameter] public string Poster { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Video"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
