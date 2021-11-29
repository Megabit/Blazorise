#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Blazorise.Benchmark.Blazorise;
using Blazorise.Benchmark.DataGrid;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Benchmark
{
    public class Program
    {
        private static void Main( string[] args )
        {
            //_ = BenchmarkRunner.Run<SequenceEquals>();
            //_ = BenchmarkRunner.Run<ReflectionBenchmark>();
            _ = BenchmarkRunner.Run<ThemeBenchmark>();
        }

        public class SequenceEquals
        {
            private List<string> simpleList1 = new() { "A", "B", "C" };
            private List<string> simpleList2 = new() { "A", "B", "C" };

            [Benchmark]
            public bool SequenceEqual() => simpleList1.SequenceEqual( simpleList2 );

            [Benchmark]
            public bool ArraySequenceEqual() => simpleList1.AreEqual( simpleList2 );
        }
    }
}