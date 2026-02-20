using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class TypeRenameAnalyzer : DiagnosticAnalyzer
{
    private static readonly IReadOnlyDictionary<string, TypeMapping> Map = CreateMap();
    private static readonly HashSet<string> CandidateSimpleNames = CreateCandidateSimpleNames( Map.Keys );

    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZTYP001",
        title: "Blazorise type renamed",
        messageFormat: "Type '{0}' was renamed to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            ctx => Analyze( ctx, Map, CandidateSimpleNames ),
            SyntaxKind.IdentifierName,
            SyntaxKind.GenericName,
            SyntaxKind.QualifiedName );
    }

    private static IReadOnlyDictionary<string, TypeMapping> CreateMap()
    {
        Dictionary<string, TypeMapping> map = new( StringComparer.Ordinal );
        foreach ( TypeMapping mapping in BlazoriseMigrationMappings.Types )
        {
            if ( mapping.NewFullName is not null )
                map[mapping.OldFullName] = mapping;
        }

        return map;
    }

    private static HashSet<string> CreateCandidateSimpleNames( IEnumerable<string> fullNames )
    {
        HashSet<string> names = new( StringComparer.Ordinal );
        foreach ( string fullName in fullNames )
        {
            string? simpleName = RenderTreeMigrationEngine.GetSimpleName( fullName );
            if ( simpleName is not null )
                names.Add( simpleName );
        }

        return names;
    }

    private static void Analyze(
        SyntaxNodeAnalysisContext context,
        IReadOnlyDictionary<string, TypeMapping> map,
        ISet<string> candidateSimpleNames )
    {
        SyntaxNode node = context.Node;

        if ( node is QualifiedNameSyntax qualified && qualified.Right != node )
            return;

        if ( node.Parent is MemberAccessExpressionSyntax )
            return;

        if ( !ShouldAnalyzeNode( node, candidateSimpleNames ) )
            return;

        ISymbol? symbol = context.SemanticModel.GetSymbolInfo( node ).Symbol;
        if ( symbol is not INamedTypeSymbol namedType )
            return;

        string metadataName = RenderTreeMigrationEngine.GetMetadataName( namedType.ConstructedFrom );

        if ( !map.TryGetValue( metadataName, out TypeMapping mapping ) )
            return;

        context.ReportDiagnostic( Diagnostic.Create(
            Rule,
            node.GetLocation(),
            mapping.OldFullName,
            mapping.NewFullName ) );
    }

    private static bool ShouldAnalyzeNode( SyntaxNode node, ISet<string> candidateSimpleNames )
    {
        string? simpleName = node switch
        {
            IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
            GenericNameSyntax genericName => genericName.Identifier.ValueText,
            _ => null,
        };

        return simpleName is not null && candidateSimpleNames.Contains( simpleName );
    }
}
