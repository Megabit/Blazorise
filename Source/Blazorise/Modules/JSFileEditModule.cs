﻿#region Using directives
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
        public virtual ValueTask Initialize( DotNetObjectReference<FileEditAdapter> dotNetObjectRef, ElementReference elementRef, string elementId )
            => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId );

        /// <inheritdoc/>
        public virtual ValueTask Destroy( ElementReference elementRef, string elementId )
            => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

        /// <inheritdoc/>
        public virtual ValueTask<string> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default )
            => InvokeSafeAsync<string>( "readFileData", elementRef, fileEntryId, position, length );

        /// <inheritdoc/>
        public virtual ValueTask Reset( ElementReference elementRef, string elementId )
            => InvokeSafeVoidAsync( "reset", elementRef, elementId );

        /// <inheritdoc/>
        public virtual ValueTask OpenFileDialog( ElementReference elementRef, string elementId )
            => InvokeSafeVoidAsync( "open", elementRef, elementId );

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override string ModuleFileName => $"./_content/Blazorise/fileEdit.js?v={VersionProvider.Version}";


        #endregion
    }
}
