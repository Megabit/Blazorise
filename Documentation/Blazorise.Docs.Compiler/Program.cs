using System;
using System.Diagnostics;
using Blazorise.Docs.Compiler.ApiDocsGenerator;

namespace Blazorise.Docs.Compiler;

class Program
{
    static int Main( string[] args )
    {
        Stopwatch stopWatch = Stopwatch.StartNew();

        string apiDocsOutputPath = GetArgValue( args, "--api-docs-path" );

        bool codeSnippetResult = new CodeSnippets().Execute();
        bool codeExamplesResult = new CodeExamplesMarkup().Execute();
        bool apiDocsGenerator = new ComponentsApiDocsGenerator( apiDocsOutputPath ).Execute();
        bool docsIndexGenerator = new DocsIndexGenerator().Execute();

        Console.WriteLine( $"Blazorise.Docs.Compiler completed in {stopWatch.ElapsedMilliseconds} milliseconds." );
        return codeSnippetResult && codeExamplesResult && apiDocsGenerator && docsIndexGenerator ? 0 : 1;
    }

    private static string GetArgValue( string[] args, string name )
    {
        if ( args is null || args.Length == 0 )
            return null;

        for ( int i = 0; i < args.Length; i++ )
        {
            string arg = args[i];

            if ( string.Equals( arg, name, StringComparison.OrdinalIgnoreCase ) )
            {
                if ( i + 1 < args.Length )
                    return args[i + 1];

                return null;
            }

            string prefix = name + "=";
            if ( arg.StartsWith( prefix, StringComparison.OrdinalIgnoreCase ) )
                return arg.Substring( prefix.Length );
        }

        return null;
    }
}