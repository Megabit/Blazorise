#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines the map of text colors.
/// </summary>
public record ThemeTextColorOptions : IEnumerable<KeyValuePair<string, Func<string>>>
{
    /// <summary>
    /// Map of text colors.
    /// </summary>
    private Dictionary<string, Func<string>> ColorMap => new()
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
        { "muted", () => Muted },
        { "white", () => White },
        { "black50", () => Black50 },
        { "white50", () => White50 },
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
    /// <param name="key">Color key</param>
    /// <returns>Return the color getter.</returns>
    public Func<string> this[string key] => ColorMap[key];

    /// <summary>
    /// Gets or sets the text primary color.
    /// </summary>
    public string Primary { get; set; } = ThemeColors.Blue.Shades["400"].Value;

    /// <summary>
    /// Gets or sets the text secondary color.
    /// </summary>
    public string Secondary { get; set; } = ThemeColors.Gray.Shades["600"].Value;

    /// <summary>
    /// Gets or sets the text success color.
    /// </summary>
    public string Success { get; set; } = ThemeColors.Green.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the text info color.
    /// </summary>
    public string Info { get; set; } = ThemeColors.Cyan.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the text warning color.
    /// </summary>
    public string Warning { get; set; } = ThemeColors.Yellow.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the text danger color.
    /// </summary>
    public string Danger { get; set; } = ThemeColors.Red.Shades["500"].Value;

    /// <summary>
    /// Gets or sets the text light color.
    /// </summary>
    public string Light { get; set; } = ThemeColors.Gray.Shades["100"].Value;

    /// <summary>
    /// Gets or sets the text dark color.
    /// </summary>
    public string Dark { get; set; } = ThemeColors.Gray.Shades["800"].Value;

    /// <summary>
    /// Gets or sets the text body color.
    /// </summary>
    public string Body { get; set; } = ThemeColors.Gray.Shades["900"].Value;

    /// <summary>
    /// Gets or sets the text muted color.
    /// </summary>
    public string Muted { get; set; } = ThemeColors.Gray.Shades["600"].Value;

    /// <summary>
    /// Gets or sets the text white color.
    /// </summary>
    public string White { get; set; } = ThemeColors.Gray.Shades["50"].Value;

    /// <summary>
    /// Gets or sets the text black-50% color.
    /// </summary>
    public string Black50 { get; set; } = "#000000";

    /// <summary>
    /// Gets or sets the text white-50% color.
    /// </summary>
    public string White50 { get; set; } = "#FFFFFF";
}