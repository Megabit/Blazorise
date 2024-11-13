#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.QRCode;

/// <summary>
/// Contracts for the <see cref="QRCode"/> JS module.
/// </summary>
public class JSQRCodeModule : BaseJSModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSQRCodeModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options ) : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the new QRCode within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Additional options for the tooltip initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId, QRCodeJSOptions options )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId, options );

    /// <summary>
    /// Updates the QRCode options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask Update( ElementReference elementRef, string elementId, object options )
        => InvokeSafeVoidAsync( "update", elementRef, elementId, options );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.QRCode/blazorise.qrcode.js?v={VersionProvider.Version}";

    #endregion
}
