#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Default implementation of the file JS module.
    /// </summary>
    public class JSFileModule : BaseJSModule, IJSFileModule
    {
        #region Constructors

        /// <summary>
        /// Default module constructor.
        /// </summary>
        /// <param name="jsRuntime">JavaScript runtime instance.</param>
        /// <param name="versionProvider">Version provider.</param>
        public JSFileModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
            : base( jsRuntime, versionProvider )
        {
        }

        #endregion

        #region Methods       

        /// <inheritdoc/>
        public virtual async ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default )
        {
            var moduleInstance = await Module;

            return await moduleInstance.InvokeAsync<string>( "readFileData", elementRef, fileEntryId, position, length );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/io.js?v={VersionProvider.Version}";


        #endregion
    }
}
