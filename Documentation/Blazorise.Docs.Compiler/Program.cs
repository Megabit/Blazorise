using System;
using System.Diagnostics;
using Blazorise.Docs.Compiler.ApiDocsGenerator;

namespace Blazorise.Docs.Compiler;

class Program
{
    static int Main()
    {
        var stopWatch = Stopwatch.StartNew();

        var blogMarkdownResult = new BlogMarkdown().Execute();
        var codeSnippetResult = new CodeSnippets().Execute();
        var codeExamplesResult = new CodeExamplesMarkup().Execute();
        var apiDocsGenerator = new ComponentsApiDocsGenerator().Execute();

        Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );
        return blogMarkdownResult && codeSnippetResult && codeExamplesResult  && apiDocsGenerator ? 0 : 1;
    }
}