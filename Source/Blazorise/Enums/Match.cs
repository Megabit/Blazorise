#region Using directives
using System;
#endregion

namespace Blazorise;

/// <summary>
/// Modifies the URL matching behavior for a link.
/// </summary>
public enum Match
{
    /// <summary>
    /// Specifies that the link should be active when it matches any prefix of the current URL.
    /// </summary>
    Prefix,

    /// <summary>
    /// Specifies that the link should be active when it matches the entire current URL.
    /// </summary>
    All,

    /// <summary>
    /// Specifies that the link should be active when it matches the supplied <see cref="BaseLinkComponent.CustomMatch"/> method callback.
    /// </summary>
    [Obsolete( "Custom match is obsolete and should be removed in future versions." )]
    Custom,
}