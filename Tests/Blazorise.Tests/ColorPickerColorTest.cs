#region Using directives
using System.Globalization;
using Xunit;
#endregion

namespace Blazorise.Tests;

public class ColorPickerColorTest
{
    [Theory]
    [InlineData( "#f00", "#FF0000" )]
    [InlineData( "#0f08", "#00FF0088" )]
    [InlineData( "#6200ea", "#6200EA" )]
    [InlineData( "#00ff0080", "#00FF0080" )]
    [InlineData( "rgba(255, 0, 0, 0.5)", "#FF000080" )]
    [InlineData( "  rgba(255, 0, 0, 0.5)  ", "#FF000080" )]
    [InlineData( "rgb(100% 0% 0% / 50%)", "#FF000080" )]
    [InlineData( "hsl(120 100% 50%)", "#00FF00" )]
    [InlineData( "blue", "#0000FF" )]
    [InlineData( "#000000", "#000000" )]
    [InlineData( "#ffffff", "#FFFFFF" )]
    public void Parsing_ShouldRoundTripLiteralColors( string input, string expected )
    {
        Assert.True( ColorPickerColor.TryParse( input, out var color ) );
        Assert.Equal( expected, color.ToHexString() );
    }

    [Theory]
    [InlineData( null )]
    [InlineData( "" )]
    [InlineData( "#xyz" )]
    [InlineData( "rgba(255, 0, 0, NaN)" )]
    [InlineData( "var(--brand-color)" )]
    public void Parsing_ShouldRejectUnresolvableColors( string input )
    {
        Assert.False( ColorPickerColor.TryParse( input, out _ ) );
    }

    [Fact]
    public void Formatting_ShouldUseInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo( "hr-HR" );
            var color = new ColorPickerColor( 0, 100, 100, 0.5 );

            Assert.Equal( "rgba(255,0,0,0.5)", color.ToRgbaString() );
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}