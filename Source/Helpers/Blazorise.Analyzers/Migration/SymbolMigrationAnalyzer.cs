using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class SymbolMigrationAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor MemberRenameRule = new(
        id: "BLZS001",
        title: "Blazorise member renamed",
        messageFormat: "Member '{0}' was renamed to '{1}' on type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor MemberRemovedRule = new(
        id: "BLZS002",
        title: "Blazorise member removed",
        messageFormat: "Member '{0}' was removed from type '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly ImmutableArray<DiagnosticDescriptor> Supported = ImmutableArray.Create(
        MemberRenameRule,
        MemberRemovedRule );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Supported;

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction( Register );
    }

    private static void Register( CompilationStartAnalysisContext context )
    {
        var map = new Dictionary<(string containingType, string oldName), SymbolMapping>();

        foreach ( var mapping in BlazoriseMigrationMappings.Symbols )
        {
            map[(mapping.ContainingType, mapping.OldName)] = mapping;
        }

        context.RegisterOperationAction(
            ctx => AnalyzeMemberReference( ctx, map ),
            OperationKind.FieldReference,
            OperationKind.PropertyReference,
            OperationKind.Invocation );
    }

    private static void AnalyzeMemberReference(
        OperationAnalysisContext context,
        IReadOnlyDictionary<(string containingType, string oldName), SymbolMapping> map )
    {
        ISymbol? member = context.Operation switch
        {
            IFieldReferenceOperation fieldRef => fieldRef.Field,
            IPropertyReferenceOperation propRef => propRef.Property,
            IInvocationOperation invocation => invocation.TargetMethod,
            _ => null,
        };

        if ( member is null || member.ContainingType is null )
            return;

        var containingMetadata = GetMetadataName( member.ContainingType.ConstructedFrom );
        var key = (containingMetadata, member.Name);

        if ( !map.TryGetValue( key, out var mapping ) )
            return;

        if ( mapping.NewName is null )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                MemberRemovedRule,
                context.Operation.Syntax.GetLocation(),
                mapping.OldName,
                mapping.ContainingType ) );
            return;
        }

        context.ReportDiagnostic( Diagnostic.Create(
            MemberRenameRule,
            context.Operation.Syntax.GetLocation(),
            mapping.OldName,
            mapping.NewName,
            mapping.ContainingType ) );
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
