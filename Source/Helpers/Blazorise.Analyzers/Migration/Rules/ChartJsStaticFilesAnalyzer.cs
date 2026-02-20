using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ChartJsStaticFilesAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZJS001",
        title: "Blazorise Charts static Chart.js reference",
        messageFormat: "Static Chart.js script reference '{0}' is no longer supported; remove it from '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly Regex ScriptSrcRegex = new(
        "<script\\b[^>]*\\bsrc\\s*=\\s*(?:\\\"(?<src>[^\\\"]+)\\\"|'(?<src>[^']+)')[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline );

    private static readonly Regex ChartJsMainRegex = new(
        "(?:^|[\\\\/])chart(?:\\.(?:umd|bundle))?(?:\\.min)?\\.js(?:$|[?#@\\\\/])",
        RegexOptions.IgnoreCase | RegexOptions.Compiled );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.None );
        context.EnableConcurrentExecution();
        context.RegisterAdditionalFileAction( AnalyzeAdditionalFile );
    }

    private static void AnalyzeAdditionalFile( AdditionalFileAnalysisContext context )
    {
        var filePath = context.AdditionalFile.Path;
        var fileName = Path.GetFileName( filePath );

        if ( !IsTargetHostFile( fileName ) )
            return;

        var sourceText = context.AdditionalFile.GetText( context.CancellationToken );
        if ( sourceText is null )
            return;

        var content = sourceText.ToString();

        foreach ( Match match in ScriptSrcRegex.Matches( content ) )
        {
            var srcGroup = match.Groups["src"];
            if ( !srcGroup.Success )
                continue;

            var src = srcGroup.Value;

            if ( !IsDisallowedChartJsScriptSource( src ) )
                continue;

            Report( context, filePath, fileName, sourceText, srcGroup, src );
        }
    }

    private static void Report(
        AdditionalFileAnalysisContext context,
        string filePath,
        string fileName,
        SourceText sourceText,
        Group srcGroup,
        string src )
    {
        var span = new TextSpan( srcGroup.Index, srcGroup.Length );
        var lineSpan = sourceText.Lines.GetLinePositionSpan( span );
        var location = Location.Create( filePath, span, lineSpan );

        context.ReportDiagnostic( Diagnostic.Create( Rule, location, src, fileName ) );
    }

    private static bool IsTargetHostFile( string fileName )
        => string.Equals( fileName, "index.html", StringComparison.OrdinalIgnoreCase )
           || string.Equals( fileName, "_Host.cshtml", StringComparison.OrdinalIgnoreCase )
           || string.Equals( fileName, "App.razor", StringComparison.OrdinalIgnoreCase );

    private static bool IsDisallowedChartJsScriptSource( string src )
    {
        if ( string.IsNullOrWhiteSpace( src ) )
            return false;

        src = src.Trim();

        if ( src.IndexOf( "chartjs-adapter-", StringComparison.OrdinalIgnoreCase ) >= 0 )
            return true;

        if ( src.IndexOf( "chartjs-plugin-", StringComparison.OrdinalIgnoreCase ) >= 0 )
            return true;

        return ChartJsMainRegex.IsMatch( src );
    }
}
