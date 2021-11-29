#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <inheritdoc/>
    public abstract class BaseJSModule : IBaseJSModule, IAsyncDisposable
    {
        #region Members

        /// <summary>
        /// JavaScript runtime instance.
        /// </summary>
        private readonly IJSRuntime jsRuntime;

        /// <summary>
        /// Version provider instance.
        /// </summary>
        private readonly IVersionProvider versionProvider;

        /// <summary>
        /// Awaitable module instance.
        /// </summary>
        private Lazy<Task<IJSObjectReference>> lazyModuleTask;

        #endregion

        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public BaseJSModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        {
            this.jsRuntime = jsRuntime;
            this.versionProvider = versionProvider;

            lazyModuleTask = new Lazy<Task<IJSObjectReference>>( LazyModuleFactory );
        }

        /// <summary>
        /// Handles the loading of the JS module.
        /// </summary>
        /// <returns>An awaitable JS module reference.</returns>
        protected virtual Task<IJSObjectReference> LazyModuleFactory()
        {
            return jsRuntime.InvokeAsync<IJSObjectReference>( "import", ModuleFileName ).AsTask();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsync( true );
        }

        /// <summary>
        /// Releases the unmanaged resources used by the module and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">True if the component is in the disposing process.</param>
        protected virtual async ValueTask DisposeAsync( bool disposing )
        {
            try
            {
                if ( !AsyncDisposed )
                {
                    AsyncDisposed = true;

                    if ( disposing )
                    {
                        if ( lazyModuleTask != null )
                        {
                            var moduleTask = lazyModuleTask.Value;

                            if ( moduleTask != null )
                            {
                                var moduleInstance = await moduleTask;
                                await moduleInstance.SafeDisposeAsync();

                                moduleTask = null;
                            }

                            lazyModuleTask = null;
                        }
                    }
                }
            }
            catch ( Exception exc )
            {
                await Task.FromException( exc );
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the component is already fully disposed (asynchronously).
        /// </summary>
        protected bool AsyncDisposed { get; private set; }

        /// <inheritdoc/>
        public Task<IJSObjectReference> Module => lazyModuleTask.Value;

        /// <inheritdoc/>
        public abstract string ModuleFileName { get; }

        /// <summary>
        /// Gets the JavaScript runtime instance.
        /// </summary>
        public IJSRuntime JSRuntime => jsRuntime;

        /// <summary>
        /// Gets the version provider instance.
        /// </summary>
        protected IVersionProvider VersionProvider => versionProvider;

        #endregion
    }
}
