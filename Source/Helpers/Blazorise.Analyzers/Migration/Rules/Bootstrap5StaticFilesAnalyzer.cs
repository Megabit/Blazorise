using System;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class Bootstrap5StaticFilesAnalyzer : DiagnosticAnalyzer
{
    private const string CurrentBootstrapVersion = "5.3.8";

    private const string CurrentBootstrapIntegrity = "sha384-sRIl4kxILFvY47J16cr9ZwB07vP4J8+LH7qKQnuqkuIAvNWLzeN8tE5YBujZqJLB";

    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZCSS001",
        title: "Blazorise Bootstrap 5 stylesheet reference",
        messageFormat: "Bootstrap 5 stylesheet reference '{0}' should use Bootstrap {1} with the current integrity hash; update it in '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly Regex LinkTagRegex = new(
        "<link\\b(?<attributes>[^>]*)>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline );

    private static readonly Regex AttributeRegex = new(
        "(?<name>[\\w:-]+)\\s*=\\s*(?:\\\"(?<value>[^\\\"]*)\\\"|'(?<value>[^']*)')",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline );

    private static readonly Regex BootstrapCssHrefRegex = new(
        "^(?:https?:)?//cdn\\.jsdelivr\\.net/npm/bootstrap@(?<version>5(?:\\.[0-9]+){1,2}(?:[-A-Za-z0-9.]+)?)/dist/css/bootstrap(?:\\.min)?\\.css(?:[?#].*)?$",
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

        foreach ( Match linkMatch in LinkTagRegex.Matches( content ) )
        {
            AttributeValue? hrefAttribute = FindAttribute( linkMatch, "href" );
            if ( hrefAttribute is null )
                continue;

            Match hrefMatch = BootstrapCssHrefRegex.Match( hrefAttribute.Value.Trim() );
            if ( !hrefMatch.Success )
                continue;

            AttributeValue? integrityAttribute = FindAttribute( linkMatch, "integrity" );
            string version = hrefMatch.Groups["version"].Value;

            if ( IsCurrentBootstrapReference( version, integrityAttribute ) )
                continue;

            Report( context, filePath, fileName, sourceText, hrefAttribute, hrefAttribute.Value );
        }
    }

    private static void Report(
        AdditionalFileAnalysisContext context,
        string filePath,
        string fileName,
        SourceText sourceText,
        AttributeValue hrefAttribute,
        string href )
    {
        TextSpan span = new TextSpan( hrefAttribute.Index, hrefAttribute.Length );
        LinePositionSpan lineSpan = sourceText.Lines.GetLinePositionSpan( span );
        Location location = Location.Create( filePath, span, lineSpan );

        context.ReportDiagnostic( Diagnostic.Create( Rule, location, href, CurrentBootstrapVersion, fileName ) );
    }

    private static bool IsCurrentBootstrapReference( string version, AttributeValue? integrityAttribute )
    {
        if ( !string.Equals( version, CurrentBootstrapVersion, StringComparison.OrdinalIgnoreCase ) )
            return false;

        return integrityAttribute is null
               || string.Equals( integrityAttribute.Value.Trim(), CurrentBootstrapIntegrity, StringComparison.Ordinal );
    }

    private static AttributeValue? FindAttribute( Match tagMatch, string attributeName )
    {
        foreach ( Match attributeMatch in AttributeRegex.Matches( tagMatch.Value ) )
        {
            Group nameGroup = attributeMatch.Groups["name"];
            Group valueGroup = attributeMatch.Groups["value"];

            if ( !nameGroup.Success
                 || !valueGroup.Success
                 || !string.Equals( nameGroup.Value, attributeName, StringComparison.OrdinalIgnoreCase ) )
                continue;

            return new AttributeValue(
                valueGroup.Value,
                tagMatch.Index + valueGroup.Index,
                valueGroup.Length );
        }

        return null;
    }

    private static bool IsTargetHostFile( string fileName )
        => string.Equals( fileName, "index.html", StringComparison.OrdinalIgnoreCase )
           || string.Equals( fileName, "_Host.cshtml", StringComparison.OrdinalIgnoreCase )
           || string.Equals( fileName, "App.razor", StringComparison.OrdinalIgnoreCase );

    private sealed class AttributeValue
    {
        public AttributeValue( string value, int index, int length )
        {
            Value = value;
            Index = index;
            Length = length;
        }

        public string Value { get; }

        public int Index { get; }

        public int Length { get; }
    }
}