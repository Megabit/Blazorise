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
                new CodeSnippets().Execute( "Docs" )
                && new ExamplesMarkup().Execute( "Docs" )
                && new CodeSnippets().Execute( "Blog" )
                && new ExamplesMarkup().Execute( "Blog" );

            Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );

            return success ? 0 : 1;
        }
    }
}
