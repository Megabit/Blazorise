#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Sets the options for a Toast instance.
/// </summary>
public class ToastInstanceOptions
{
    /// <summary>
    /// Occurs before the toast is opened.
    /// Use this to intercept or modify the toast's behavior before it becomes visible.
    /// </summary>
    public Func<ToastOpeningEventArgs, Task> Opening { get; set; }

    /// <summary>
    /// Occurs before the toast is closed.
    /// Use this to intercept or modify the toast's behavior before it is hidden.
    /// </summary>
    public Func<ToastClosingEventArgs, Task> Closing { get; set; }

    /// <summary>
    /// Occurs after the toast has been successfully opened.
    /// </summary>
    public EventCallback? Opened { get; set; }

    /// <summary>
    /// Occurs after the toast has been successfully closed.
    /// </summary>
    public EventCallback? Closed { get; set; }

    /// <summary>
    /// Specifies whether the toast should use an animated transition.
    /// If <c>true</c>, the toast will have smooth transitions during opening and closing.
    /// </summary>
    public bool? Animated { get; set; }

    /// <summary>
    /// The duration of the animation in milliseconds.
    /// Applicable only when <see cref="Animated"/> is set to <c>true</c>.
    /// </summary>
    public int? AnimationDuration { get; set; }

    /// <summary>
    /// Specifies whether the toast should automatically hide after a delay.
    /// If <c>true</c>, the toast will disappear without user interaction.
    /// </summary>
    public bool? Autohide { get; set; }

    /// <summary>
    /// The delay in milliseconds before the toast is automatically hidden.
    /// Only applicable when <see cref="Autohide"/> is set to <c>true</c>.
    /// </summary>
    public double? AutohideDelay { get; set; }

    /// <summary>
    /// Custom CSS class name(s) to apply to the toast.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Custom inline styles to apply to the toast.
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// The icon to display inside the toast header.
    /// </summary>
    public IconName? IconName { get; set; }

    /// <summary>
    /// Determines whether to display an icon in the toast.
    /// </summary>
    public bool? ShowIcon { get; set; }

    /// <summary>
    /// Specifies the style of the icon displayed in the toast.
    /// </summary>
    public IconStyle? IconStyle { get; set; }

    /// <summary>
    /// The size of the icon displayed in the toast.
    /// </summary>
    public IconSize? IconSize { get; set; }

    /// <summary>
    /// Creates and returns the default options for a toast instance.
    /// </summary>
    /// <returns>An instance of <see cref="ToastInstanceOptions"/> with default values.</returns>
    public static ToastInstanceOptions Default => new()
    {
    };
}