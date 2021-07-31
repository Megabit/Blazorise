#region Using directives
using Blazorise.Extensions;
using Xunit;
#endregion

namespace Blazorise.Tests.Extensions
{
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
    }
}
