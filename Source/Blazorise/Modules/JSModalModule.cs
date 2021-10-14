#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default implementation of the modal JS module.
    /// </summary>
    public abstract class JSModalModule : BaseJSModule, IJSModalModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public JSModalModule( IJSRuntime jsRuntime )
            : base( jsRuntime )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask OpenModal( ElementReference elementRef, bool scrollToTop )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "open", elementRef, scrollToTop );
        }

        /// <inheritdoc/>
        public virtual async ValueTask CloseModal( ElementReference elementRef )
        {
            if ( moduleTask != null )
            {
                var moduleInstance = await moduleTask;

                await moduleInstance.InvokeVoidAsync( "close", elementRef );
            }
        }

        #endregion
    }
}
