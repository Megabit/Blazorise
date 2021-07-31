using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Blazorise.DataGrid.Utils;
using Blazorise.Utilities;
using Xunit;

namespace Blazorise.Tests.DataGrid
{
    public class ReflectionTests
    {
        [Fact]
        public void InitObject_WithComplexObject_Returns_FullyInstancedObject()
        {
            // Act
            var result = Reflection.InitObject<Test>();

            // Assert
            Assert.NotNull( result );

            Assert.NotNull( result.NestedTest );
            Assert.Equal( default, result.NestedTest.SomeValueType );
            Assert.Equal( default, result.NestedTest.SomeNullableValueType );

            Assert.NotNull( result.NestedTest.FurtherNestedTest );
            Assert.Equal( default, result.NestedTest.FurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.NestedTest.FurtherNestedTest.SomeNullableValueType );

            Assert.NotNull( result.NestedTest.AnotherFurtherNestedTest );
            Assert.Equal( default, result.NestedTest.AnotherFurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.NestedTest.AnotherFurtherNestedTest.SomeNullableValueType );

            // Assert Copy for AnotherNestedTest
            Assert.NotNull( result.AnotherNestedTest );
            Assert.Equal( default, result.AnotherNestedTest.SomeValueType );
            Assert.Equal( default, result.AnotherNestedTest.SomeNullableValueType );

            Assert.NotNull( result.AnotherNestedTest.FurtherNestedTest );
            Assert.Equal( default, result.AnotherNestedTest.FurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.AnotherNestedTest.FurtherNestedTest.SomeNullableValueType );

            Assert.NotNull( result.AnotherNestedTest.AnotherFurtherNestedTest );
            Assert.Equal( default, result.AnotherNestedTest.AnotherFurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.AnotherNestedTest.AnotherFurtherNestedTest.SomeNullableValueType );
        }

        private class Test
        {
            public NestedTest NestedTest { get; set; }
            public int SomeValueType { get; set; }
            public int? SomeNullableValueType { get; set; }
            public NestedTest AnotherNestedTest { get; set; }

        }

        private class NestedTest
        {
            public FurtherNestedTest FurtherNestedTest { get; set; }

            public int SomeValueType { get; set; }
            public int? SomeNullableValueType { get; set; }

            public FurtherNestedTest AnotherFurtherNestedTest { get; set; }
        }

        private class FurtherNestedTest
        {
            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }
        }

    }
}
