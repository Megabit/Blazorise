#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the <see cref="_TransitionableRoute"/> JS module.
/// </summary>
public class JSTransitionableRouteModule : BaseJSModule, IJSTransitionableRouteModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSTransitionableRouteModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( DotNetObjectReference<_TransitionableRoute> dotNetObjectReference, object options )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectReference, options );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/transitionableRoute.js?v={VersionProvider.Version}";

    #endregion
}
