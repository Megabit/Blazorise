#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="_TransitionableRoute"/> JS module.
/// </summary>
public interface IJSTransitionableRouteModule
{
    /// <summary>
    /// Initializes the new <see cref="_TransitionableRoute"/> within the JS module.
    /// </summary>
    /// <param name="dotNetObjectReference">Reference to the date adapter.</param>
    /// <param name="options">Additional options for the module initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<_TransitionableRoute> dotNetObjectReference, object options );
}
