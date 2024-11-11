#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules.JSOptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="ColorPicker"/> JS module.
/// </summary>
public interface IJSColorPickerModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="ColorPicker"/> within the JS module.
    /// </summary>
    /// <param name="dotNetObjectRef">Reference to the color picker.</param>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Additional options for the tooltip initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<ColorPicker> dotNetObjectRef, ElementReference elementRef, string elementId, ColorPickerJSOptions options );

    /// <summary>
    /// Updates the <see cref="ColorPicker"/> value.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="value">Value to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateValue( ElementReference elementRef, string elementId, object value );

    /// <summary>
    /// Updates the <see cref="ColorPicker"/> options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateOptions( ElementReference elementRef, string elementId, ColorPickerUpdateJsOptions options );

    /// <summary>
    /// Updates the <see cref="ColorPicker"/> localization.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="localization">Options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateLocalization( ElementReference elementRef, string elementId, object localization );

    /// <summary>
    /// Sets focus to the <see cref="ColorPicker"/>.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="scrollToElement">If true, scrolls to the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement );

    /// <summary>
    /// Selects the <see cref="ColorPicker"/>.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="focus">If true, it will focus to the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Select( ElementReference elementRef, string elementId, bool focus );
}