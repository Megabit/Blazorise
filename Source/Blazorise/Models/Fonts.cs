#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Defines built-in Blazorise font family names.
/// </summary>
public static class Fonts
{
    /// <summary>
    /// Sans-serif built-in font family.
    /// </summary>
    public const string Helvetica = "Helvetica";

    /// <summary>
    /// Serif built-in font family.
    /// </summary>
    public const string Times = "Times";

    /// <summary>
    /// Monospace built-in font family.
    /// </summary>
    public const string Courier = "Courier";

    /// <summary>
    /// Built-in font families.
    /// </summary>
    public static IReadOnlyList<FontFamily> BuiltIn { get; } =
    [
        new()
        {
            Name = Helvetica,
            DisplayName = Helvetica,
            CssFamily = "Helvetica, Arial, sans-serif",
        },
        new()
        {
            Name = Times,
            DisplayName = Times,
            CssFamily = "\"Times New Roman\", Times, serif",
        },
        new()
        {
            Name = Courier,
            DisplayName = Courier,
            CssFamily = "\"Courier New\", Courier, monospace",
        },
    ];
}