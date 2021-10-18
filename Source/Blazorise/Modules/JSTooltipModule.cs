#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the tooltip JS module.
    /// </summary>
    public abstract class JSTooltipModule : BaseJSModule, IJSTooltipModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public JSTooltipModule( IJSRuntime jsRuntime )
            : base( jsRuntime )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask Initialize( ElementReference elementRef, string elementId, object options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", elementRef, elementId, options );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
        {
            if ( moduleTask != null )
            {
                var moduleInstance = await moduleTask;

                await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateContent( ElementReference elementRef, string elementId, string content )
        {
            if ( moduleTask != null )
            {
                var moduleInstance = await moduleTask;

                await moduleInstance.InvokeVoidAsync( "updateContent", elementId );
            }
        }

        #endregion
    }
}
