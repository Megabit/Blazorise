#region Using directives
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

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
    /// <param name="options">Blazorise options.</param>
    public JSFileModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods       

    /// <inheritdoc/>
    public virtual ValueTask<byte[]> ReadDataAsync( ElementReference elementRef, int fileEntryId, long position, long length, CancellationToken cancellationToken = default )
        => InvokeSafeAsync<byte[]>( "readFileData", elementRef, fileEntryId, position, length );

    /// <inheritdoc/>
    public virtual ValueTask<IJSStreamReference> ReadDataAsync( ElementReference elementRef, int fileEntryId, CancellationToken cancellationToken = default )
        => InvokeSafeAsync<IJSStreamReference>( "readFileDataStream", elementRef, fileEntryId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/io.js?v={VersionProvider.Version}";


    #endregion
}