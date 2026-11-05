#region Using directives
using System.Globalization;
using Blazorise.Localization;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Supplies date names from the same resources used by the date picker calendar.
/// </summary>
internal static class DatePickerCulture
{
    #region Methods

    /// <summary>
    /// Creates a formatting culture with localized names without changing the supplied culture.
    /// </summary>
    public static CultureInfo Resolve( CultureInfo culture, ITextLocalizer localizer )
    {
        culture ??= CultureInfo.CurrentCulture;

        if ( localizer is null )
        {
            return culture;
        }

        var localizedCulture = (CultureInfo)culture.Clone();
        var dateTimeFormat = localizedCulture.DateTimeFormat;
        var invariantFormat = CultureInfo.InvariantCulture.DateTimeFormat;

        dateTimeFormat.DayNames = LocalizeNames( dateTimeFormat.DayNames, invariantFormat.DayNames, culture, localizer );
        dateTimeFormat.AbbreviatedDayNames = LocalizeNames( dateTimeFormat.AbbreviatedDayNames, invariantFormat.AbbreviatedDayNames, culture, localizer );

        // The month resource keys describe the Gregorian calendar only.
        if ( dateTimeFormat.Calendar is GregorianCalendar )
        {
            var monthKeys = invariantFormat.MonthNames;
            monthKeys[4] = "May!";

            dateTimeFormat.MonthNames = LocalizeNames( dateTimeFormat.MonthNames, monthKeys, culture, localizer );
            dateTimeFormat.MonthGenitiveNames = LocalizeNames( dateTimeFormat.MonthGenitiveNames, monthKeys, culture, localizer );
            dateTimeFormat.AbbreviatedMonthNames = LocalizeNames( dateTimeFormat.AbbreviatedMonthNames, invariantFormat.AbbreviatedMonthNames, culture, localizer );
            dateTimeFormat.AbbreviatedMonthGenitiveNames = LocalizeNames( dateTimeFormat.AbbreviatedMonthGenitiveNames, invariantFormat.AbbreviatedMonthNames, culture, localizer );
        }

        if ( localizer.TryGetString( culture, "AM", out var amDesignator ) && !string.IsNullOrEmpty( amDesignator ) )
        {
            dateTimeFormat.AMDesignator = amDesignator;
        }

        if ( localizer.TryGetString( culture, "PM", out var pmDesignator ) && !string.IsNullOrEmpty( pmDesignator ) )
        {
            dateTimeFormat.PMDesignator = pmDesignator;
        }

        return localizedCulture;
    }

    private static string[] LocalizeNames( string[] names, string[] keys, CultureInfo culture, ITextLocalizer localizer )
    {
        for ( var index = 0; index < keys.Length; index++ )
        {
            if ( !string.IsNullOrEmpty( keys[index] )
                && localizer.TryGetString( culture, keys[index], out var translation )
                && !string.IsNullOrEmpty( translation ) )
            {
                names[index] = translation;
            }
        }

        return names;
    }

    #endregion
}