#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the map of main theme colors.
/// </summary>
public record ThemeColorOptions : IEnumerable<KeyValuePair<string, Func<string>>>
{
    private Dictionary<string, Func<string>> colorMap => new()
    {
        { "primary", () => Primary },
        { "secondary", () => Secondary },
        { "success", () => Success },
        { "info", () => Info },
        { "warning", () => Warning },
        { "danger", () => Danger },
        { "light", () => Light },
        { "dark", () => Dark },
        { "link", () => Link },
    };

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
    {
        return colorMap.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return colorMap.GetEnumerator();
    }

    /// <summary>
    /// Gets the color handler associated with the specified color key.
    /// </summary>
    /// <param name="key">Color key</param>
    /// <returns>Return the color getter.</returns>
    public Func<string> this[string key] => colorMap[key];

    /// <summary>
    /// Gets or sets the primary theme color.
    /// </summary>
    public string Primary { get; set; } = ThemeColors.Blue.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the secondary theme color.
    /// </summary>
    public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

    /// <summary>
    /// Gets or sets the success theme color.
    /// </summary>
    public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the info theme color.
    /// </summary>
    public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the warning theme color.
    /// </summary>
    public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the danger theme color.
    /// </summary>
    public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the light theme color.
    /// </summary>
    public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

    /// <summary>
    /// Gets or sets the dark theme color.
    /// </summary>
    public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;

    /// <summary>
    /// Gets or sets the link theme color.
    /// </summary>
    public string Link { get; set; } = ThemeColors.Blue.Shades["500"].Value;
}