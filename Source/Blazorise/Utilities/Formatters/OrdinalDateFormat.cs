#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Blazorise.Localization;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Resolves date-specific ordinal patterns from localization resources.
/// </summary>
internal static class OrdinalDateFormat
{
    #region Members

    private static readonly string[] categories = [ "Zero", "One", "Two", "Few", "Many", "Other" ];

    #endregion

    #region Methods

    /// <summary>
    /// Gets the native date format for an ordinal day in the specified culture.
    /// </summary>
    public static string GetDayFormat( int day, CultureInfo culture, ITextLocalizer localizer )
        => GetDayFormat( GetDayCategory( day, culture ), culture, localizer );

    /// <summary>
    /// Gets the distinct formats to consider when parsing an ordinal day.
    /// </summary>
    public static IEnumerable<string> GetDayFormats( CultureInfo culture, ITextLocalizer localizer )
    {
        var formats = new HashSet<string>( StringComparer.Ordinal );

        foreach ( var category in categories )
        {
            var format = GetDayFormat( category, culture, localizer );

            if ( formats.Add( format ) )
            {
                yield return format;
            }
        }
    }

    private static string GetDayFormat( string category, CultureInfo culture, ITextLocalizer localizer )
    {
        if ( localizer is null
            || !localizer.TryGetString( culture, $"OrdinalDay{category}", out var pattern )
            || string.IsNullOrEmpty( pattern ) )
        {
            return "%d";
        }

        var placeholderIndex = pattern.IndexOf( "{0}", StringComparison.Ordinal );

        if ( placeholderIndex < 0 || pattern.IndexOf( "{0}", placeholderIndex + 3, StringComparison.Ordinal ) >= 0 )
        {
            return "%d";
        }

        var builder = new StringBuilder();

        for ( var index = 0; index < pattern.Length; index++ )
        {
            if ( index == placeholderIndex )
            {
                // Preserve the native day specifier so month inflection and calendar rules still apply.
                builder.Append( "%d" );
                index += 2;
                continue;
            }

            builder.Append( '\\' );
            builder.Append( pattern[index] );
        }

        return builder.ToString();
    }

    private static string GetDayCategory( int day, CultureInfo culture )
    {
        // Select patterns for days in dates, not general-purpose ordinal numbers or cardinal plurals.
        // https://www.unicode.org/reports/tr35/tr35-dates.html#dayOfMonth
        if ( culture.Equals( CultureInfo.InvariantCulture ) || culture.TwoLetterISOLanguageName == "en" )
        {
            if ( day % 100 is 11 or 12 or 13 )
            {
                return "Other";
            }

            return ( day % 10 ) switch
            {
                1 => "One",
                2 => "Two",
                3 => "Few",
                _ => "Other",
            };
        }

        // These date conventions distinguish the first day only, even when ordinary ordinal rules differ.
        // Spanish: https://www.fundeu.es/recomendacion/como-se-escriben-las-fechas/
        // Italian: https://www.treccani.it/enciclopedia/date-prontuario_(Enciclopedia-dell%27Italiano)/
        // Portuguese: https://www12.senado.leg.br/manualdecomunicacao/estilos/datas
        return culture.TwoLetterISOLanguageName switch
        {
            "es" or "fr" or "it" or "pt" when day == 1 => "One",
            _ => "Other",
        };
    }

    #endregion
}