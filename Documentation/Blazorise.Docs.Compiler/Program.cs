using System;
using System.Diagnostics;

namespace Blazorise.Docs.Compiler
{
    class Program
    {
        static int Main()
        {
            var stopWatch = Stopwatch.StartNew();
            var success =
                new CodeSnippets().Execute()
                && new ExamplesMarkup().Execute();

            Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );

            return success ? 0 : 1;
        }
    }
}
