using Blazorise.Extensions;
using Blazorise.Tests.TestServices;
using Xunit;

namespace Blazorise.Tests.Utils;

public class NavigationExtensionsTest
{
    [Theory]
    [InlineData( null, "" )]
    [InlineData( "", "https://www.example.com/" )]
    [InlineData( "test", "https://www.example.com/test" )]
    [InlineData( "test?arg1=0", "https://www.example.com/test?arg1=0" )]
    [InlineData( "mailto:test@blazorise", "mailto:test@blazorise" )]
    [InlineData( "http://blazorise.com:123", "http://blazorise.com:123/" )]
    [InlineData( "C:\\file.bat", "file:///C:/file.bat" )]
    [InlineData( "xyz://blazorise.com:abcd", "xyz://blazorise.com:abcd" )]
    public void GetAbsoluteUriTest( string uri, string expected )
    {
        var nav = new TestNavigationManager();
        var result = nav.GetAbsoluteUri( uri );
        Assert.Equal( expected, result );
    }

    [Theory]
    [InlineData( null, false )]
    [InlineData( "", false )]
    [InlineData( "https://www.example.com", false )]
    [InlineData( "https://www.example.com/bas", false )]
    [InlineData( "https://www.example.com/base", true )]
    [InlineData( "https://www.example.com/base/", true )]
    [InlineData( "https://www.example.com/base/sub", false )]
    [InlineData( "/base/", true )]
    [InlineData( "/base/sub", false )]
    public void IsMatch_AllTest( string uri, bool isMatch )
    {
        var nav = new TestNavigationManager();
        var result = nav.IsMatch( uri, Match.All, null );
        Assert.Equal( isMatch, result );
    }

    [Theory]
    [InlineData( null, true )]
    [InlineData( "", true )]
    [InlineData( "https://www.example.com", true )]
    [InlineData( "https://www.example.com/bas", false )]
    [InlineData( "https://www.example.com/base", true )]
    [InlineData( "https://www.example.com/base/", true )]
    [InlineData( "https://www.example.com/base/sub", false )]
    [InlineData( "/base/", true )]
    [InlineData( "/base/sub", false )]
    public void IsMatch_PrefixTest( string uri, bool isMatch )
    {
        var nav = new TestNavigationManager();
        var result = nav.IsMatch( uri, Match.Prefix, null );
        Assert.Equal( isMatch, result );
    }

    [Fact]
    public void IsMatch_CustomTest()
    {
        var nav = new TestNavigationManager();
        Assert.False( nav.IsMatch( "file://test.txt", Match.Prefix, _ => false ) );
        Assert.True( nav.IsMatch( "file://test.txt", Match.Prefix, _ => true ) );
    }
}