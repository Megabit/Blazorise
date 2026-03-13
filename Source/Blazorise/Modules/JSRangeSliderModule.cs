#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="RangeSlider{TValue}"/> JS module.
/// </summary>
public class JSRangeSliderModule : BaseJSModule, IJSRangeSliderModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSRangeSliderModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference startElementRef, string startElementId, ElementReference endElementRef, string endElementId, bool clampToOtherHandle, bool allowEqualValues )
        => InvokeSafeVoidAsync( "initialize", startElementRef, startElementId, endElementRef, endElementId, clampToOtherHandle, allowEqualValues );

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/rangeSlider.js?v={VersionProvider.Version}";

    #endregion
}