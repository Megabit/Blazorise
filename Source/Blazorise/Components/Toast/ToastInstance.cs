#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Represents an instance of a toast message with associated properties and behavior.
/// </summary>
public class ToastInstance
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToastInstance"/> class.
    /// </summary>
    /// <param name="toastProvider">The provider managing this toast instance.</param>
    /// <param name="toastId">A unique identifier for this toast instance.</param>
    /// <param name="title">The title text to display in the toast header.</param>
    /// <param name="message">The body content to display in the toast.</param>
    /// <param name="intent">The intent or purpose of the toast, which typically determines its styling.</param>
    /// <param name="toastInstanceOptions">Optional settings to customize this toast instance.</param>
    public ToastInstance(
        ToastProvider toastProvider,
        string toastId,
        string title,
        MarkupString message,
        ToastIntent intent,
        ToastInstanceOptions toastInstanceOptions )
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
    /// The provider managing this toast instance.
    /// </summary>
    public ToastProvider ToastProvider { get; private set; }

    /// <summary>
    /// A reference to the toast component associated with this instance.
    /// </summary>
    public Toast ToastRef { get; set; }

    /// <summary>
    /// A unique identifier for this toast instance.
    /// </summary>
    public string ToastId { get; set; }

    /// <summary>
    /// Indicates whether the toast is currently visible.
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// The title text displayed in the toast header.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// The content displayed in the body of the toast.
    /// </summary>
    public MarkupString Message { get; set; }

    /// <summary>
    /// Specifies the intent or purpose of the toast.
    /// This typically determines the styling or visual representation of the toast.
    /// </summary>
    public ToastIntent Intent { get; set; }

    /// <summary>
    /// Custom options for configuring this toast instance.
    /// </summary>
    public ToastInstanceOptions ToastInstanceOptions { get; private set; }

    /// <summary>
    /// Occurs before the toast is opened.
    /// </summary>
    public Func<ToastOpeningEventArgs, Task> Opening => ToastInstanceOptions?.Opening ?? ToastProvider.Opening;

    /// <summary>
    /// Occurs before the toast is closed.
    /// </summary>
    public Func<ToastClosingEventArgs, Task> Closing => ToastInstanceOptions?.Closing ?? ToastProvider.Closing;

    /// <summary>
    /// Occurs after the toast has successfully opened.
    /// </summary>
    public EventCallback Opened => ToastInstanceOptions?.Opened ?? ToastProvider.Opened;

    /// <summary>
    /// Occurs after the toast has successfully closed.
    /// </summary>
    public EventCallback Closed => ToastInstanceOptions?.Closed ?? ToastProvider.Closed;

    /// <summary>
    /// Specifies whether the toast should have an animated transition.
    /// </summary>
    public bool Animated => ToastInstanceOptions?.Animated ?? ToastProvider.Animated;

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// </summary>
    public int AnimationDuration => ToastInstanceOptions?.AnimationDuration ?? ToastProvider.AnimationDuration;

    /// <summary>
    /// Indicates whether the toast should automatically hide after a delay.
    /// </summary>
    public bool Autohide => ToastInstanceOptions?.Autohide ?? ToastProvider.Autohide;

    /// <summary>
    /// The delay in milliseconds before automatically hiding the toast.
    /// </summary>
    public double AutohideDelay => ToastInstanceOptions?.AutohideDelay ?? ToastProvider.AutohideDelay;

    /// <summary>
    /// Custom CSS class name(s) applied to the toast.
    /// </summary>
    public string Class => ToastInstanceOptions?.Class;

    /// <summary>
    /// Custom inline styles applied to the toast.
    /// </summary>
    public string Style => ToastInstanceOptions?.Style;

    /// <summary>
    /// Indicates whether an icon should be displayed in the toast.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool? ShowIcon => ToastInstanceOptions?.ShowIcon;

    /// <summary>
    /// The style applied to the icon in the toast (e.g., solid, regular).
    /// </summary>
    public IconStyle? IconStyle => ToastInstanceOptions?.IconStyle;

    /// <summary>
    /// The size of the icon displayed in the toast.
    /// </summary>
    public IconSize? IconSize => ToastInstanceOptions?.IconSize;

    /// <summary>
    /// The specific icon to display in the toast header.
    /// </summary>
    public IconName? IconName => ToastInstanceOptions?.IconName;
}