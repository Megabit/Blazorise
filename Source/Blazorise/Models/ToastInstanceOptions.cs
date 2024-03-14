using Microsoft.AspNetCore.Components;

namespace Blazorise;

/// <summary>
/// Sets the options for Toast instance.
/// </summary>
public class ToastInstanceOptions
{
    /// <summary>
    /// Occurs after the toast has opened.
    /// </summary>
    public EventCallback? Opened { get; set; }

    /// <summary>
    /// Occurs after the toast has closed.
    /// </summary>
    public EventCallback? Closed { get; set; }

    /// <summary>
    /// Creates the default toast options.
    /// </summary>
    /// <returns>Default toast options.</returns>
    public static ToastInstanceOptions Default => new()
    {
    };
}
