using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class MemberRemovedAnalyzer : DiagnosticAnalyzer
{
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
        context.RegisterCompilationStartAction( Register );
    }

    private static void Register( CompilationStartAnalysisContext context )
    {
        var map = new Dictionary<(string containingType, string oldName), SymbolMapping>();
        foreach ( var mapping in BlazoriseMigrationMappings.Symbols )
        {
            if ( mapping.NewName is null )
                map[(mapping.ContainingType, mapping.OldName)] = mapping;
        }

        context.RegisterOperationAction(
            ctx => Analyze( ctx, map ),
            OperationKind.FieldReference,
            OperationKind.PropertyReference,
            OperationKind.Invocation );
    }

    private static void Analyze(
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

        var containingMetadata = RenderTreeMigrationEngine.GetMetadataName( member.ContainingType.ConstructedFrom );
        var key = (containingMetadata, member.Name);

        if ( !map.TryGetValue( key, out var mapping ) )
            return;

        context.ReportDiagnostic( Diagnostic.Create(
            Rule,
            context.Operation.Syntax.GetLocation(),
            mapping.OldName,
            mapping.ContainingType ) );
    }
}

