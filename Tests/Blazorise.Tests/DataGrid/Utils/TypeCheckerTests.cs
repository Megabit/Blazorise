using System.Collections;
using System.Collections.Generic;
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

            Assert.True( TypeChecker.IsListOrCollection( IEnumerableObj.GetType() ) );
            Assert.True( TypeChecker.IsListOrCollection( IListObj.GetType() ) );
            Assert.True( TypeChecker.IsListOrCollection( ListObj.GetType() ) );
            Assert.True( TypeChecker.IsListOrCollection( strArray.GetType() ) );
        }

        [Fact]
        public void IsListOrCollection_WrongType_ReturnsFalse()
        {
            string str = "Hello";
            object obj = new { a = 1 };
            int valueType = 1;
            int? nullableValueType = 1;

            Assert.False( TypeChecker.IsListOrCollection( str.GetType() ) );
            Assert.False( TypeChecker.IsListOrCollection( obj.GetType() ) );
            Assert.False( TypeChecker.IsListOrCollection( valueType.GetType() ) );
            Assert.False( TypeChecker.IsListOrCollection( nullableValueType.GetType() ) );
        }
    }
}