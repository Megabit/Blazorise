#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using Blazorise.Localization;
using Xunit;
#endregion

namespace Blazorise.Tests.Utils;

public class TextLocalizerTests
{
    [Fact]
    public void EveryDatePickerLanguageProvidesOrdinalPatterns()
    {
        // setup
        var localizer = new TextLocalizer<DatePicker<DateTime>>( new TextLocalizerService() );
        var assembly = typeof( DatePicker<DateTime> ).Assembly;
        const string prefix = "Blazorise.Resources.Localization.DatePicker.";
        var resourceCount = 0;

        // test
        foreach ( var resourceName in assembly.GetManifestResourceNames() )
        {
            if ( !resourceName.StartsWith( prefix, StringComparison.Ordinal ) || !resourceName.EndsWith( ".json", StringComparison.Ordinal ) )
            {
                continue;
            }

            var cultureName = resourceName[prefix.Length..^5];
            var culture = CultureInfo.GetCultureInfo( cultureName );

            // validate
            Assert.True( localizer.TryGetString( culture, "OrdinalDayOther", out var pattern ), resourceName );
            Assert.Contains( "{0}", pattern );
            resourceCount++;
        }

        Assert.True( resourceCount > 0 );
    }

    [Fact]
    public void TryGetStringUsesExactCultureThenParentForEachResource()
    {
        // setup
        var localizer = new TextLocalizer<DatePicker<DateTime>>( new TextLocalizerService() );
        localizer.AddLanguageResource( new TextLocalizationResource
        {
            Culture = "fr-CA",
            Translations = new Dictionary<string, string>
            {
                ["OrdinalDayOne"] = "{0}ᵉʳ",
            },
        } );
        var culture = CultureInfo.GetCultureInfo( "fr-CA" );

        // test
        var foundExact = localizer.TryGetString( culture, "OrdinalDayOne", out var exact );
        var foundParent = localizer.TryGetString( culture, "OrdinalDayOther", out var parent );
        var foundInvariant = localizer.TryGetString( CultureInfo.InvariantCulture, "OrdinalDayFew", out var invariant );

        // validate
        Assert.True( foundExact );
        Assert.Equal( "{0}ᵉʳ", exact );
        Assert.True( foundParent );
        Assert.Equal( "{0}", parent );
        Assert.True( foundInvariant );
        Assert.Equal( "{0}rd", invariant );
    }

    [Theory]
    [InlineData( "ja-JP", "OrdinalDayOne" )]
    [InlineData( "fr-FR", "OrdinalDayTwo" )]
    public void TryGetStringDoesNotUseTheUiCultureOrEnglishFallback( string cultureName, string key )
    {
        // setup
        var originalUICulture = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo( "en-US" );
            var localizer = new TextLocalizer<DatePicker<DateTime>>( new TextLocalizerService() );

            // test
            var found = localizer.TryGetString( CultureInfo.GetCultureInfo( cultureName ), key, out var value );

            // validate
            Assert.False( found );
            Assert.Null( value );
            Assert.Equal( "{0}st", localizer.GetString( CultureInfo.GetCultureInfo( "ja-JP" ), "OrdinalDayOne" ) );
        }
        finally
        {
            CultureInfo.CurrentUICulture = originalUICulture;
        }
    }
}