using System.Collections;
using System.Collections.Generic;
using Blazorise.DataGrid;
using Blazorise.DataGrid.Utils;
using Xunit;

namespace Blazorise.Tests.DataGrid.Utils
{
    public class TypeCheckerTests
    {
        [Fact]
        public void IsCollection_ListOrCollectionDerivedType_ReturnsTrue()
        {
            Assert.True( typeof( IEnumerable<string> ).IsCollection() );
            Assert.True( typeof( List<string> ).IsCollection() );
            Assert.True( typeof( string[] ).IsCollection() );
            Assert.True( typeof( IList ).IsCollection() );
            Assert.True( typeof( IEnumerable ).IsCollection() );
        }

        [Fact]
        public void IsCollection_WrongType_ReturnsFalse()
        {
            Assert.False( typeof( string ).IsCollection() );
            Assert.False( typeof( object ).IsCollection() );
            Assert.False( typeof( int ).IsCollection() );
            Assert.False( typeof( int? ).IsCollection() );
        }
    }
}