using System.Collections.Generic;
using Blazorise.DataGrid.Utils;
using Xunit;

namespace Blazorise.Tests.DataGrid.Utils
{
    public class RecursiveObjectActivatorTests
    {
        [Fact]
        public void InitObject_WithComplexObject_Returns_FullyInstancedObject()
        {
            // Act
            var result = RecursiveObjectActivator.CreateInstance<Test>();

            // Assert
            Assert.NotNull( result );

            Assert.NotNull( result.CycleTest );
            Assert.NotNull( result.CycleTest.CircularReference );
            Assert.NotNull( result.CycleTest.CircularReference._CircularReference );
            Assert.Null( result.CycleTest.CircularReference._CircularReference._CircularReference );

            Assert.NotNull( result.CycleTest.ObjectCycle);
            Assert.NotNull( result.CycleTest.ObjectCycle.CircularReference );
            Assert.NotNull( result.CycleTest.ObjectCycle.CircularReference._CircularReference );
            Assert.Null( result.CycleTest.ObjectCycle.CircularReference._CircularReference._CircularReference );

            Assert.NotNull( result.CycleTest.ObjectCycle.CycleTest );
            Assert.Null( result.CycleTest.ObjectCycle.CycleTest.ObjectCycle );
            Assert.NotNull( result.CycleTest.ObjectCycle.CycleTest.CircularReference );

            Assert.Equal( default, result.UnusualReferenceType );
            Assert.Equal( default, result.SomeValueType );
            Assert.Equal( default, result.SomeNullableValueType );
            Assert.Equal( default, result.List );

            Assert.NotNull( result.NestedTest );
            Assert.Equal( default, result.NestedTest.UnusualReferenceType );
            Assert.Equal( default, result.NestedTest.SomeValueType );
            Assert.Equal( default, result.NestedTest.SomeNullableValueType );
            Assert.Equal( default, result.NestedTest.List );

            Assert.NotNull( result.NestedTest.FurtherNestedTest );
            Assert.Equal( default, result.NestedTest.FurtherNestedTest.UnusualReferenceType );
            Assert.Equal( default, result.NestedTest.FurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.NestedTest.FurtherNestedTest.SomeNullableValueType );
            Assert.Equal( default, result.NestedTest.FurtherNestedTest.List );

            Assert.NotNull( result.NestedTest.AnotherFurtherNestedTest );
            Assert.Equal( default, result.NestedTest.AnotherFurtherNestedTest.UnusualReferenceType );
            Assert.Equal( default, result.NestedTest.AnotherFurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.NestedTest.AnotherFurtherNestedTest.SomeNullableValueType );
            Assert.Equal( default, result.NestedTest.AnotherFurtherNestedTest.List );

            // Assert Copy for AnotherNestedTest
            Assert.NotNull( result.AnotherNestedTest );
            Assert.Equal( default, result.AnotherNestedTest.UnusualReferenceType );
            Assert.Equal( default, result.AnotherNestedTest.SomeValueType );
            Assert.Equal( default, result.AnotherNestedTest.SomeNullableValueType );
            Assert.Equal( default, result.AnotherNestedTest.List );

            Assert.NotNull( result.AnotherNestedTest.FurtherNestedTest );
            Assert.Equal( default, result.AnotherNestedTest.FurtherNestedTest.UnusualReferenceType );
            Assert.Equal( default, result.AnotherNestedTest.FurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.AnotherNestedTest.FurtherNestedTest.SomeNullableValueType );
            Assert.Equal( default, result.AnotherNestedTest.FurtherNestedTest.List );

            Assert.NotNull( result.AnotherNestedTest.AnotherFurtherNestedTest );
            Assert.Equal( default, result.AnotherNestedTest.AnotherFurtherNestedTest.UnusualReferenceType );
            Assert.Equal( default, result.AnotherNestedTest.AnotherFurtherNestedTest.SomeValueType );
            Assert.Equal( default, result.AnotherNestedTest.AnotherFurtherNestedTest.SomeNullableValueType );
            Assert.Equal( default, result.AnotherNestedTest.AnotherFurtherNestedTest.List );
        }

        private class Test
        {
            public CyclicTest CycleTest { get; set; }
            public NestedTest NestedTest { get; set; }
            public NestedTest AnotherNestedTest { get; set; }

            public string UnusualReferenceType { get; set; }

            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }

            public List<NestedTest> List { get; set; }

            private NestedTest PrivateNestedTest { get; set; }

            protected NestedTest ProtectedNestedTest { get; set; }

            internal NestedTest InternalNestedTest { get; set; }
        }

        private class NestedTest
        {
            public FurtherNestedTest FurtherNestedTest { get; set; }
            public FurtherNestedTest AnotherFurtherNestedTest { get; set; }

            public string UnusualReferenceType { get; set; }

            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }
            public List<NestedTest> List { get; set; }
        }

        private class FurtherNestedTest
        {
            public string UnusualReferenceType { get; set; }

            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }

            public List<NestedTest> List { get; set; }
        }

        private class CyclicTest
        {
            public CircularReference CircularReference{ get; set; }

            public ObjectCycle ObjectCycle { get; set; }
        }

        private class CircularReference
        {
            public CircularReference _CircularReference { get; set; }
        }

        private class ObjectCycle
        {
            public CircularReference CircularReference { get; set; }

            public CyclicTest CycleTest { get; set; }
        }
    }
}