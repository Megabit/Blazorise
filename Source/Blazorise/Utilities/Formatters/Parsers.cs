#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Various methods for parsing dates and times.
/// </summary>
public static class Parsers
{
    /// <summary>
    /// Internal date format. Compatible with HTML date inputs.
    /// </summary>
    public const string InternalDateFormat = "yyyy-MM-dd";

    /// <summary>
    /// Internal date-time format. Compatible with HTML date inputs.
    /// </summary>
    public const string InternalDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";

    /// <summary>
    /// Internal month format. Compatible with HTML date inputs.
    /// </summary>
    public const string InternalMonthFormat = "yyyy-MM";

    /// <summary>
    /// Default date format.
    /// </summary>
    public const string ExternalDateFormat = "dd.MM.yyyy";

    /// <summary>
    /// Default date format.
    /// </summary>
    public const string ExternalDateTimeFormat = "dd.MM.yyyy HH:mm";

    /// <summary>
    /// Default month format.
    /// </summary>
    public const string ExternalMonthFormat = "MM.yyyy";

    /// <summary>
    /// Internal time format. Compatible with HTML time inputs.
    /// </summary>
    public const string InternalTimeFormat = "HH\\:mm\\:ss";

    /// <summary>
    /// Possible date formats.
    /// </summary>
    public static readonly string[] SupportedParseDateFormats = new string[]
    {
        InternalDateFormat,
        ExternalDateFormat,
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-ddTHH:mm:ss",
        CultureInfo.InvariantCulture.DateTimeFormat.LongDatePattern,
        CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern,
        "o", // a string representing UTC
    };

    /// <summary>
    /// Possible date-time formats.
    /// </summary>
    public static readonly string[] SupportedParseDateTimeFormats = new string[]
    {
        InternalDateTimeFormat,
        ExternalDateTimeFormat,
        "yyyy-MM-ddTHH:mm",
        CultureInfo.InvariantCulture.DateTimeFormat.LongDatePattern,
        CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern,
        "o", // a string representing UTC
    };

    /// <summary>
    /// Possible month formats.
    /// </summary>
    public static readonly string[] SupportedParseMonthFormats = new string[]
    {
        InternalMonthFormat,
        ExternalMonthFormat,
        "yyyy-MM-dd",
        CultureInfo.InvariantCulture.DateTimeFormat.LongDatePattern,
        CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern,
        "o", // a string representing UTC
    };

    /// <summary>
    /// Possible time formats.
    /// </summary>
    public static readonly string[] SupportedParseTimeSpanTimeFormats = new string[]
    {
        InternalTimeFormat,
        InternalTimeFormat.ToLowerInvariant(), // TimeSpan has a slightly diferent format for hours part
        "hh\\:mm",
        CultureInfo.InvariantCulture.DateTimeFormat.LongTimePattern,
        CultureInfo.InvariantCulture.DateTimeFormat.ShortTimePattern,
    };

    /// <summary>
    /// Possible time formats.
    /// </summary>
    public static readonly string[] SupportedParseTimeOnlyTimeFormats = new string[]
    {
        InternalTimeFormat,
        "HH\\:mm",
        CultureInfo.InvariantCulture.DateTimeFormat.LongTimePattern,
        CultureInfo.InvariantCulture.DateTimeFormat.ShortTimePattern,
    };

    /// <summary>
    /// Possible time formats.
    /// </summary>
    public static readonly string[] SupportedParseDateTimeTimeFormats = new string[]
    {
        InternalTimeFormat,
        InternalTimeFormat.ToLowerInvariant(), // TimeSpan has a slightly diferent format for hours part
        "hh\\:mm",
        CultureInfo.InvariantCulture.DateTimeFormat.LongTimePattern,
        CultureInfo.InvariantCulture.DateTimeFormat.ShortTimePattern,
    };

    /// <summary>
    /// Gets the internal input date format based on the input mode.
    /// </summary>
    /// <param name="inputMode">Input mode.</param>
    /// <returns>Valid date format.</returns>
    public static string GetInternalDateFormat( DateInputMode inputMode )
    {
        return inputMode switch
        {
            DateInputMode.DateTime => InternalDateTimeFormat,
            DateInputMode.Month => InternalMonthFormat,
            _ => InternalDateFormat,
        };
    }

    private static string[] GetSupportedParseDateFormats( DateInputMode inputMode )
    {
        return inputMode switch
        {
            DateInputMode.DateTime => SupportedParseDateTimeFormats,
            DateInputMode.Month => SupportedParseMonthFormats,
            _ => SupportedParseDateFormats,
        };
    }

    /// <summary>
    /// Tries to parse a date from a given string value.
    /// </summary>
    /// <typeparam name="TValue">The type of object to return.</typeparam>
    /// <param name="value">String value to parse.</param>
    /// <param name="inputMode">Hint for parsing method.</param>
    /// <param name="result">An object whose value represents the parsed string.</param>
    /// <returns>True if parsing was successful.</returns>
    public static bool TryParseDate<TValue>( string value, DateInputMode inputMode, out TValue result )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
        {
            result = default;
            return false;
        }

        var supportedParseFormats = GetSupportedParseDateFormats( inputMode );

        var type = Nullable.GetUnderlyingType( typeof( TValue ) ) ?? typeof( TValue );

        if ( type == typeof( DateTime ) && DateTime.TryParseExact( value, supportedParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDt ) )
        {
            result = (TValue)(object)parsedDt;
            return true;
        }

        if ( type == typeof( DateTime ) && DateTimeOffset.TryParse( value, out var parsedDto ) )
        {
            result = (TValue)(object)parsedDto.DateTime;
            return true;
        }

        if ( type == typeof( DateOnly ) && DateOnly.TryParse( value, out var parsedDto3 ) )
        {
            result = (TValue)(object)parsedDto3;
            return true;
        }

        if ( type == typeof( DateTimeOffset ) && DateTimeOffset.TryParse( value, out var parsedDto2 ) )
        {
            result = (TValue)(object)parsedDto2;
            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    /// Tries to parse a time from a given string value.
    /// </summary>
    /// <typeparam name="TValue">The type of object to return.</typeparam>
    /// <param name="value">String to parse.</param>
    /// <param name="result">An object whose value represents the parsed string.</param>
    /// <returns>True if parsing was successful.</returns>
    public static bool TryParseTime<TValue>( string value, out TValue result )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
        {
            result = default;
            return false;
        }

        var type = Nullable.GetUnderlyingType( typeof( TValue ) ) ?? typeof( TValue );

        if ( type == typeof( TimeSpan ) && TimeSpan.TryParseExact( value, SupportedParseTimeSpanTimeFormats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out var timeSpan ) )
        {
            result = (TValue)(object)timeSpan;
            return true;
        }

        if ( type == typeof( TimeOnly ) && TimeOnly.TryParseExact( value, SupportedParseTimeOnlyTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeOnly ) )
        {
            result = (TValue)(object)timeOnly;
            return true;
        }

        if ( type == typeof( DateTime ) && DateTime.TryParseExact( value, SupportedParseDateTimeTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime ) )
        {
            result = (TValue)(object)dateTime;
            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    /// Tries to parse a string value into a date or date-related type based on the specified conversion type and input mode.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="conversionType">The type to which the string should be converted.</param>
    /// <param name="inputMode">The date input mode that determines the supported date formats.</param>
    /// <param name="result">The parsed result, or the default value if parsing fails.</param>
    /// <returns>True if parsing is successful; otherwise, false.</returns>
    public static bool TryParseDate( string value, Type conversionType, DateInputMode inputMode, out object result )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
        {
            result = default;
            return false;
        }

        var supportedParseFormats = GetSupportedParseDateFormats( inputMode );

        var type = Nullable.GetUnderlyingType( conversionType ) ?? conversionType;

        if ( type == typeof( DateTime ) && DateTime.TryParseExact( value, supportedParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDt ) )
        {
            result = parsedDt;
            return true;
        }

        if ( type == typeof( DateTime ) && DateTimeOffset.TryParse( value, out var parsedDto ) )
        {
            result = parsedDto.DateTime;
            return true;
        }

        if ( type == typeof( DateOnly ) && DateOnly.TryParse( value, out var parsedDto3 ) )
        {
            result = parsedDto3;
            return true;
        }

        if ( type == typeof( DateTimeOffset ) && DateTimeOffset.TryParse( value, out var parsedDto2 ) )
        {
            result = parsedDto2;
            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    /// Parses a comma-separated string of dates into a readonly list or array of the specified type.
    /// </summary>
    /// <typeparam name="TValue">The target type, either an array or IReadOnlyList.</typeparam>
    /// <param name="csv">A string containing the date values separated by the specified delimiter.</param>
    /// <param name="delimiter">The delimiter used to separate the values in the string.</param>
    /// <param name="inputMode">The date input mode to determine supported date formats.</param>
    /// <returns>A readonly list or array containing the parsed date values.</returns>
    /// <exception cref="ArgumentException">Thrown if the target type is not an array or IReadOnlyList.</exception>
    public static TValue ParseCsvDatesToReadOnlyList<TValue>( string csv, string delimiter, DateInputMode inputMode )
    {
        var targetType = typeof( TValue );

        Type elementType;
        bool isArray = false;

        if ( targetType.IsArray )
        {
            isArray = true;
            elementType = targetType.GetElementType();
        }
        else if ( targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof( IReadOnlyList<> ) )
        {
            elementType = targetType.GetGenericArguments().Single();
        }
        else
        {
            throw new ArgumentException( "The target type must be either an array or a generic IReadOnlyList.", nameof( targetType ) );
        }

        var multipleValues = csv
            .Split( delimiter, StringSplitOptions.None )
            .Select( val =>
            {
                if ( TryParseDate( val, elementType, inputMode, out var newValue ) )
                    return newValue;

                return Activator.CreateInstance( elementType );
            } ).ToList();

        if ( isArray )
        {
            var array = Array.CreateInstance( elementType, multipleValues.Count );
            for ( int i = 0; i < multipleValues.Count; i++ )
            {
                array.SetValue( multipleValues[i], i );
            }

            return (TValue)(object)array;
        }
        else
        {
            var castedValues = typeof( Enumerable )
                .GetMethod( "Cast" )
                .MakeGenericMethod( elementType )
                .Invoke( null, new object[] { multipleValues } );

            var typedList = typeof( Enumerable )
                .GetMethod( "ToList" )
                .MakeGenericMethod( elementType )
                .Invoke( null, new object[] { castedValues } );

            var readOnlyListType = typeof( ReadOnlyCollection<> ).MakeGenericType( elementType );
            var readOnlyList = Activator.CreateInstance( readOnlyListType, typedList );

            return (TValue)readOnlyList;
        }
    }
}