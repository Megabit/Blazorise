#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="Tabs"/> JS module.
/// </summary>
public interface IJSTabsModule : IBaseJSModule, IJSDestroyableModule
{
    /// <summary>
    /// Initializes keyboard navigation for the tabs.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId );
}