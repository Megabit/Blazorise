using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Blazorise.Extensions;

namespace Blazorise.Benchmark
{
    public class Program
    {
        static void Main( string[] args )
        {
            Console.WriteLine( "Hello World!" );
            var summary = BenchmarkRunner.Run<SequenceEquals>();
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
