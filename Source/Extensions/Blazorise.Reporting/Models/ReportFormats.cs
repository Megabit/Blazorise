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
    public static ReportFormatDefinition Number( int? decimalPlaces = null, bool useThousandsSeparator = true, ReportNegativeNumberFormat negativeNumberFormat = ReportNegativeNumberFormat.Default )
        => new ReportNumberFormatDefinition
        {
            DecimalPlaces = decimalPlaces,
            UseThousandsSeparator = useThousandsSeparator,
            NegativeNumberFormat = negativeNumberFormat,
        };

    /// <summary>
    /// Creates a currency format.
    /// </summary>
    public static ReportFormatDefinition Currency( int? decimalPlaces = null, string currencySymbol = null, ReportNegativeNumberFormat negativeNumberFormat = ReportNegativeNumberFormat.Default )
        => new ReportCurrencyFormatDefinition
        {
            DecimalPlaces = decimalPlaces,
            CurrencySymbol = currencySymbol,
            NegativeNumberFormat = negativeNumberFormat,
        };

    /// <summary>
    /// Creates a percent format.
    /// </summary>
    public static ReportFormatDefinition Percent( int? decimalPlaces = null, ReportNegativeNumberFormat negativeNumberFormat = ReportNegativeNumberFormat.Default )
        => new ReportPercentFormatDefinition
        {
            DecimalPlaces = decimalPlaces,
            NegativeNumberFormat = negativeNumberFormat,
        };

    /// <summary>
    /// Creates a date format.
    /// </summary>
    public static ReportFormatDefinition Date( ReportDateFormat dateFormat = ReportDateFormat.ShortDate )
        => new ReportDateFormatDefinition
        {
            DateFormat = dateFormat,
        };

    /// <summary>
    /// Creates a time format.
    /// </summary>
    public static ReportFormatDefinition Time( ReportDateFormat dateFormat = ReportDateFormat.ShortTime )
        => new ReportTimeFormatDefinition
        {
            DateFormat = dateFormat,
        };

    /// <summary>
    /// Creates a date and time format.
    /// </summary>
    public static ReportFormatDefinition DateTime( ReportDateFormat dateFormat = ReportDateFormat.ShortDateTime )
        => new ReportDateTimeFormatDefinition
        {
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
                DecimalPlaces = numberFormat.DecimalPlaces,
                NegativeNumberFormat = numberFormat.NegativeNumberFormat,
                UseThousandsSeparator = numberFormat.UseThousandsSeparator,
            },
            ReportCurrencyFormatDefinition currencyFormat => new ReportCurrencyFormatDefinition
            {
                CultureName = currencyFormat.CultureName,
                DecimalPlaces = currencyFormat.DecimalPlaces,
                NegativeNumberFormat = currencyFormat.NegativeNumberFormat,
                CurrencySymbol = currencyFormat.CurrencySymbol,
            },
            ReportPercentFormatDefinition percentFormat => new ReportPercentFormatDefinition
            {
                CultureName = percentFormat.CultureName,
                DecimalPlaces = percentFormat.DecimalPlaces,
                NegativeNumberFormat = percentFormat.NegativeNumberFormat,
            },
            ReportDateFormatDefinition dateFormat => new ReportDateFormatDefinition
            {
                CultureName = dateFormat.CultureName,
                DateFormat = dateFormat.DateFormat,
            },
            ReportTimeFormatDefinition timeFormat => new ReportTimeFormatDefinition
            {
                CultureName = timeFormat.CultureName,
                DateFormat = timeFormat.DateFormat,
            },
            ReportDateTimeFormatDefinition dateTimeFormat => new ReportDateTimeFormatDefinition
            {
                CultureName = dateTimeFormat.CultureName,
                DateFormat = dateTimeFormat.DateFormat,
            },
            ReportBooleanFormatDefinition booleanFormat => new ReportBooleanFormatDefinition
            {
                CultureName = booleanFormat.CultureName,
                TrueText = booleanFormat.TrueText,
                FalseText = booleanFormat.FalseText,
            },
            ReportCustomFormatDefinition customFormat => new ReportCustomFormatDefinition
            {
                CultureName = customFormat.CultureName,
                Format = customFormat.Format,
            },
            ReportTextFormatDefinition textFormat => new ReportTextFormatDefinition
            {
                CultureName = textFormat.CultureName,
            },
            _ => new ReportTextFormatDefinition
            {
                CultureName = format.CultureName,
            },
        };
    }
}