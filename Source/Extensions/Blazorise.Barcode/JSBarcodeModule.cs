#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Barcode;

/// <summary>
/// Contracts for the <see cref="Barcode"/> JS module.
/// </summary>
public class JSBarcodeModule : BaseJSModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSBarcodeModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options ) : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes the new Barcode within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Additional options for the barcode initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId, BarcodeJSOptions options )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId, options );

    /// <summary>
    /// Updates the Barcode options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask Update( ElementReference elementRef, string elementId, BarcodeJSOptions options )
        => InvokeSafeVoidAsync( "update", elementRef, elementId, options );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Barcode/blazorise.barcode.js?v={VersionProvider.Version}";

    #endregion
}