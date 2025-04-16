#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the various utilites JS module.
/// </summary>
public interface IJSUtilitiesModule : IBaseJSModule
{
    /// <summary>
    /// Adds a classname to the specified element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="classname">CSS classname to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask AddClass( ElementReference elementRef, string classname );

    /// <summary>
    /// Removes a classname from the specified element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="classname">CSS classname to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveClass( ElementReference elementRef, string classname );

    /// <summary>
    /// Toggles a classname on the given element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="classname">CSS classname to toggle.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ToggleClass( ElementReference elementRef, string classname );

    /// <summary>
    /// Adds a classname to the body element.
    /// </summary>
    /// <param name="classname">CSS classname to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask AddClassToBody( string classname );

    /// <summary>
    /// Removes a classname from the body element.
    /// </summary>
    /// <param name="classname"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveClassFromBody( string classname );

    /// <summary>
    /// Adds an attribute to the body element.
    /// </summary>
    /// <param name="attribute">A string specifying the name of the attribute whose value is to be set.</param>
    /// <param name="value">A string containing the value to assign to the attribute.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask AddAttributeToBody( string attribute, string value );

    /// <summary>
    /// Removes an attribute from the body element.
    /// </summary>
    /// <param name="attribute">A string specifying the name of the attribute to remove from the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveAttributeFromBody( string attribute );

    /// <summary>
    /// Indicates if parent element has a specified classname.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="classname">CSS classname to check.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask<bool> ParentHasClass( ElementReference elementRef, string classname );

    /// <summary>
    /// Sets focus to the given element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="scrollToElement">If true, scrolls to the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Focus( ElementReference elementRef, string elementId, bool scrollToElement );

    /// <summary>
    /// Selects the given element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="focus">If true, it will focus to the element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Select( ElementReference elementRef, string elementId, bool focus );

    /// <summary>
    /// Show a browser picker for the supplied input element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ShowPicker( ElementReference elementRef, string elementId );

    /// <summary>
    /// Scrolls the view to the given anchor element.
    /// </summary>
    /// <param name="anchorTarget">Anchor element id.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollAnchorIntoView( string anchorTarget );

    /// <summary>
    /// Scrolls the view to the given element.
    /// </summary>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="smooth">True if the scroll animation should be smoothed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollElementIntoView( string elementId, bool smooth = true );

    /// <summary>
    /// Sets the caret to the specified position.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="caret">Caret position.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SetCaret( ElementReference elementRef, int caret );

    /// <summary>
    /// Gets the caret position.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask<int> GetCaret( ElementReference elementRef );

    /// <summary>
    /// Updates the input with the specified value.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="value">New element value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SetTextValue( ElementReference elementRef, object value );

    /// <summary>
    /// Apply the html property to the element.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="property">Property name.</param>
    /// <param name="value">Property value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask SetProperty( ElementReference elementRef, string property, object value );

    /// <summary>
    /// Gets the element info.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask<DomElement> GetElementInfo( ElementReference elementRef, string elementId );

    /// <summary>
    /// Gets the User Agent name.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask<string> GetUserAgent();

    /// <summary>
    /// Copies the specified element content to the clipboard.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask CopyToClipboard( ElementReference elementRef, string elementId );

    /// <summary>
    /// Writes a log message to the browser console.
    /// </summary>
    /// <param name="message">Message to write.</param>
    /// <param name="args">Optional parameters.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Log( string message, params string[] args );

    /// <summary>
    /// Checks if the current system theme is in dark mode.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the theme is in dark mode, otherwise false.</returns>
    ValueTask<bool> IsSystemDarkMode();

}