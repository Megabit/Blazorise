#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Supplies the information about the UI toast.
/// </summary>
public class ToastEventArgs : EventArgs
{
    /// <summary>
    /// A default <see cref="ToastEventArgs"/> constructor.
    /// </summary>
    /// <param name="intent">Type of the toast to show.</param>
    /// <param name="message">Toast content.</param>
    /// <param name="title">Title of the toast.</param>
    /// <param name="options">Additional options to override default toast settings.</param>
    public ToastEventArgs( ToastIntent intent, string message, string title, ToastInstanceOptions options )
        : this( intent, (MarkupString)message, title, options )
    {
    }

    /// <summary>
    /// A default <see cref="ToastEventArgs"/> constructor.
    /// </summary>
    /// <param name="intent">Type of the toast to show.</param>
    /// <param name="message">Toast content.</param>
    /// <param name="title">Title of the toast.</param>
    /// <param name="options">Additional options to override default toast settings.</param>
    public ToastEventArgs( ToastIntent intent, MarkupString message, string title, ToastInstanceOptions options )
    {
        Intent = intent;
        Message = message;
        Title = title;
        Options = options;
    }

    /// <summary>
    /// Gets the toast type.
    /// </summary>
    public ToastIntent Intent { get; }

    /// <summary>
    /// Gets the toast title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the toast description.
    /// </summary>
    public MarkupString Message { get; }

    /// <summary>
    /// Gets the options that will override default toast settings.
    /// </summary>
    public ToastInstanceOptions Options { get; }
}