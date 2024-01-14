using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.Utils;

public class FormatersTests
{


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
    public void PascalCaseToFriendlyName_Returns_FriendlyFormat( string input, string expected )
    {
        var result = Formaters.PascalCaseToFriendlyName( input );

        Assert.Equal( expected, result );
    }

}