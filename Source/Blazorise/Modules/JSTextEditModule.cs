#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the <see cref="TextEdit"/> JS module.
    /// </summary>
    public class JSTextEditModule : BaseJSModule, IJSTextEditModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSTextEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask Initialize( ElementReference elementRef, string elementId, string maskType, string editMask )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", elementRef, elementId, maskType, editMask );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
        {
            if ( IsUnsafe )
                return;

            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/textEdit.js?v={VersionProvider.Version}";

        #endregion
    }
}
