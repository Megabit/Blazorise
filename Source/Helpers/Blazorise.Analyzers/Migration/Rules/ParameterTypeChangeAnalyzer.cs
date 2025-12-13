using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazorise.Analyzers.Migration.Rules;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ParameterTypeChangeAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor Rule = new(
        id: "BLZP002",
        title: "Blazorise parameter type changed",
        messageFormat: "Parameter '{0}' on component '{1}' has changed type: {2}",
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
        public override void OnAttributeBeforeUpdate(
            OperationAnalysisContext context,
            MigrationContext migration,
            ComponentContext component,
            string attributeName,
            IOperation valueOperation,
            Location location )
        {
            var metadataName = RenderTreeMigrationEngine.GetMetadataName( component.ComponentType.ConstructedFrom );
            var valueType = RenderTreeMigrationEngine.UnwrapNullable( valueOperation.Type );

            if ( ( string.Equals( metadataName, "Blazorise.DataGrid.DataGridColumn`1", System.StringComparison.Ordinal )
                   || string.Equals( metadataName, "Blazorise.DataGridColumn`1", System.StringComparison.Ordinal ) )
                 && string.Equals( attributeName, "Width", System.StringComparison.Ordinal )
                 && valueType?.SpecialType == SpecialType.System_String )
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName );

                context.ReportDiagnostic( Diagnostic.Create(
                    Rule,
                    location,
                    properties,
                    attributeName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    "Width now expects IFluentSizing (e.g., Width.Px(60))." ) );
            }

            if ( ( string.Equals( metadataName, "Blazorise.Row", System.StringComparison.Ordinal )
                   || string.Equals( metadataName, "Blazorise.Fields", System.StringComparison.Ordinal ) )
                 && string.Equals( attributeName, "Gutter", System.StringComparison.Ordinal )
                 && valueType is INamedTypeSymbol named
                 && named.IsTupleType )
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add( MigrationDiagnosticProperties.OldName, attributeName );

                context.ReportDiagnostic( Diagnostic.Create(
                    Rule,
                    location,
                    properties,
                    attributeName,
                    component.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                    "Gutter now expects IFluentGutter fluent API instead of tuple." ) );
            }
        }
    }
}

