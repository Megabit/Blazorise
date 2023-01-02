#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the map of spacing sizes.
/// </summary>
public record ThemeSpacingOptions : IEnumerable<KeyValuePair<string, Func<string>>>
{
    /// <summary>
    /// Map of text colors.
    /// </summary>
    private Dictionary<string, Func<string>> SpacingMap => new()
    {
        { "0", () => Is0 },
        { "1", () => Is1 },
        { "2", () => Is2 },
        { "3", () => Is3 },
        { "4", () => Is4 },
        { "5", () => Is5 },
    };

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
    {
        return SpacingMap.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return SpacingMap.GetEnumerator();
    }

    /// <summary>
    /// Gets the spacing associated with the specified key.
    /// </summary>
    /// <param name="key">Spacing key</param>
    /// <returns>Return the spacing getter.</returns>
    public Func<string> this[string key] => SpacingMap[key];

    /// <summary>
    /// Gets or sets the zero spacing size.
    /// </summary>
    public string Is0 { get; set; } = "0";

    /// <summary>
    /// Gets or sets the spacing for the extra small sizes.
    /// </summary>
    public string Is1 { get; set; } = "0.25rem";

    /// <summary>
    /// Gets or sets the spacing for the small sizes.
    /// </summary>
    public string Is2 { get; set; } = "0.5rem";

    /// <summary>
    /// Gets or sets the spacing for the default sizes.
    /// </summary>
    public string Is3 { get; set; } = "1rem";

    /// <summary>
    /// Gets or sets the spacing for the large sizes.
    /// </summary>
    public string Is4 { get; set; } = "1.5rem";

    /// <summary>
    /// Gets or sets the spacing for the extra large sizes.
    /// </summary>
    public string Is5 { get; set; } = "3rem";
}