using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BenchmarkDotNet.Attributes;
using Blazorise.DataGrid.Utils;
using Blazorise.Utilities;

namespace Blazorise.Benchmark.DataGrid
{
    [MemoryDiagnoser]
    public class ReflectionBenchmark
    {
        [Benchmark]
        public Test InitObject_WithComplexObject_Returns_FullyInstancedObject()
            => Reflection.InitObject<Test>();

        [Benchmark]
        public Test InitObject_Standard()
            => Activator.CreateInstance<Test>();

        public class Test
        {
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

    }
}
