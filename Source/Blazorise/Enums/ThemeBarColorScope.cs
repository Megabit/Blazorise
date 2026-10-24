#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Specifies which bar orientations receive the configured bar colors.
/// </summary>
[Flags]
public enum ThemeBarColorScope
{
    /// <summary>
    /// Does not apply the configured bar colors.
    /// </summary>
    None = 0,

    /// <summary>
    /// Applies the configured colors to horizontal bars.
    /// </summary>
    Horizontal = 1,

    /// <summary>
    /// Applies the configured colors to vertical bars.
    /// </summary>
    Vertical = 2,

    /// <summary>
    /// Applies the configured colors to horizontal and vertical bars.
    /// </summary>
    Both = Horizontal | Vertical,
}