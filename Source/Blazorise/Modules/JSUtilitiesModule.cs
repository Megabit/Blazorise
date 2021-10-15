#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation for the utilities JS module.
    /// </summary>
    public class JSUtilitiesModule : BaseJSModule, IJSUtilitiesModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        public JSUtilitiesModule( IJSRuntime jsRuntime )
            : base( jsRuntime )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask AddClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "addClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "removeClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask ToggleClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "toggleClass", elementRef, classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask AddClassToBody( string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "addClassToBody", classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask RemoveClassFromBody( string classname )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "removeClassFromBody", classname );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<bool> ParentHasClass( ElementReference elementRef, string classname )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<bool>( "parentHasClass", elementRef, classname );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => "./_content/Blazorise/utilities.js";

        #endregion
    }
}