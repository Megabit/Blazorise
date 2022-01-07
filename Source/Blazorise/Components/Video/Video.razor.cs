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
                    ManifestUri = ManifestUri
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

        [Parameter] public bool Controls { get; set; } = true;

        [Parameter] public bool AutoPlay { get; set; }

        [Parameter] public string ManifestUri { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Video"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
