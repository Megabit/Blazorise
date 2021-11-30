#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the closable JS module.
    /// </summary>
    public class JSClosableModule : BaseJSModule, IJSClosableModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSClosableModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask Register( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "registerClosableComponent", dotNetObjectRef, elementRef );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Unregister( ICloseActivator component )
        {
            if ( IsUnsafe )
                return;

            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "unregisterClosableComponent", component.ElementRef );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/closable.js?v={VersionProvider.Version}";

        #endregion
    }
}
