#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the <see cref="MemoEdit"/> JS module.
    /// </summary>
    public class JSMemoEditModule : BaseJSModule, IJSMemoEditModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSMemoEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
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
            if ( IsUnsafe )
                return;

            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options )
        {
            if ( IsUnsafe )
                return;

            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/memoEdit.js?v={VersionProvider.Version}";

        #endregion
    }
}
