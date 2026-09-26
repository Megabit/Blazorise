#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="Tabs"/> JS module.
/// </summary>
public class JSTabsModule : BaseJSModule, IJSTabsModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSTabsModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId );

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
    public override string ModuleFileName => $"./_content/Blazorise/tabs.js?v={VersionProvider.Version}";

    #endregion
}