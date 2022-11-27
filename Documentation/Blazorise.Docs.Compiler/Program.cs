using System;
using System.Diagnostics;

namespace Blazorise.Docs.Compiler;

class Program
{
    static int Main()
    {
        var stopWatch = Stopwatch.StartNew();

        var blogMarkdownResult = new BlogMarkdown().Execute();
        var codeSnippetResult = new CodeSnippets().Execute();
        var codeExamplesResult = new CodeExamplesMarkup().Execute();

        Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );

        return blogMarkdownResult && codeSnippetResult && codeExamplesResult ? 0 : 1;
    }
}