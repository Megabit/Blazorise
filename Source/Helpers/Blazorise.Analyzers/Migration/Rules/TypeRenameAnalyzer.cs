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
        context.RegisterCompilationStartAction( Register );
    }

    private static void Register( CompilationStartAnalysisContext context )
    {
        var map = new Dictionary<string, TypeMapping>( StringComparer.Ordinal );
        foreach ( var mapping in BlazoriseMigrationMappings.Types )
        {
            if ( mapping.NewFullName is not null )
                map[mapping.OldFullName] = mapping;
        }

        context.RegisterSyntaxNodeAction(
            ctx => Analyze( ctx, map ),
            SyntaxKind.IdentifierName,
            SyntaxKind.GenericName,
            SyntaxKind.QualifiedName );
    }

    private static void Analyze( SyntaxNodeAnalysisContext context, IReadOnlyDictionary<string, TypeMapping> map )
    {
        var node = context.Node;

        if ( node is QualifiedNameSyntax qualified && qualified.Right != node )
            return;

        if ( node.Parent is MemberAccessExpressionSyntax )
            return;

        var symbol = context.SemanticModel.GetSymbolInfo( node ).Symbol;
        if ( symbol is not INamedTypeSymbol namedType )
            return;

        var metadataName = RenderTreeMigrationEngine.GetMetadataName( namedType.ConstructedFrom );

        if ( !map.TryGetValue( metadataName, out var mapping ) )
            return;

        context.ReportDiagnostic( Diagnostic.Create(
            Rule,
            node.GetLocation(),
            mapping.OldFullName,
            mapping.NewFullName ) );
    }
}
