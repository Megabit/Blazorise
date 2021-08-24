using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Blazorise.DataGrid.Utils;

namespace Blazorise.Benchmark.DataGrid
{
    [MemoryDiagnoser]
    public class ReflectionBenchmark
    {
        [Benchmark]
        public Test InitObject_WithComplexObject_Returns_FullyInstancedObject()
            => RecursiveObjectActivator.CreateInstance<Test>();

        [Benchmark]
        public Test InitObject_Standard()
            => Activator.CreateInstance<Test>();

        public class Test
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

        public class NestedTest
        {
            public FurtherNestedTest FurtherNestedTest { get; set; }
            public FurtherNestedTest AnotherFurtherNestedTest { get; set; }

            public string UnusualReferenceType { get; set; }

            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }
            public List<NestedTest> List { get; set; }
        }

        public class FurtherNestedTest
        {
            public string UnusualReferenceType { get; set; }

            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }

            public List<NestedTest> List { get; set; }
        }

        public class CyclicTest
        {
            public CircularReference CircularReference { get; set; }

            public ObjectCycle ObjectCycle { get; set; }
        }

        public class CircularReference
        {
            public CircularReference _CircularReference { get; set; }
        }

        public class ObjectCycle
        {
            public CircularReference CircularReference { get; set; }

            public CyclicTest CycleTest { get; set; }
        }
    }
}