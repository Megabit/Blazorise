#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Service to show a simple toast message.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// An event raised after the toast is received.
    /// </summary>
    public event EventHandler<ToastEventArgs> ToastReceived;

    /// <summary>
    /// Show the simple info toast.
    /// </summary>
    /// <param name="message">Info toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Info( string message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple info toast.
    /// </summary>
    /// <param name="message">Info toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Info( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple success toast.
    /// </summary>
    /// <param name="message">Success toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Success( string message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple success toast.
    /// </summary>
    /// <param name="message">Success toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Success( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple warning toast.
    /// </summary>
    /// <param name="message">Warning toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Warning( string message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple warning toast.
    /// </summary>
    /// <param name="message">Warning toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Warning( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple error toast.
    /// </summary>
    /// <param name="message">Error toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Error( string message, string title = null, Action<ToastInstanceOptions> options = null );

    /// <summary>
    /// Show the simple error toast.
    /// </summary>
    /// <param name="message">Error toast to show.</param>
    /// <param name="title">Toast title.</param>
    /// <param name="options">Options to override toast dialog appearance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Error( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null );
}