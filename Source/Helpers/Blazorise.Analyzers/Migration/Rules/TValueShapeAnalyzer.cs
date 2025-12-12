using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class TValueShapeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZT001",
        title: "Blazorise TValue shape is invalid",
        messageFormat: "Component '{0}' expects TValue shape '{1}', but value expression is of type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create( Rule );

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction( compilationStart =>
        {
            RenderTreeMigrationEngine.Register( compilationStart, new Handler() );
        } );
    }

    private sealed class Handler : MigrationHandler
    {
        public override void OnAttributeAfterUpdate(
            OperationAnalysisContext context,
            MigrationContext migration,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            ReportIfNeeded( context, component );
        }

        public override void OnCloseComponent( OperationAnalysisContext context, MigrationContext migration, ComponentContext component )
        {
            ReportIfNeeded( context, component );
        }

        private static void ReportIfNeeded( OperationAnalysisContext context, ComponentContext componentContext )
        {
            var mapping = componentContext.Mapping;

            if ( mapping is null || mapping.TValueShape == TValueShape.Any )
                return;

            if ( componentContext.HasReportedTValueShape )
                return;

            var valueType = componentContext.ValueType ?? componentContext.TValueType;
            var location = componentContext.ValueLocation ?? componentContext.ComponentLocation;

            if ( valueType is null || location is null )
                return;

            if ( mapping.TValueShape == TValueShape.Single && RenderTreeMigrationEngine.IsMultiValueType( valueType ) )
            {
                context.ReportDiagnostic( Diagnostic.Create(
                    Rule,
                    location,
                    componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    mapping.TValueShape.ToString(),
                    valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
                componentContext.HasReportedTValueShape = true;
                return;
            }

            var multiShapeType = componentContext.TValueType ?? componentContext.ValueType ?? valueType;

            if ( mapping.TValueShape == TValueShape.SingleOrMultiListOrArray
                 && componentContext.IsMultiple
                 && multiShapeType is not null
                 && !RenderTreeMigrationEngine.IsMultiValueType( multiShapeType ) )
            {
                context.ReportDiagnostic( Diagnostic.Create(
                    Rule,
                    location,
                    componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    TValueShape.SingleOrMultiListOrArray.ToString(),
                    valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
                componentContext.HasReportedTValueShape = true;
            }
        }
    }
}

