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
        /// <param name="versionProvider">Version provider.</param>
        public JSBreakpointModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
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
            if ( IsUnsafe )
                return;

            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "unregisterBreakpointComponent", component.ElementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string> GetBreakpoint()
        {
            if ( IsUnsafe )
                return await Task.FromResult<string>( null );

            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<string>( "getBreakpoint" );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/breakpoint.js?v={VersionProvider.Version}";

        #endregion
    }
}
