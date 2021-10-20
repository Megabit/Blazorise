#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the <see cref="Table"/> JS module.
    /// </summary>
    public class JSTableModule : BaseJSModule, IJSTableModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSTableModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask InitializeFixedHeader( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initializeTableFixedHeader", elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask DestroyFixedHeader( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroyTableFixedHeader", elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask ScrollTableToPixels( ElementReference elementRef, string elementId, int pixels )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "fixedHeaderScrollTableToPixels", elementRef, elementId, pixels );
        }

        /// <inheritdoc/>
        public virtual async ValueTask ScrollTableToRow( ElementReference elementRef, string elementId, int row )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "fixedHeaderScrollTableToRow", elementRef, elementId, row );
        }

        /// <inheritdoc/>
        public virtual async ValueTask InitializeResizable( ElementReference elementRef, string elementId, TableResizeMode resizeMode )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initializeResizable", elementRef, elementId, resizeMode );
        }

        /// <inheritdoc/>
        public virtual async ValueTask DestroyResizable( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroyResizable", elementRef, elementId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/table.js?v={VersionProvider.Version}";

        #endregion
    }
}
