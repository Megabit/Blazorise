#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="FileInput"/> JS module.
/// </summary>
public class JSFileInputModule : BaseJSModule, IJSFileInputModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSFileInputModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( DotNetObjectReference<FileInputAdapter> dotNetObjectRef, ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef, elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask Destroy( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "destroy", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask Reset( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "reset", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask RemoveFile( ElementReference elementRef, string elementId, int fileId )
        => InvokeSafeVoidAsync( "removeFile", elementRef, elementId, fileId );

    /// <inheritdoc/>
    public virtual ValueTask OpenFileDialog( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "open", elementRef, elementId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/fileInput.js?v={VersionProvider.Version}";

    #endregion
}