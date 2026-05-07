using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class AnimateStaticFilesAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZJS002",
        title: "Blazorise Animate obsolete script reference",
        messageFormat: "Blazorise Animate script reference '{0}' is obsolete; remove it from '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly Regex ScriptSrcRegex = new(
        "<script\\b[^>]*\\bsrc\\s*=\\s*(?:\\\"(?<src>[^\\\"]+)\\\"|'(?<src>[^']+)')[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline );

    private static readonly Regex AnimateScriptRegex = new(
        "(?:^|[\\\\/])blazorise\\.animate(?:\\.min)?\\.js(?:$|[?#])",
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
        string filePath = context.AdditionalFile.Path;
        string fileName = Path.GetFileName( filePath );

        if ( !IsTargetHostFile( fileName ) )
            return;

        SourceText? sourceText = context.AdditionalFile.GetText( context.CancellationToken );
        if ( sourceText is null )
            return;

        string content = sourceText.ToString();

        foreach ( Match match in ScriptSrcRegex.Matches( content ) )
        {
            Group srcGroup = match.Groups["src"];
            if ( !srcGroup.Success )
                continue;

            string src = srcGroup.Value;

            if ( !IsObsoleteAnimateScriptSource( src ) )
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
        TextSpan span = new TextSpan( srcGroup.Index, srcGroup.Length );
        LinePositionSpan lineSpan = sourceText.Lines.GetLinePositionSpan( span );
        Location location = Location.Create( filePath, span, lineSpan );

        context.ReportDiagnostic( Diagnostic.Create( Rule, location, src, fileName ) );
    }

    private static bool IsTargetHostFile( string fileName )
        => string.Equals( fileName, "index.html", StringComparison.OrdinalIgnoreCase )
           || string.Equals( fileName, "_Host.cshtml", StringComparison.OrdinalIgnoreCase )
           || string.Equals( fileName, "App.razor", StringComparison.OrdinalIgnoreCase );

    private static bool IsObsoleteAnimateScriptSource( string src )
    {
        if ( string.IsNullOrWhiteSpace( src ) )
            return false;

        src = src.Trim();

        return src.IndexOf( "_content/Blazorise.Animate/", StringComparison.OrdinalIgnoreCase ) >= 0
               && AnimateScriptRegex.IsMatch( src );
    }
}
