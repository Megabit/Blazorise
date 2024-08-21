#region Using directives
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions;

public class EventArgsExtensions
{
    [Theory]
    [InlineData( "a" )]
    [InlineData( "0" )]
    [InlineData( "9" )]
    [InlineData( "ç" )]
    [InlineData( "z" )]
    [InlineData( "A" )]
    [InlineData( "Z" )]
    public void IsTextKey_Should_Return_True_When_TextCharacter( string text )
    {
        var keyboardEventArgs = new KeyboardEventArgs()
        {
            Key = text
        };

        keyboardEventArgs.IsTextKey().Should().BeTrue();
    }

    [Theory]
    [InlineData( "abc" )]
    [InlineData( null )]
    [InlineData( "123" )]
    [InlineData( "12" )]
    public void IsTextKey_Should_Return_False_When_Invalid( string text )
    {
        var keyboardEventArgs = new KeyboardEventArgs()
        {
            Key = text
        };

        keyboardEventArgs.IsTextKey().Should().BeFalse();
    }
}