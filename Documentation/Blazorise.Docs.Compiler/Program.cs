using System;
using System.Diagnostics;
using Blazorise.Docs.Compiler.ApiDocsGenerator;

namespace Blazorise.Docs.Compiler;

class Program
{
    static int Main()
    {
        Stopwatch stopWatch = Stopwatch.StartNew();

        bool codeSnippetResult = new CodeSnippets().Execute();
        bool codeExamplesResult = new CodeExamplesMarkup().Execute();
        bool apiDocsGenerator = new ComponentsApiDocsGenerator().Execute();
        bool docsIndexGenerator = new DocsIndexGenerator().Execute();

        Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );
        return codeSnippetResult && codeExamplesResult && apiDocsGenerator && docsIndexGenerator ? 0 : 1;
    }
}