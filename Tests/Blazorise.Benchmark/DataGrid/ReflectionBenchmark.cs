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

        public class Test
        {
            public NestedTest NestedTest { get; set; }
            public int SomeValueType { get; set; }
            public int? SomeNullableValueType { get; set; }
            public NestedTest AnotherNestedTest { get; set; }

        }

        public class NestedTest
        {
            public FurtherNestedTest FurtherNestedTest { get; set; }

            public int SomeValueType { get; set; }
            public int? SomeNullableValueType { get; set; }

            public FurtherNestedTest AnotherFurtherNestedTest { get; set; }
        }

        public class FurtherNestedTest
        {
            public int SomeValueType { get; set; }

            public int? SomeNullableValueType { get; set; }
        }

    }
}
