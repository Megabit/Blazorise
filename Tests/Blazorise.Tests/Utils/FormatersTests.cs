using System;
using System.Globalization;
using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.Utils;

public class FormatersTests
{

    [Theory]
    [InlineData( "C2", "$12.50" )]
    [InlineData( "{0:C2}", "$12.50" )]
    [InlineData( "Amount: {0:C2}", "Amount: $12.50" )]
    public void FormatDisplayValue_Supports_DirectAndCompositeFormats( string displayFormat, string expected )
    {
        var result = Formaters.FormatDisplayValue( 12.5m, displayFormat, CultureInfo.GetCultureInfo( "en-US" ) );

        Assert.Equal( expected, result );
    }

    [Fact]
    public void FormatDisplayValue_Supports_DirectDateFormat()
    {
        var result = Formaters.FormatDisplayValue( new DateTime( 2026, 8, 25 ), "dd.MM.yyyy", CultureInfo.InvariantCulture );

        Assert.Equal( "25.08.2026", result );
    }

    [Fact]
    public void FormatDisplayValue_Supports_CustomFormatterWithDirectFormat()
    {
        var result = Formaters.FormatDisplayValue( 12.5m, "custom", new TestCustomFormatter() );

        Assert.Equal( "custom:12.5", result );
    }

    [Fact]
    public void FormatDisplayValue_PreservesNullBehavior()
    {
        Assert.Null( Formaters.FormatDisplayValue( null, null ) );
        Assert.Equal( string.Empty, Formaters.FormatDisplayValue( null, "C" ) );
        Assert.Equal( string.Empty, Formaters.FormatDisplayValue( null, "{0:C}" ) );
    }

    [Theory]
    [InlineData( 1, "1 B" )]
    [InlineData( 1024, "1 KB" )]
    [InlineData( 2048, "2 KB" )]
    [InlineData( 1000000, "976.563 KB" )]
    [InlineData( 1048576, "1 MB" )]
    [InlineData( 2097152, "2 MB" )]
    [InlineData( 1000000000, "953.674 MB" )]
    [InlineData( 1073741824, "1 GB" )]
    public void GetBytesReadable_Returns_HumanReadableFormat( long bytes, string expected )
    {
        var result = Formaters.GetBytesReadable( bytes );

        Assert.Equal( expected, result );
    }

    [Theory]
    [InlineData( "", "" )]
    [InlineData( " ", " " )]
    [InlineData( "FirstName", "First Name" )]
    [InlineData( "FirstNameVeryLong", "First Name Very Long" )]
    [InlineData( " FirstName ", " First Name " )]
    [InlineData( "_FirstName ", "_First Name " )]
    [InlineData( "UPPERCASE", "UPPERCASE" )]
    [InlineData( "UPPER CASE", "UPPER CASE" )]
    [InlineData( null, null )]
    public void PascalCaseToFriendlyName_Returns_FriendlyFormat( string input, string expected )
    {
        var result = Formaters.PascalCaseToFriendlyName( input );

        Assert.Equal( expected, result );
    }

    private sealed class TestCustomFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat( Type formatType )
            => formatType == typeof( ICustomFormatter ) ? this : null;

        public string Format( string format, object arg, IFormatProvider formatProvider )
            => format == "custom"
                ? $"custom:{Convert.ToString( arg, CultureInfo.InvariantCulture )}"
                : null;
    }

}