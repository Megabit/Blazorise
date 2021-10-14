#region Using directives
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
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
        /// Awaitable module instance.
        /// </summary>
        protected Task<IJSObjectReference> moduleTask;

        #endregion

        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public BaseJSModule( IJSRuntime jsRuntime )
        {
            this.jsRuntime = jsRuntime;
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
                        if ( moduleTask != null )
                        {
                            var moduleInstance = await moduleTask;
                            await moduleInstance.DisposeAsync();

                            moduleTask = null;
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

        private string ModuleFilePath
            => Path.Combine( $"./_content/Blazorise/{ModuleFileName}.js" );

        /// <inheritdoc/>
        public Task<IJSObjectReference> Module
            => moduleTask ??= jsRuntime.InvokeAsync<IJSObjectReference>( "import", ModuleFilePath ).AsTask();

        /// <inheritdoc/>
        public abstract string ModuleFileName { get; }

        #endregion
    }
}
