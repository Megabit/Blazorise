using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Defines the message to show in the <see cref="Toast"/>.
/// </summary>
public class ToastInstance
{
    /// <summary>
    /// The constructor for the ToastInstance.
    /// </summary>
    /// <param name="toastProvider"></param>
    /// <param name="toastId"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="intent"></param>
    /// <param name="toastInstanceOptions"></param>
    public ToastInstance( ToastProvider toastProvider, string toastId, string title, MarkupString message, ToastIntent intent, ToastInstanceOptions toastInstanceOptions )
    {
        ToastProvider = toastProvider;
        ToastId = toastId;
        Visible = true;
        Title = title;
        Message = message;
        Intent = intent;
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
    public MarkupString Message { get; set; }

    /// <summary>
    /// Intent of the toast message.
    /// </summary>
    public ToastIntent Intent { get; set; }

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

    /// <summary>
    /// Specifies whether the Toast should have an animated transition.
    /// </summary>
    public bool Animated => ToastInstanceOptions?.Animated ?? ToastProvider.Animated;

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    public int AnimationDuration => ToastInstanceOptions?.AnimationDuration ?? ToastProvider.AnimationDuration;

    /// <summary>
    /// Automatically hide the toast after the delay.
    /// </summary>
    public bool Autohide => ToastInstanceOptions?.Autohide ?? ToastProvider.Autohide;

    /// <summary>
    /// Delay in milliseconds before hiding the toast.
    /// </summary>
    public double AutohideDelay => ToastInstanceOptions?.AutohideDelay ?? ToastProvider.AutohideDelay;
}
