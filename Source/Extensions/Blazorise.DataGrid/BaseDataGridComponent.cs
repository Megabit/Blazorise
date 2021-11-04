#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.DataGrid
{
    /// <summary>
    /// Minimal base class for datagrid components.
    /// </summary>
    public class BaseDataGridComponent : BaseAfterRenderComponent, IAsyncDisposable
    {
        #region Methods

        protected override void OnInitialized()
        {
            if ( JSModule == null )
            {
                JSModule = new JSDataGridModule( JSRuntime, VersionProvider );
            }

            base.OnInitialized();

            ElementId ??= IdGenerator.Generate;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                DisposeResources();
            }

            base.Dispose( disposing );
        }

        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                await DisposeResourcesAsync();
            }

            await base.DisposeAsync( disposing );
        }

        protected virtual void DisposeResources()
        {
        }

        protected virtual async Task DisposeResourcesAsync()
        {
            // Blazor will always run async disposal if it is defined so we need to also make sure
            // any synchronous disposables are handled.
            DisposeResources();

            await JSModule.SafeDisposeAsync();
        }

        #endregion

        #region Properties

        protected JSDataGridModule JSModule { get; private set; }

        /// <summary>
        /// Gets or sets the JS runtime.
        /// </summary>
        [Inject] private IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the version provider.
        /// </summary>
        [Inject] private IVersionProvider VersionProvider { get; set; }

        /// <summary>
        /// Gets or sets the classname provider.
        /// </summary>
        [Inject] protected IClassProvider ClassProvider { get; set; }

        /// <summary>
        /// Gets or set the IIdGenerator.
        /// </summary>
        [Inject] protected IIdGenerator IdGenerator { get; set; }

        /// <summary>
        /// Gets or sets the datagrid element id.
        /// </summary>
        [Parameter] public string ElementId { get; set; }

        #endregion
    }
}