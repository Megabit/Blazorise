using System;
using System.Collections.Generic;

namespace Blazorise.Reporting;

/// <summary>
/// Common font families supported by the built-in report PDF renderer.
/// </summary>
public static class ReportFontFamilies
{
    /// <summary>
    /// Sans-serif font family.
    /// </summary>
    public const string Helvetica = "Helvetica";

    /// <summary>
    /// Serif font family.
    /// </summary>
    public const string Times = "Times";

    /// <summary>
    /// Monospace font family.
    /// </summary>
    public const string Courier = "Courier";

    /// <summary>
    /// Supported font family names.
    /// </summary>
    public static IReadOnlyList<string> Supported { get; } =
    [
        Helvetica,
        Times,
        Courier,
    ];

    /// <summary>
    /// Normalizes a font family name to one of the supported report font family names.
    /// </summary>
    /// <param name="family">Font family name.</param>
    /// <returns>A supported font family name, or null when the supplied family is not supported.</returns>
    public static string Normalize( string family )
    {
        foreach ( string supportedFamily in Supported )
        {
            if ( string.Equals( family, supportedFamily, StringComparison.OrdinalIgnoreCase ) )
                return supportedFamily;
        }

        return null;
    }
}