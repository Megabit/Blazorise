using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.Utils;

public class ColorContrastTest
{
    [Theory]
    [InlineData( "#123", 255, 17, 34, 51 )]
    [InlineData( "#1234", 68, 17, 34, 51 )]
    [InlineData( "#DBB5E680", 128, 219, 181, 230 )]
    [InlineData( "RGB(100% 0% 0% / 50%)", 127, 255, 0, 0 )]
    [InlineData( "hsl(120,100%,50%)", 255, 0, 255, 0 )]
    [InlineData( "hsl(.5turn 100% 50%)", 255, 0, 255, 255 )]
    [InlineData( "hsla(240deg,100%,50%,.5)", 127, 0, 0, 255 )]
    [InlineData( "hsl(-60deg 100% 50%)", 255, 255, 0, 255 )]
    [InlineData( "hsl(400grad 100% 50%)", 255, 255, 0, 0 )]
    [InlineData( "hsl(60deg 150% 50%)", 255, 255, 255, 0 )]
    [InlineData( "hsl(0 100% 150%)", 255, 255, 255, 255 )]
    public void LiteralColors_ParseWithoutBrowserState( string value, int alpha, int red, int green, int blue )
    {
        Assert.True( HtmlColorCodeParser.TryParse( value, out System.Drawing.Color color ) );
        Assert.Equal( System.Drawing.Color.FromArgb( alpha, red, green, blue ), color );
    }

    [Theory]
    [InlineData( "var(--accent)" )]
    [InlineData( "currentColor" )]
    [InlineData( "#xyz" )]
    [InlineData( "rgb(NaN 0 0)" )]
    [InlineData( "hsl(0 100% NaN%)" )]
    public void UnresolvedColors_AreNotGuessed( string value )
    {
        Assert.False( HtmlColorCodeParser.TryParse( value, out _ ) );
    }

    [Theory]
    [InlineData( "#123", 1, 2, 3 )]
    [InlineData( "#123456", 18, 52, 86 )]
    [InlineData( "Violet", 141, 56, 201 )]
    public void LegacyRgbParser_PreservesExistingHexAndNamedColors( string value, byte red, byte green, byte blue )
    {
        Assert.True( HtmlColorCodeParser.TryParse( value, out byte actualRed, out byte actualGreen, out byte actualBlue ) );
        Assert.Equal( red, actualRed );
        Assert.Equal( green, actualGreen );
        Assert.Equal( blue, actualBlue );
    }

    [Fact]
    public void LiteralParser_UsesStandardNamedColors()
    {
        Assert.True( HtmlColorCodeParser.TryParse( "Violet", out System.Drawing.Color color ) );
        Assert.Equal( System.Drawing.Color.Violet, color );
    }

    [Fact]
    public void Contrast_UsesThemeThresholdAndSupportsAnExplicitOverride()
    {
        Theme theme = new() { Black = "#101010", White = "#eeeeee", LuminanceThreshold = 200 };
        System.Drawing.Color background = System.Drawing.Color.FromArgb( 180, 180, 180 );

        Assert.Equal( System.Drawing.Color.FromArgb( 238, 238, 238 ), ThemeGenerator.Contrast( theme, background ) );
        Assert.Equal( System.Drawing.Color.FromArgb( 16, 16, 16 ), ThemeGenerator.Contrast( theme, background, 150 ) );
    }

    [Fact]
    public void AccessibleContrast_PreservesThemePreferenceWhenReadable()
    {
        Theme theme = new() { Black = "#101010", White = "#eeeeee" };
        Assert.Equal( System.Drawing.Color.FromArgb( 16, 16, 16 ), ThemeGenerator.GetAccessibleContrastColor( theme, System.Drawing.Color.White ) );
        Assert.Equal( System.Drawing.Color.FromArgb( 238, 238, 238 ), ThemeGenerator.GetAccessibleContrastColor( theme, System.Drawing.Color.Black ) );
    }

    [Fact]
    public void AccessibleContrast_FallsBackWhenPreferredThemeColorsAreTooSimilar()
    {
        Theme theme = new() { Black = "#999999", White = "#aaaaaa" };
        Assert.Equal( System.Drawing.Color.Black, ThemeGenerator.GetAccessibleContrastColor( theme, System.Drawing.Color.White ) );
    }

    [Fact]
    public void AccessibleContrast_AccountsForForegroundOpacity()
    {
        Theme theme = new() { Black = "#00000020", White = "#ffffff" };
        Assert.Equal( System.Drawing.Color.Black, ThemeGenerator.GetAccessibleContrastColor( theme, System.Drawing.Color.White ) );
    }
}