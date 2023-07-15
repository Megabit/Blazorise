using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Blazorise.States;

/// <summary>
/// Tracks the Bar Toggler State.
/// </summary>
public record BarTogglerState
{
    /// <summary>
    /// Occurs when the button is clicked.
    /// </summary>
    public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Provides options for inline or popout styles. Only supported by Vertical Bar. Uses inline by default.
    /// </summary>
    public BarTogglerMode Mode { get; set; }

    /// <summary>
    /// Controls which <see cref="Bar"/> will be toggled. Uses parent <see cref="Bar"/> by default. 
    /// </summary>
    public Bar Bar { get; set; }
}