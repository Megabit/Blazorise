#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the map of container breakpoint sizes.
/// </summary>
public record ThemeContainerMaxWidthOptions : IEnumerable<KeyValuePair<string, Func<string>>>
{
    private Dictionary<string, Func<string>> breakpointMap => new()
    {
        { "mobile", () => Mobile },
        { "tablet", () => Tablet },
        { "desktop", () => Desktop },
        { "widescreen", () => Widescreen },
        { "fullhd", () => FullHD },
    };

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
    {
        return breakpointMap.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return breakpointMap.GetEnumerator();
    }

    /// <summary>
    /// Gets the breakpoint handler associated with the specified breakpoint key.
    /// </summary>
    /// <param name="key">Breakpoint key</param>
    /// <returns>Return the breakpoint getter.</returns>
    public Func<string> this[string key] => breakpointMap[key];

    /// <summary>
    /// Gets or sets the breakpoint size for mobile screens.
    /// </summary>
    public string Mobile { get; set; } = "540px";

    /// <summary>
    /// Gets or sets the breakpoint size for tablet screens.
    /// </summary>
    public string Tablet { get; set; } = "720px";

    /// <summary>
    /// Gets or sets the breakpoint size for desktop screens.
    /// </summary>
    public string Desktop { get; set; } = "960px";

    /// <summary>
    /// Gets or sets the breakpoint size for wide screens.
    /// </summary>
    public string Widescreen { get; set; } = "1140px";

    /// <summary>
    /// Gets or sets the breakpoint size for largest screens.
    /// </summary>
    public string FullHD { get; set; } = "1320px";
}