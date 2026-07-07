#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportFormatResolver
{
    #region Methods

    internal static string FormatValue( object value, ReportFormatDefinition format )
    {
        if ( value is null )
            return string.Empty;

        format ??= ReportFormats.Text();
        CultureInfo culture = ResolveCulture( format );

        return format switch
        {
            ReportNumberFormatDefinition numberFormat => FormatFormattable( value, BuildNumberFormat( numberFormat ), culture ),
            ReportCurrencyFormatDefinition currencyFormat => FormatCurrency( value, currencyFormat, culture ),
            ReportPercentFormatDefinition percentFormat => FormatFormattable( value, BuildPercentFormat( percentFormat ), culture ),
            ReportDateFormatDefinition dateFormat => FormatFormattable( value, BuildDateFormat( dateFormat.DateFormat, dateOnly: true ), culture ),
            ReportTimeFormatDefinition timeFormat => FormatFormattable( value, BuildDateFormat( timeFormat.DateFormat, timeOnly: true ), culture ),
            ReportDateTimeFormatDefinition dateTimeFormat => FormatFormattable( value, BuildDateFormat( dateTimeFormat.DateFormat ), culture ),
            ReportBooleanFormatDefinition booleanFormat => FormatBoolean( value, booleanFormat, culture ),
            ReportCustomFormatDefinition customFormat => FormatFormattable( value, customFormat.Format, culture ),
            _ => Convert.ToString( value, culture ),
        };
    }

    internal static string GetDisplayText( ReportFormatDefinition format )
    {
        if ( format is null || format.Category == ReportFormatCategory.Text )
            return "Text";

        return format switch
        {
            ReportNumberFormatDefinition numberFormat => $"Number ({BuildNumberFormat( numberFormat )})",
            ReportCurrencyFormatDefinition currencyFormat => $"Currency ({BuildCurrencyFormat( currencyFormat )})",
            ReportPercentFormatDefinition percentFormat => $"Percent ({BuildPercentFormat( percentFormat )})",
            ReportDateFormatDefinition dateFormat => $"Date ({BuildDateFormat( dateFormat.DateFormat, dateOnly: true )})",
            ReportTimeFormatDefinition timeFormat => $"Time ({BuildDateFormat( timeFormat.DateFormat, timeOnly: true )})",
            ReportDateTimeFormatDefinition dateTimeFormat => $"Date and time ({BuildDateFormat( dateTimeFormat.DateFormat )})",
            ReportBooleanFormatDefinition => "Boolean",
            ReportCustomFormatDefinition customFormat => string.IsNullOrWhiteSpace( customFormat.Format ) ? "Custom" : $"Custom ({customFormat.Format})",
            _ => format.Category.ToString(),
        };
    }

    private static string FormatCurrency( object value, ReportCurrencyFormatDefinition format, CultureInfo culture )
    {
        CultureInfo currencyCulture = (CultureInfo)culture.Clone();

        if ( !string.IsNullOrWhiteSpace( format.CurrencySymbol ) )
            currencyCulture.NumberFormat.CurrencySymbol = format.CurrencySymbol;

        if ( format.NegativeNumberFormat == ReportNegativeNumberFormat.Parentheses )
            currencyCulture.NumberFormat.CurrencyNegativePattern = 0;

        return FormatFormattable( value, BuildCurrencyFormat( format ), currencyCulture );
    }

    private static string FormatBoolean( object value, ReportBooleanFormatDefinition format, CultureInfo culture )
    {
        if ( value is bool boolValue )
            return boolValue
                ? string.IsNullOrWhiteSpace( format.TrueText ) ? bool.TrueString : format.TrueText
                : string.IsNullOrWhiteSpace( format.FalseText ) ? bool.FalseString : format.FalseText;

        return Convert.ToString( value, culture );
    }

    private static string FormatFormattable( object value, string format, CultureInfo culture )
    {
        if ( string.IsNullOrWhiteSpace( format ) )
            return Convert.ToString( value, culture );

        if ( value is not IFormattable formattable )
            return Convert.ToString( value, culture );

        try
        {
            return formattable.ToString( format, culture );
        }
        catch ( FormatException )
        {
            return Convert.ToString( value, culture );
        }
    }

    private static string BuildNumberFormat( ReportNumberFormatDefinition format )
    {
        if ( format.NegativeNumberFormat == ReportNegativeNumberFormat.Parentheses )
            return BuildCustomNumericFormat( format.UseThousandsSeparator ? "#,##0" : "0", format.DecimalPlaces, percent: false );

        string specifier = format.UseThousandsSeparator ? "N" : "F";
        return format.DecimalPlaces is int decimalPlaces ? $"{specifier}{Math.Max( 0, decimalPlaces )}" : specifier;
    }

    private static string BuildCurrencyFormat( ReportCurrencyFormatDefinition format )
    {
        return format.DecimalPlaces is int decimalPlaces ? $"C{Math.Max( 0, decimalPlaces )}" : "C";
    }

    private static string BuildPercentFormat( ReportPercentFormatDefinition format )
    {
        if ( format.NegativeNumberFormat == ReportNegativeNumberFormat.Parentheses )
            return BuildCustomNumericFormat( "0", format.DecimalPlaces, percent: true );

        return format.DecimalPlaces is int decimalPlaces ? $"P{Math.Max( 0, decimalPlaces )}" : "P";
    }

    private static string BuildCustomNumericFormat( string integerPattern, int? decimalPlaces, bool percent )
    {
        string decimals = decimalPlaces is > 0
            ? "." + new string( '0', decimalPlaces.Value )
            : string.Empty;
        string suffix = percent ? "%" : string.Empty;
        string positive = $"{integerPattern}{decimals}{suffix}";
        string negative = $"({positive})";

        return $"{positive};{negative}";
    }

    private static string BuildDateFormat( ReportDateFormat dateFormat, bool dateOnly = false, bool timeOnly = false )
    {
        if ( dateOnly )
        {
            return dateFormat == ReportDateFormat.LongDate ? "D" : "d";
        }

        if ( timeOnly )
        {
            return dateFormat == ReportDateFormat.LongTime ? "T" : "t";
        }

        return dateFormat switch
        {
            ReportDateFormat.LongDate => "D",
            ReportDateFormat.ShortTime => "t",
            ReportDateFormat.LongTime => "T",
            ReportDateFormat.LongDateTime => "f",
            _ => "g",
        };
    }

    private static CultureInfo ResolveCulture( ReportFormatDefinition format )
    {
        if ( !string.IsNullOrWhiteSpace( format?.CultureName ) )
        {
            try
            {
                return CultureInfo.GetCultureInfo( format.CultureName );
            }
            catch ( CultureNotFoundException )
            {
            }
        }

        return CultureInfo.CurrentCulture;
    }

    #endregion
}