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
        public void IsListOrCollection_ListOrCollectionDerivedType_ReturnsTrue()
        {
            IEnumerable<string> IEnumerableObj = new string[] { "A", "B", "C" };
            IList IListObj = new ArrayList();
            List<string> ListObj = new();
            string[] strArray = { "A", "B", "C" };

            Assert.True( IEnumerableObj.GetType().IsListOrCollection() );
            Assert.True( IListObj.GetType().IsListOrCollection() );
            Assert.True( ListObj.GetType().IsListOrCollection() );
            Assert.True( strArray.GetType().IsListOrCollection() );
        }

        [Fact]
        public void IsListOrCollection_WrongType_ReturnsFalse()
        {
            string str = "Hello";
            object obj = new { a = 1 };
            int valueType = 1;
            int? nullableValueType = 1;

            Assert.False( str.GetType().IsListOrCollection() );
            Assert.False( obj.GetType().IsListOrCollection() );
            Assert.False( valueType.GetType().IsListOrCollection() );
            Assert.False( nullableValueType.GetType().IsListOrCollection() );
        }
    }
}