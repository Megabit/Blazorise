#region Using directives
using Blazorise.Extensions;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions;

public class ArrayExtensions
{
    [Theory]
    [InlineData( null, null )]
    [InlineData( new[] { "A", "B", "C" }, new[] { "A", "B", "C" } )]
    [InlineData( new[] { "1", "2", "3" }, new[] { "1", "2", "3" } )]
    public void AreEqual_Returns_True( string[] list1, string[] list2 )
    {
        Assert.True( list1.AreEqual( list2 ) );
    }

    [Theory]
    [InlineData( null, new[] { "data" } )]
    [InlineData( new[] { "A", "B", "C" }, new[] { "C", "B", "A" } )]
    [InlineData( new[] { "1", "2", "3" }, new[] { "1", "2", "3", "4" } )]
    public void AreEqual_Returns_False( string[] list1, string[] list2 )
    {
        Assert.False( list1.AreEqual( list2 ) );
    }


    [Theory]
    [InlineData( null, null )]
    [InlineData( new[] { "A", "B", "C" }, new[] { "A", "C", "B" } )]
    [InlineData( new[] { "1", "2", "3" }, new[] { "3", "2", "1" } )]
    public void AreEqualOrdered_Returns_True( string[] list1, string[] list2 )
    {
        Assert.True( list1.AreEqualOrdered( list2 ) );
    }

    [Theory]
    [InlineData( null, new[] { "data" } )]
    [InlineData( new[] { "A", "B", "C" }, new[] { "C", "B", "A", "D" } )]
    [InlineData( new[] { "1", "2", "3" }, new[] { "1", "2", "3", "4" } )]
    public void AreEqualOrdered_Returns_False( string[] list1, string[] list2 )
    {
        Assert.False( list1.AreEqualOrdered( list2 ) );
    }

    [Theory]
    [InlineData( new[] { "A", "B", "C" }, "A", 0 )]
    [InlineData( new[] { "A", "B", "C" }, "B", 1 )]
    [InlineData( new[] { "A", "B", "C" }, "C", 2 )]
    public void Index_Found_Returns_ActualIndex( string[] list, string search, int expected )
    {
        Assert.Equal( expected, list.Index( x => x == search ) );
    }

    [Theory]
    [InlineData( null, "A", -1 )]
    [InlineData( new[] { "A", "B", "C" }, "D", -1 )]
    [InlineData( new[] { "A", "B", "C" }, null, -1 )]
    public void Index_NotFound_Returns_Minus1( string[] list, string search, int expected )
    {
        Assert.Equal( expected, list.Index( x => x == search ) );
    }



}