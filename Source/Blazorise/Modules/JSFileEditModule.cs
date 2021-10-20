#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the <see cref="FileEdit"/> JS module.
    /// </summary>
    public class JSFileEditModule : BaseJSModule, IJSFileEditModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSFileEditModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public virtual async ValueTask Initialize( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "destroy", elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<string>( "readFileData", elementRef, fileEntryId, position, length );
        }

        /// <inheritdoc/>
        public virtual async ValueTask Reset( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "reset", elementRef, elementId );
        }

        /// <inheritdoc/>
        public virtual async ValueTask OpenFileDialog( ElementReference elementRef, string elementId )
        {
            var moduleInstance = await Module;

            await moduleInstance.InvokeVoidAsync( "open", elementRef, elementId );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/fileEdit.js?v={VersionProvider.Version}";


        #endregion
    }
}
