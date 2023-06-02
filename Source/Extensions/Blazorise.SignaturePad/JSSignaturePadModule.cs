#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.SignaturePad
{
    /// <summary>
    /// Default implementation of the signature pad JS module.
    /// </summary>
    public class JSSignaturePadModule : BaseJSModule,
        IJSDestroyableModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSSignaturePadModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        public virtual async ValueTask Initialize( DotNetObjectReference<SignaturePad> dotNetObjectReference, ElementReference elementRef, string elementId, object options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectReference, elementRef, elementId, options );
        }

        public virtual async ValueTask Destroy( ElementReference canvasRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, elementId );
        }

        public virtual async ValueTask UpdateOptions( ElementReference elementRef, string elementId, object options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "updateOptions", elementRef, elementId, options );
        }

        public virtual async ValueTask Clear( ElementReference canvasRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "clear", canvasRef, elementId );
        }

        public virtual async ValueTask<string> Undo( ElementReference canvasRef, string elementId )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<string>( "undo", canvasRef, elementId );
        }

        public virtual async ValueTask<SignaturePad> GetData( ElementReference canvasRef )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<SignaturePad>( "getData", canvasRef );
        }

        public virtual async ValueTask SetData( ElementReference canvasRef, string elementId, byte[] data )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "setData", canvasRef, elementId, data );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.SignaturePad/signaturepad.js";

        #endregion
    }
}