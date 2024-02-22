using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Defines the message to show in the <see cref="Toast"/>.
/// </summary>
public class ToastInstance
{
    public ToastInstance( ToastProvider toastProvider, string toastId, string title, string message, ToastInstanceOptions toastInstanceOptions )
    {
        ToastProvider = toastProvider;
        ToastId = toastId;
        Visible = true;
        Title = title;
        Message = message;
        ToastInstanceOptions = toastInstanceOptions;
    }

    /// <summary>
    /// The Toast provider.
    /// </summary>
    public ToastProvider ToastProvider { get; private set; }

    /// <summary>
    /// Tracks the Toast reference.
    /// </summary>
    public Toast ToastRef { get; set; }

    /// <summary>
    /// Tracks the Toast id.
    /// </summary>
    public string ToastId { get; set; }

    /// <summary>
    /// Control's the Toast visibility.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// Text to show in the toast header.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Text to show in the toast body.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Sets the options for ToastProvider.
    /// </summary>
    public ToastInstanceOptions ToastInstanceOptions { get; private set; }

    /// <summary>
    /// Occurs after the toast has opened.
    /// </summary>
    public EventCallback Opened => ToastInstanceOptions?.Opened ?? ToastProvider.Opened;

    /// <summary>
    /// Occurs after the toast has closed.
    /// </summary>
    public EventCallback Closed => ToastInstanceOptions?.Closed ?? ToastProvider.Closed;
}
