#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the breakpoint JS module.
    /// </summary>
    public class JSBreakpointModule : BaseJSModule, IJSBreakpointModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public JSBreakpointModule( IJSRuntime jsRuntime )
            : base( jsRuntime )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask RegisterBreakpoint( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "registerBreakpointComponent", dotNetObjectRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask UnregisterBreakpoint( IBreakpointActivator component )
        {
            if ( moduleTask != null )
            {
                var moduleInstance = await moduleTask;

                await moduleInstance.InvokeVoidAsync( "unregisterBreakpointComponent", component.ElementId );
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string> GetBreakpoint()
        {
            if ( moduleTask != null )
            {
                var moduleInstance = await moduleTask;

                return await moduleInstance.InvokeAsync<string>( "getBreakpoint" );
            }

            return await Task.FromResult<string>( null );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => "./_content/Blazorise/breakpoint.js";

        #endregion
    }
}
