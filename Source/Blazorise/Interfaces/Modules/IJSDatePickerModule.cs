#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="DatePicker{TValue}"/> JS module.
/// </summary>
public interface IJSDatePickerModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="DatePicker{TValue}"/> within the JS module.
    /// </summary>
    /// <param name="dotNetObjectReference">Reference to the date adapter.</param>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Additional options for the tooltip initialization.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<DatePickerAdapter> dotNetObjectReference, ElementReference elementRef, string elementId, DatePickerInitializeJSOptions options );

    /// <summary>
    /// Activates the <see cref="DatePicker{TValue}"/>.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Additional options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Activate( ElementReference elementRef, string elementId, object options );

    /// <summary>
    /// Updates the <see cref="DatePicker{TValue}"/> value.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="value">Value to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateValue( ElementReference elementRef, string elementId, object value );

    /// <summary>
    /// Updates the <see cref="DatePicker{TValue}"/> options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateOptions( ElementReference elementRef, string elementId, DatePickerUpdateJSOptions options );

    /// <summary>
    /// Opens the <see cref="DatePicker{TValue}"/> dropdown menu.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Open( ElementReference elementRef, string elementId );

    /// <summary>
    /// Closes the <see cref="DatePicker{TValue}"/> dropdown menu.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Close( ElementReference elementRef, string elementId );

    /// <summary>
    /// Toggles the <see cref="DatePicker{TValue}"/> dropdown menu.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Toggle( ElementReference elementRef, string elementId );

    /// <summary>
    /// Updates the <see cref="DatePicker{TValue}"/> localization.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="localization">Options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateLocalization( ElementReference elementRef, string elementId, object localization );

    /// <summary>
    /// Sets focus to the <see cref="DatePicker{TValue}"/>.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="scrollToElement">If true, scrolls to the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement );

    /// <summary>
    /// Selects the <see cref="DatePicker{TValue}"/>.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="focus">If true, it will focus to the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Select( ElementReference elementRef, string elementId, bool focus );
}