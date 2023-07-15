#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the map of background colors.
/// </summary>
public record ThemeBackgroundOptions : IEnumerable<KeyValuePair<string, Func<string>>>
{
    /// <summary>
    /// Map of background colors.
    /// </summary>
    protected Dictionary<string, Func<string>> ColorMap => new()
    {
        { "primary", () => Primary },
        { "secondary", () => Secondary },
        { "success", () => Success },
        { "info", () => Info },
        { "warning", () => Warning },
        { "danger", () => Danger },
        { "light", () => Light },
        { "dark", () => Dark },
        { "body", () => Body },
        { "muted", () => Muted }
    };

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, Func<string>>> GetEnumerator()
    {
        return ColorMap.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ColorMap.GetEnumerator();
    }

    /// <summary>
    /// Gets the color handler associated with the specified color key.
    /// </summary>
    /// <param name="key">Color key.</param>
    /// <returns>Return the color getter.</returns>
    public Func<string> this[string key] => ColorMap[key];

    /// <summary>
    /// Gets or sets the background primary color.
    /// </summary>
    public string Primary { get; set; } = ThemeColors.Blue.Shades["400"].Value;

    /// <summary>
    /// Gets or sets the background secondary color.
    /// </summary>
    public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

    /// <summary>
    /// Gets or sets the background success color.
    /// </summary>
    public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the background info color.
    /// </summary>
    public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the background warning color.
    /// </summary>
    public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the background danger color.
    /// </summary>
    public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the background light color.
    /// </summary>
    public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

    /// <summary>
    /// Gets or sets the background dark color.
    /// </summary>
    public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;

    /// <summary>
    /// Gets or sets the background body color.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the background muted color.
    /// </summary>
    public string Muted { get; set; }
}