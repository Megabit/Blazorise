using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class MemberRemovedAnalyzer : DiagnosticAnalyzer
{
    private static readonly IReadOnlyDictionary<(string containingType, string oldName), SymbolMapping> Map = CreateMap();
    private static readonly HashSet<string> CandidateMemberNames = CreateCandidateMemberNames();

    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZS002",
        title: "Blazorise member removed",
        messageFormat: "Member '{0}' was removed from type '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(
            ctx => Analyze( ctx, Map, CandidateMemberNames ),
            OperationKind.FieldReference,
            OperationKind.PropertyReference,
            OperationKind.Invocation );
    }

    private static void Analyze(
        OperationAnalysisContext context,
        IReadOnlyDictionary<(string containingType, string oldName), SymbolMapping> map,
        ISet<string> candidateMemberNames )
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

        if ( !candidateMemberNames.Contains( member.Name ) )
            return;

        string containingMetadata = RenderTreeMigrationEngine.GetMetadataName( member.ContainingType.ConstructedFrom );
        (string containingType, string oldName) key = (containingMetadata, member.Name);

        if ( !map.TryGetValue( key, out SymbolMapping mapping ) )
            return;

        context.ReportDiagnostic( Diagnostic.Create(
            Rule,
            context.Operation.Syntax.GetLocation(),
            mapping.OldName,
            mapping.ContainingType ) );
    }

    private static IReadOnlyDictionary<(string containingType, string oldName), SymbolMapping> CreateMap()
    {
        Dictionary<(string containingType, string oldName), SymbolMapping> map = new();

        foreach ( SymbolMapping mapping in BlazoriseMigrationMappings.Symbols )
        {
            if ( mapping.NewName is null )
                map[(mapping.ContainingType, mapping.OldName)] = mapping;
        }

        return map;
    }

    private static HashSet<string> CreateCandidateMemberNames()
    {
        HashSet<string> names = new( StringComparer.Ordinal );

        foreach ( SymbolMapping mapping in BlazoriseMigrationMappings.Symbols )
        {
            if ( mapping.NewName is null )
                names.Add( mapping.OldName );
        }

        return names;
    }
}

