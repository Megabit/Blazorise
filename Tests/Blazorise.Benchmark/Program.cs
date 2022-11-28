#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Blazorise.Benchmark.Blazorise;
using Blazorise.Benchmark.DataGrid;
using Blazorise.DataGrid.Utils;
using Blazorise.Extensions;
#endregion

namespace Blazorise.Benchmark;

public class Program
{
    private static void Main( string[] args )
    {
        //_ = BenchmarkRunner.Run<SequenceEquals>();
        //_ = BenchmarkRunner.Run<ReflectionBenchmark>();
        //_ = BenchmarkRunner.Run<ThemeBenchmark>();
        _ = BenchmarkRunner.Run<PropertyGetter>();
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

    public class PropertyGetter
    {
        private class TestClass
        {
            private string testProperty { get; set; }
        }

        private const string PROPERTY_NAME = "testProperty";
        private static Func<object, string> cachedPropertyExpressionGetter;
        private static PropertyInfo cachedPropertyInfo;

        private TestClass Instantiate()
            => new();


        [Benchmark]
        public string StandardReflection()
        {
            var instance = Instantiate();
            var disposablesPropertyInfo = instance.GetType().GetProperty( PROPERTY_NAME, BindingFlags.Instance | BindingFlags.NonPublic );
            return disposablesPropertyInfo?.GetValue( instance ) as string;
        }

        [Benchmark]
        public string StandardReflectionCached()
        {
            var instance = Instantiate();
            if ( cachedPropertyInfo is null )
                cachedPropertyInfo = instance.GetType().GetProperty( PROPERTY_NAME, BindingFlags.Instance | BindingFlags.NonPublic );

            return cachedPropertyInfo?.GetValue( instance ) as string;
        }

        [Benchmark]
        public string ExpressionGetter()
        {
            var instance = Instantiate();
            return ExpressionCompiler.GetProperty<string>( instance, PROPERTY_NAME );
        }

        [Benchmark]
        public string ExpressionGetterCached()
        {
            var instance = Instantiate();
            if ( cachedPropertyExpressionGetter is null )
                cachedPropertyExpressionGetter = ExpressionCompiler.CreatePropertyGetter<string>( instance, PROPERTY_NAME );

            return cachedPropertyExpressionGetter( instance );
        }

    }
}