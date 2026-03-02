#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class EnumerationToStringTest
{
    [Fact]
    public void ToString_ForUtilityColorEnumerations_DoesNotThrowAndMatchesName()
    {
        AssertToStringReturnsName( TextColor.Primary );
        AssertToStringReturnsName( TextColor.Primary.OnSelf );
        AssertToStringReturnsName( TextColor.Primary.OnWrapper );
        AssertToStringReturnsName( TextColor.Primary.Emphasis );
        AssertToStringReturnsName( TextColor.Primary.Emphasis.OnSelf );

        AssertToStringReturnsName( Background.Primary );
        AssertToStringReturnsName( Background.Primary.OnSelf );
        AssertToStringReturnsName( Background.Primary.OnWrapper );

        var subtleBackground = (Background)Background.Primary.Subtle;

        AssertToStringReturnsName( subtleBackground );
        AssertToStringReturnsName( subtleBackground.OnWrapper );
    }

    [Fact]
    public void ToString_ForOtherEnumerationRecords_MatchesName()
    {
        AssertToStringReturnsName( Color.Primary );
        AssertToStringReturnsName( Intent.Warning );
        AssertToStringReturnsName( Target.Blank );
    }

    private static void AssertToStringReturnsName<T>( T value )
        where T : Enumeration<T>
    {
        var exception = Record.Exception( () => value.ToString() );

        Assert.Null( exception );
        Assert.Equal( value.Name, value.ToString() );
    }
}