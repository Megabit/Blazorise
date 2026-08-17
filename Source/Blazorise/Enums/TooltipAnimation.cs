#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the visual effects used when a tooltip is shown or hidden. Multiple effects can be combined.
/// </summary>
[Flags]
public enum TooltipAnimation
{
    /// <summary>
    /// Disables the tooltip transition.
    /// </summary>
    None = 0,

    /// <summary>
    /// Fades the tooltip in and out.
    /// </summary>
    Fade = 1 << 0,

    /// <summary>
    /// Scales the tooltip in and out.
    /// </summary>
    Scale = 1 << 1,

    /// <summary>
    /// Shifts the tooltip toward or away from its target.
    /// </summary>
    Shift = 1 << 2,

    /// <summary>
    /// Blurs the tooltip in and out.
    /// </summary>
    Blur = 1 << 3,

    /// <summary>
    /// Uses the tooltip animation preferred by the active CSS provider.
    /// </summary>
    Auto = 1 << 4,

    /// <summary>
    /// Fades and scales the tooltip in and out.
    /// </summary>
    FadeScale = Fade | Scale,

    /// <summary>
    /// Fades and shifts the tooltip toward or away from its target.
    /// </summary>
    FadeShift = Fade | Shift,

    /// <summary>
    /// Fades and blurs the tooltip in and out.
    /// </summary>
    FadeBlur = Fade | Blur,
}