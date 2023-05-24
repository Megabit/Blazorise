﻿#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the dropdown JS module.
/// </summary>
public interface IJSDropdownModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new dropdown within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="targetElementId">Target element for the dropdown menu.</param>
    /// <param name="altTargetElementId">An alternative target element for the dropdown menu, usually for split dropdowns.</param>
    /// <param name="menuElementId">Dropdown menu element id.</param>
    /// <param name="showElementId">Id for the target element that will be listening to the 'show event'.</param>
    /// <param name="options">Additional options for the button initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId, string targetElementId, string altTargetElementId, string menuElementId, string showElementId, object options );

    /// <summary>
    /// Shows the dropdown menu.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Show( ElementReference elementRef, string elementId );

    /// <summary>
    /// Hides the dropdown menu.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Hide( ElementReference elementRef, string elementId );
}