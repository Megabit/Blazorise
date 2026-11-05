#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using Blazorise.Localization;
using Blazorise.Utilities;
using Xunit;
#endregion

namespace Blazorise.Tests.Utils;

public class PickerDateTimeFormatTests
{
    private readonly ITextLocalizer localizer = new TextLocalizer<DatePicker<DateTime>>( new TextLocalizerService() );

    [Theory]
    [InlineData( 1, "1st" )]
    [InlineData( 2, "2nd" )]
    [InlineData( 3, "3rd" )]
    [InlineData( 4, "4th" )]
    [InlineData( 11, "11th" )]
    [InlineData( 12, "12th" )]
    [InlineData( 13, "13th" )]
    [InlineData( 21, "21st" )]
    [InlineData( 22, "22nd" )]
    [InlineData( 23, "23rd" )]
    [InlineData( 31, "31st" )]
    public void OrdinalDaysFormatAndParse( int day, string ordinal )
    {
        var culture = CultureInfo.GetCultureInfo( "en-US" );
        var date = new DateTime( 2026, 12, day, 15, 24, 36 );
        var format = PickerDateTimeFormat.Normalize( "{day:ordinal} MMMM yyyy HH:mm:ss" );
        var expected = $"{ordinal} December 2026 15:24:36";

        Assert.Equal( expected, PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer ) );
        Assert.True( PickerDateTimeFormat.TryParseExact( expected, format, culture, DateTimeStyles.None, out var parsed, localizer ) );
        Assert.Equal( date, parsed );
    }

    [Theory]
    [InlineData( "cs-CZ", 3, "3." )]
    [InlineData( "cs", 1, "1." )]
    [InlineData( "da-DK", 3, "3." )]
    [InlineData( "en-GB", 3, "3rd" )]
    [InlineData( "", 3, "3rd" )]
    [InlineData( "es-ES", 1, "1.º" )]
    [InlineData( "es-MX", 2, "2" )]
    [InlineData( "es-ES", 21, "21" )]
    [InlineData( "fr-FR", 1, "1er" )]
    [InlineData( "fr-FR", 2, "2" )]
    [InlineData( "fr-FR", 21, "21" )]
    [InlineData( "de-DE", 3, "3." )]
    [InlineData( "de-AT", 11, "11." )]
    [InlineData( "hr-HR", 3, "3." )]
    [InlineData( "is-IS", 3, "3." )]
    [InlineData( "it-IT", 1, "1º" )]
    [InlineData( "it-IT", 2, "2" )]
    [InlineData( "it-IT", 21, "21" )]
    [InlineData( "nl-NL", 3, "3" )]
    [InlineData( "pl-PL", 3, "3" )]
    [InlineData( "pt-BR", 1, "1º" )]
    [InlineData( "pt-PT", 2, "2" )]
    [InlineData( "pt-BR", 21, "21" )]
    [InlineData( "ro-RO", 3, "3" )]
    [InlineData( "ru-RU", 3, "3" )]
    [InlineData( "sk-SK", 3, "3." )]
    [InlineData( "tr-TR", 3, "3" )]
    [InlineData( "zh-CN", 3, "3" )]
    [InlineData( "ja-JP", 3, "3" )]
    public void OrdinalCulturePolicyAppliesToFormattingAndParsing( string cultureName, int day, string ordinal )
    {
        var culture = CultureInfo.GetCultureInfo( cultureName );
        var date = new DateTime( 2026, 11, day );
        var format = PickerDateTimeFormat.Normalize( "yyyy-MM-{day:ordinal}" );
        var expected = $"2026-11-{ordinal}";

        Assert.Equal( expected, PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer ) );
        Assert.True( PickerDateTimeFormat.TryParseExact( expected, format, culture, DateTimeStyles.None, out var parsed, localizer ) );
        Assert.Equal( date, parsed );
    }

    [Theory]
    [InlineData( "ja-JP" )]
    [InlineData( "ar-SA" )]
    public void OrdinalFallbackPreservesNativeMonthNamesAndCalendarDays( string cultureName )
    {
        var culture = CultureInfo.GetCultureInfo( cultureName );
        var date = new DateTime( 2026, 11, 3 );
        var format = PickerDateTimeFormat.Normalize( "{day:ordinal} MMMM yyyy" );
        var expected = date.ToString( "d MMMM yyyy", culture );

        Assert.Equal( expected, PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer ) );
        Assert.True( PickerDateTimeFormat.TryParseExact( expected, format, culture, DateTimeStyles.None, out var parsed, localizer ) );
        Assert.Equal( date, parsed );
    }

    [Theory]
    [InlineData( 11, "dddd {day:ordinal}, MMMM yyyy", "Srijeda 4., Studeni 2026" )]
    [InlineData( 11, "dddd d MMMM yyyy", "Srijeda 4 Studeni 2026" )]
    [InlineData( 11, "ddd d MMM yyyy", "Sri 4 Stu 2026" )]
    [InlineData( 5, "dddd {day:ordinal}, MMMM yyyy", "Ponedjeljak 4., Svibanj 2026" )]
    [InlineData( 5, "ddd d MMM yyyy", "Pon 4 Svi 2026" )]
    public void LocalizedNamesDoNotDependOnRuntimeDateNames( int month, string displayFormat, string expected )
    {
        // setup
        var culture = (CultureInfo)CultureInfo.GetCultureInfo( "hr-HR" ).Clone();
        culture.DateTimeFormat = (DateTimeFormatInfo)CultureInfo.GetCultureInfo( "en-US" ).DateTimeFormat.Clone();
        var date = new DateTime( 2026, month, 4 );
        var format = PickerDateTimeFormat.Normalize( displayFormat );

        // test
        var formatted = PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer );
        var parsedSuccessfully = PickerDateTimeFormat.TryParseExact( formatted, format, culture, DateTimeStyles.None, out var parsed, localizer );

        // validate
        Assert.Equal( expected, formatted );
        Assert.True( parsedSuccessfully );
        Assert.Equal( date, parsed );
        Assert.Equal( "Wednesday", culture.DateTimeFormat.DayNames[(int)DayOfWeek.Wednesday] );
        Assert.Equal( "November", culture.DateTimeFormat.MonthNames[10] );
        Assert.Equal( "May", CultureInfo.InvariantCulture.DateTimeFormat.MonthNames[4] );
    }

    [Theory]
    [InlineData( "1th November 2026" )]
    [InlineData( "2rd November 2026" )]
    [InlineData( "11st November 2026" )]
    [InlineData( "12nd November 2026" )]
    [InlineData( "13rd November 2026" )]
    [InlineData( "31st November 2026" )]
    public void InvalidOrdinalDatesAreRejected( string text )
    {
        var format = PickerDateTimeFormat.Normalize( "{day:ordinal} MMMM yyyy" );

        Assert.False( PickerDateTimeFormat.TryParseExact( text, format, CultureInfo.GetCultureInfo( "en-US" ), DateTimeStyles.None, out _, localizer ) );
    }

    [Theory]
    [InlineData( "fr-FR", "2026-11-2er" )]
    [InlineData( "es-ES", "2026-11-2.º" )]
    [InlineData( "it-IT", "2026-11-2º" )]
    [InlineData( "pt-BR", "2026-11-2º" )]
    [InlineData( "de-DE", "2026-11-3rd" )]
    public void LocalizedPatternsMustMatchTheParsedDay( string cultureName, string text )
    {
        // setup
        var culture = CultureInfo.GetCultureInfo( cultureName );
        var format = PickerDateTimeFormat.Normalize( "yyyy-MM-{day:ordinal}" );

        // test
        var parsed = PickerDateTimeFormat.TryParseExact( text, format, culture, DateTimeStyles.None, out _, localizer );

        // validate
        Assert.False( parsed );
    }

    [Theory]
    [InlineData( "yyyy-MM-dd '{day:ordinal}'", "2026-11-03 {day:ordinal}" )]
    [InlineData( "yyyy-MM-dd \"{day:ordinal}\"", "2026-11-03 {day:ordinal}" )]
    [InlineData( @"yyyy-MM-dd \{\d\a\y\:\o\r\d\i\n\a\l\}", "2026-11-03 {day:ordinal}" )]
    [InlineData( "yyyy-MM-dd 'it\\'s {day:ordinal}'", "2026-11-03 it's {day:ordinal}" )]
    public void QuotedAndEscapedExtensionsRemainLiteral( string displayFormat, string expected )
    {
        var date = new DateTime( 2026, 11, 3 );
        var culture = CultureInfo.GetCultureInfo( "en-US" );
        var format = PickerDateTimeFormat.Normalize( displayFormat );

        Assert.Equal( expected, PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer ) );
        Assert.True( PickerDateTimeFormat.TryParseExact( expected, format, culture, DateTimeStyles.None, out var parsed, localizer ) );
        Assert.Equal( date, parsed );
    }

    [Theory]
    [InlineData( "{month:ordinal}" )]
    [InlineData( "{hour:ordinal}" )]
    [InlineData( "{day:unknown}" )]
    [InlineData( "{day:ordinal" )]
    public void UnsupportedExtensionsReportAFormatError( string format )
    {
        Assert.Throws<FormatException>( () => PickerDateTimeFormat.Normalize( format ) );
    }

    [Fact]
    public void OrdinalFormattingSupportsDateOnlyAndPreservesDateTimeOffset()
    {
        var culture = CultureInfo.GetCultureInfo( "en-US" );
        var date = new DateOnly( 2026, 11, 3 );
        var offsetDate = new DateTimeOffset( 2026, 11, 3, 0, 15, 0, TimeSpan.FromHours( 2 ) );
        var format = PickerDateTimeFormat.Normalize( "dddd {day:ordinal} MMMM yyyy" );

        Assert.Equal( "Tuesday 3rd November 2026", PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer ) );
        Assert.Equal( "Tuesday 3rd November 2026 00:15 +02:00", PickerDateTimeFormat.FormatDateValue( offsetDate, format + " HH:mm zzz", culture, localizer ) );
        Assert.Null( PickerDateTimeFormat.FormatDateValue( null, format, culture, localizer ) );
    }

    [Fact]
    public void WeekDateDisplayUsesTheOrdinalOfTheWeekStart()
    {
        var format = PickerDateTimeFormat.Normalize( "dddd {day:ordinal} MMMM yyyy" );

        Assert.Equal( "Monday 2nd November 2026", WeekDateFormat.Format( new DateTime( 2026, 11, 3 ), format, CultureInfo.GetCultureInfo( "en-US" ), localizer ) );
    }

    [Fact]
    public void CustomOrdinalPatternsPreserveLiteralAffixesAndRoundTrip()
    {
        // setup
        var culture = CultureInfo.GetCultureInfo( "en-GB" );
        var date = new DateTime( 2026, 11, 2 );
        var format = PickerDateTimeFormat.Normalize( "yyyy-MM-{day:ordinal}" );
        localizer.AddLanguageResource( new TextLocalizationResource
        {
            Culture = "en-GB",
            Translations = new Dictionary<string, string>
            {
                ["OrdinalDayTwo"] = "day '{0}'!",
            },
        } );

        // test
        var formatted = PickerDateTimeFormat.FormatDateValue( date, format, culture, localizer );
        var parsedSuccessfully = PickerDateTimeFormat.TryParseExact( formatted, format, culture, DateTimeStyles.None, out var parsed, localizer );

        // validate
        Assert.Equal( "2026-11-day '2'!", formatted );
        Assert.True( parsedSuccessfully );
        Assert.Equal( date, parsed );
        Assert.False( PickerDateTimeFormat.TryParseExact( "2026-11-day '3'!", format, culture, DateTimeStyles.None, out _, localizer ) );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "missing placeholder" )]
    [InlineData( "{0}{0}" )]
    public void MissingOrInvalidCategoryPatternsUsePlainDayNumbers( string pattern )
    {
        // setup
        var incompleteLocalizer = new TextLocalizer<PickerDateTimeFormatTests>( new TextLocalizerService() );
        var translations = new Dictionary<string, string>
        {
            ["OrdinalDayOther"] = "{0}th",
        };

        if ( pattern is not null )
        {
            translations["OrdinalDayOne"] = pattern;
        }

        incompleteLocalizer.AddLanguageResource( new TextLocalizationResource
        {
            Culture = "en",
            Translations = translations,
        } );
        var date = new DateTime( 2026, 11, 1 );
        var culture = CultureInfo.GetCultureInfo( "en-US" );
        var format = PickerDateTimeFormat.Normalize( "yyyy-MM-{day:ordinal}" );

        // test
        var formatted = PickerDateTimeFormat.FormatDateValue( date, format, culture, incompleteLocalizer );
        var parsedSuccessfully = PickerDateTimeFormat.TryParseExact( formatted, format, culture, DateTimeStyles.None, out var parsed, incompleteLocalizer );

        // validate
        Assert.Equal( "2026-11-1", formatted );
        Assert.True( parsedSuccessfully );
        Assert.Equal( date, parsed );
        Assert.False( PickerDateTimeFormat.TryParseExact( "2026-11-1th", format, culture, DateTimeStyles.None, out _, incompleteLocalizer ) );
    }
}