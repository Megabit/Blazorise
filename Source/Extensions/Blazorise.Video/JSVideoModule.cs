#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Video
{
    public class JSVideoModule : BaseJSModule,
        IJSDestroyableModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSVideoModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        public virtual async ValueTask Initialize( ElementReference elementRef, string elementId, object options )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", elementRef, elementId, options );
        }

        public virtual async ValueTask Destroy( ElementReference canvasRef, string canvasId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, canvasId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise.Video/video.js?v={VersionProvider.Version}";

        #endregion
    }
}
