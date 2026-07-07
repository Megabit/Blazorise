using System;

namespace Blazorise.Reporting;

/// <summary>
/// Creates common report value format definitions.
/// </summary>
public static class ReportFormats
{
    /// <summary>
    /// Creates a plain text format.
    /// </summary>
    public static ReportFormatDefinition Text()
        => new ReportTextFormatDefinition();

    /// <summary>
    /// Creates a number format.
    /// </summary>
    public static ReportFormatDefinition Number( int? decimalPlaces = null, bool useThousandsSeparator = true, ReportNegativeNumberFormat negativeNumberFormat = ReportNegativeNumberFormat.Default, string customFormat = null )
        => new ReportNumberFormatDefinition
        {
            CustomFormat = customFormat,
            DecimalPlaces = decimalPlaces,
            UseThousandsSeparator = useThousandsSeparator,
            NegativeNumberFormat = negativeNumberFormat,
        };

    /// <summary>
    /// Creates a currency format.
    /// </summary>
    public static ReportFormatDefinition Currency( int? decimalPlaces = null, string currencySymbol = null, ReportNegativeNumberFormat negativeNumberFormat = ReportNegativeNumberFormat.Default, string customFormat = null )
        => new ReportCurrencyFormatDefinition
        {
            CustomFormat = customFormat,
            DecimalPlaces = decimalPlaces,
            CurrencySymbol = currencySymbol,
            NegativeNumberFormat = negativeNumberFormat,
        };

    /// <summary>
    /// Creates a percent format.
    /// </summary>
    public static ReportFormatDefinition Percent( int? decimalPlaces = null, ReportNegativeNumberFormat negativeNumberFormat = ReportNegativeNumberFormat.Default, string customFormat = null )
        => new ReportPercentFormatDefinition
        {
            CustomFormat = customFormat,
            DecimalPlaces = decimalPlaces,
            NegativeNumberFormat = negativeNumberFormat,
        };

    /// <summary>
    /// Creates a date format.
    /// </summary>
    public static ReportFormatDefinition Date( ReportDateFormat dateFormat = ReportDateFormat.ShortDate, string customFormat = null )
        => new ReportDateFormatDefinition
        {
            CustomFormat = customFormat,
            DateFormat = dateFormat,
        };

    /// <summary>
    /// Creates a time format.
    /// </summary>
    public static ReportFormatDefinition Time( ReportDateFormat dateFormat = ReportDateFormat.ShortTime, string customFormat = null )
        => new ReportTimeFormatDefinition
        {
            CustomFormat = customFormat,
            DateFormat = dateFormat,
        };

    /// <summary>
    /// Creates a date and time format.
    /// </summary>
    public static ReportFormatDefinition DateTime( ReportDateFormat dateFormat = ReportDateFormat.ShortDateTime, string customFormat = null )
        => new ReportDateTimeFormatDefinition
        {
            CustomFormat = customFormat,
            DateFormat = dateFormat,
        };

    /// <summary>
    /// Creates a boolean format.
    /// </summary>
    public static ReportFormatDefinition Boolean( string trueText = null, string falseText = null )
        => new ReportBooleanFormatDefinition
        {
            TrueText = trueText,
            FalseText = falseText,
        };

    /// <summary>
    /// Creates a custom .NET format.
    /// </summary>
    public static ReportFormatDefinition Custom( string format )
        => new ReportCustomFormatDefinition
        {
            Format = format,
        };

    /// <summary>
    /// Creates a copy of an existing report format definition.
    /// </summary>
    public static ReportFormatDefinition Clone( ReportFormatDefinition format )
    {
        if ( format is null )
            return null;

        return format switch
        {
            ReportNumberFormatDefinition numberFormat => new ReportNumberFormatDefinition
            {
                CultureName = numberFormat.CultureName,
                CustomFormat = numberFormat.CustomFormat,
                DecimalPlaces = numberFormat.DecimalPlaces,
                NegativeNumberFormat = numberFormat.NegativeNumberFormat,
                UseThousandsSeparator = numberFormat.UseThousandsSeparator,
            },
            ReportCurrencyFormatDefinition currencyFormat => new ReportCurrencyFormatDefinition
            {
                CultureName = currencyFormat.CultureName,
                CustomFormat = currencyFormat.CustomFormat,
                DecimalPlaces = currencyFormat.DecimalPlaces,
                NegativeNumberFormat = currencyFormat.NegativeNumberFormat,
                CurrencySymbol = currencyFormat.CurrencySymbol,
            },
            ReportPercentFormatDefinition percentFormat => new ReportPercentFormatDefinition
            {
                CultureName = percentFormat.CultureName,
                CustomFormat = percentFormat.CustomFormat,
                DecimalPlaces = percentFormat.DecimalPlaces,
                NegativeNumberFormat = percentFormat.NegativeNumberFormat,
            },
            ReportDateFormatDefinition dateFormat => new ReportDateFormatDefinition
            {
                CultureName = dateFormat.CultureName,
                CustomFormat = dateFormat.CustomFormat,
                DateFormat = dateFormat.DateFormat,
            },
            ReportTimeFormatDefinition timeFormat => new ReportTimeFormatDefinition
            {
                CultureName = timeFormat.CultureName,
                CustomFormat = timeFormat.CustomFormat,
                DateFormat = timeFormat.DateFormat,
            },
            ReportDateTimeFormatDefinition dateTimeFormat => new ReportDateTimeFormatDefinition
            {
                CultureName = dateTimeFormat.CultureName,
                CustomFormat = dateTimeFormat.CustomFormat,
                DateFormat = dateTimeFormat.DateFormat,
            },
            ReportBooleanFormatDefinition booleanFormat => new ReportBooleanFormatDefinition
            {
                CultureName = booleanFormat.CultureName,
                CustomFormat = booleanFormat.CustomFormat,
                TrueText = booleanFormat.TrueText,
                FalseText = booleanFormat.FalseText,
            },
            ReportCustomFormatDefinition customFormat => new ReportCustomFormatDefinition
            {
                CultureName = customFormat.CultureName,
                CustomFormat = customFormat.CustomFormat,
                Format = customFormat.Format,
            },
            ReportTextFormatDefinition textFormat => new ReportTextFormatDefinition
            {
                CultureName = textFormat.CultureName,
                CustomFormat = textFormat.CustomFormat,
            },
            _ => throw new NotSupportedException( $"Unsupported report format definition '{format.GetType().FullName}'." ),
        };
    }
}