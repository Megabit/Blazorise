using System.Collections.Generic;
using System.Collections;
using Xunit;

namespace Blazorise.Tests.DataGrid.Reflection
{
    public class IsListOrCollectionTests
    {
        [Fact]
        public void IsListOrCollection_ListOrCollectionDerivedType_ReturnsTrue()
        {
            IEnumerable<string> IEnumerableObj = new string[] { "A", "B", "C" };
            IList IListObj = new ArrayList();
            List<string> ListObj = new();
            string[] strArray = { "A", "B", "C" };

            Assert.True( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( IEnumerableObj.GetType() ) );
            Assert.True( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( IListObj.GetType() ) );
            Assert.True( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( ListObj.GetType() ) );
            Assert.True( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( strArray.GetType() ) );
        }

        [Fact]
        public void IsListOrCollection_WrongType_ReturnsFalse()
        {
            string str = "Hello";
            object obj = new { a = 1 };
            int valueType = 1;
            int? nullableValueType = 1;

            Assert.False( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( str.GetType() ) );
            Assert.False( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( obj.GetType() ) );
            Assert.False( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( valueType.GetType() ) );
            Assert.False( Blazorise.DataGrid.Utils.Reflection.IsListOrCollection( nullableValueType.GetType() ) );
        }

    }
}
