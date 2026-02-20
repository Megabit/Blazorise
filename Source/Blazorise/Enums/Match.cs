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
}