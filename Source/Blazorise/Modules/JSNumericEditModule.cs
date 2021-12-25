#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the <see cref="NumericEdit{TValue}"/> JS module.
    /// </summary>
    public class JSNumericEditModule : BaseJSModule, IJSNumericEditModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSNumericEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask Initialize( DotNetObjectReference<NumericEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId, object options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId, options );
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

        /// <inheritdoc/>
        public virtual async ValueTask UpdateValue<TValue>( ElementReference elementRef, string elementId, TValue value )
        {
            if ( IsUnsafe )
                return;

            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "updateValue", elementRef, elementId, value );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/numericEdit.js?v={VersionProvider.Version}";

        #endregion
    }
}
