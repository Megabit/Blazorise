#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="Offcanvas"/> JS module.
/// </summary>
public abstract class JSOffcanvasModule : BaseJSModule, IJSOffcanvasModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSOffcanvasModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask OpenOffcanvas( ElementReference elementRef, Placement placement )
        => InvokeSafeVoidAsync( "openOffcanvas", elementRef, placement.ToString().ToLower() );

    /// <inheritdoc/>
    public virtual ValueTask CloseOffcanvas( ElementReference elementRef )
        => InvokeSafeVoidAsync( "closeOffcanvas", elementRef );

    #endregion
}