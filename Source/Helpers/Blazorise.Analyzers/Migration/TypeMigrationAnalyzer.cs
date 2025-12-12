using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class TypeMigrationAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor TypeRenameRule = new(
        id: "BLZTYP001",
        title: "Blazorise type renamed",
        messageFormat: "Type '{0}' was renamed to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor TypeRemovedRule = new(
        id: "BLZTYP002",
        title: "Blazorise type removed",
        messageFormat: "Type '{0}' was removed in Blazorise 2.0",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly ImmutableArray<DiagnosticDescriptor> Supported = ImmutableArray.Create(
        TypeRenameRule,
        TypeRemovedRule );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Supported;

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
            map[mapping.OldFullName] = mapping;

        context.RegisterSyntaxNodeAction(
            ctx => AnalyzeTypeUsage( ctx, map ),
            SyntaxKind.IdentifierName,
            SyntaxKind.GenericName,
            SyntaxKind.QualifiedName );
    }

    private static void AnalyzeTypeUsage(
        SyntaxNodeAnalysisContext context,
        IReadOnlyDictionary<string, TypeMapping> map )
    {
        var node = context.Node;

        if ( node is QualifiedNameSyntax qualified && qualified.Right != node )
            return;

        if ( node.Parent is MemberAccessExpressionSyntax )
            return;

        var symbol = context.SemanticModel.GetSymbolInfo( node ).Symbol;
        if ( symbol is not INamedTypeSymbol namedType )
            return;

        var metadataName = GetMetadataName( namedType.ConstructedFrom );

        if ( !map.TryGetValue( metadataName, out var mapping ) )
            return;

        if ( mapping.NewFullName is null )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                TypeRemovedRule,
                node.GetLocation(),
                mapping.OldFullName ) );
            return;
        }

        context.ReportDiagnostic( Diagnostic.Create(
            TypeRenameRule,
            node.GetLocation(),
            mapping.OldFullName,
            mapping.NewFullName ) );
    }

    private static string GetMetadataName( INamedTypeSymbol type )
    {
        var nsSymbol = type.ContainingNamespace;
        if ( nsSymbol is null || nsSymbol.IsGlobalNamespace )
            return type.MetadataName;

        var ns = nsSymbol.ToDisplayString( SymbolDisplayFormat.FullyQualifiedFormat );

        if ( string.IsNullOrEmpty( ns ) )
            return type.MetadataName;

        const string globalPrefix = "global::";
        if ( ns.StartsWith( globalPrefix, StringComparison.Ordinal ) )
            ns = ns.Substring( globalPrefix.Length );

        return $"{ns}.{type.MetadataName}";
    }
}

