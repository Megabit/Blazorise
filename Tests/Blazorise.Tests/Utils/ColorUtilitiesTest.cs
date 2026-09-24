using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.Utils;

public class ColorUtilitiesTest
{
    [Fact]
    public void LuminanceAndContrastRatio_PreserveTheirScales()
    {
        Assert.Equal( 0d, ColorUtilities.LuminanceFromColor( System.Drawing.Color.Black ), 6 );
        Assert.Equal( 100d, ColorUtilities.LuminanceFromColor( System.Drawing.Color.White ), 6 );
        Assert.Equal( 21d, ColorUtilities.GetContrastRatio( System.Drawing.Color.Black, System.Drawing.Color.White ), 6 );
        Assert.Equal( 1d, ColorUtilities.GetContrastRatio( System.Drawing.Color.White, System.Drawing.Color.White ), 6 );
    }

    [Fact]
    public void BlendAndMix_PreserveTheirDifferentRounding()
    {
        Assert.Equal( System.Drawing.Color.FromArgb( 63, 63, 63 ), ColorUtilities.Blend( System.Drawing.Color.White, System.Drawing.Color.Black, 25 ) );
        Assert.Equal( System.Drawing.Color.FromArgb( 64, 64, 64 ), ColorUtilities.Mix( System.Drawing.Color.White, System.Drawing.Color.Black, 25 ) );
    }

    [Fact]
    public void Mix_WeightsRgbChannelsByAlpha()
    {
        System.Drawing.Color transparentRed = System.Drawing.Color.FromArgb( 0, 255, 0, 0 );

        Assert.Equal( System.Drawing.Color.FromArgb( 128, 0, 0, 255 ), ColorUtilities.Mix( transparentRed, System.Drawing.Color.Blue, 50 ) );
    }

    [Fact]
    public void Brightness_PreservesAlphaAndTruncatesChannels()
    {
        System.Drawing.Color color = System.Drawing.Color.FromArgb( 128, 100, 150, 200 );

        Assert.Equal( System.Drawing.Color.FromArgb( 128, 177, 202, 227 ), ColorUtilities.Lighten( color, 50 ) );
        Assert.Equal( System.Drawing.Color.FromArgb( 128, 75, 112, 150 ), ColorUtilities.Darken( color, 25 ) );
    }

    [Fact]
    public void HexFormatting_RoundTripsTrailingAlpha()
    {
        System.Drawing.Color color = System.Drawing.Color.FromArgb( 128, 18, 52, 86 );

        Assert.Equal( "#12345680", ColorUtilities.ToHex( color ) );
        Assert.True( HtmlColorCodeParser.TryParse( ColorUtilities.ToHex( color ), out System.Drawing.Color parsed ) );
        Assert.Equal( color, parsed );
        Assert.Equal( "#123456", ColorUtilities.ToHex( System.Drawing.Color.FromArgb( 18, 52, 86 ) ) );
        Assert.Equal( "#123456FF", ColorUtilities.ToHexRGBA( System.Drawing.Color.FromArgb( 18, 52, 86 ) ) );
    }

    [Fact]
    public void HslConversion_RoundTripsCyan()
    {
        HslColor hsl = HslColor.FromColor( System.Drawing.Color.Cyan );

        Assert.Equal( 180d, hsl.Hue );
        Assert.Equal( 100d, hsl.Saturation );
        Assert.Equal( 50d, hsl.Luminosity );
        Assert.Equal( "#00FFFF", ColorUtilities.ToHex( hsl ) );
    }

    [Fact]
    public void Contrast_AcceptsForegroundsAndThresholdWithoutATheme()
    {
        System.Drawing.Color background = System.Drawing.Color.FromArgb( 180, 180, 180 );
        System.Drawing.Color dark = System.Drawing.Color.FromArgb( 16, 16, 16 );
        System.Drawing.Color light = System.Drawing.Color.FromArgb( 238, 238, 238 );

        Assert.Equal( light, ColorUtilities.Contrast( background, dark, light, 200 ) );
        Assert.Equal( dark, ColorUtilities.Contrast( background, dark, light, 150 ) );
        Assert.Equal( dark, ColorUtilities.GetAccessibleContrastColor( background, dark, light, 150 ) );
        Assert.Equal( System.Drawing.Color.Black, ColorUtilities.GetAccessibleContrastColor( background, dark, light, 200 ) );
    }
}