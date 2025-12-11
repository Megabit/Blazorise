using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ComponentMigrationAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor ComponentNameRule = new(
        id: "BLZC001",
        title: "Blazorise component renamed",
        messageFormat: "Component '{0}' was renamed to '{1}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor ParameterNameRule = new(
        id: "BLZP001",
        title: "Blazorise parameter renamed",
        messageFormat: "Parameter '{0}' was renamed to '{1}' for component '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly DiagnosticDescriptor TValueShapeRule = new(
        id: "BLZT001",
        title: "Blazorise TValue shape is invalid",
        messageFormat: "Component '{0}' expects TValue shape '{1}', but value expression is of type '{2}'",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true );

    private static readonly ImmutableArray<DiagnosticDescriptor> Supported = ImmutableArray.Create(
        ComponentNameRule,
        ParameterNameRule,
        TValueShapeRule );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Supported;

    public override void Initialize( AnalysisContext context )
    {
        context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics );
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction( Register );
    }

    private static void Register( CompilationStartAnalysisContext context )
    {
        var renderTreeBuilder = context.Compilation.GetTypeByMetadataName( "Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder" );
        var componentBase = context.Compilation.GetTypeByMetadataName( "Microsoft.AspNetCore.Components.ComponentBase" );

        if ( renderTreeBuilder is null || componentBase is null )
            return;

        var componentByOld = new Dictionary<string, ComponentMapping>( StringComparer.Ordinal );
        var componentByNew = new Dictionary<string, ComponentMapping>( StringComparer.Ordinal );

        foreach ( var mapping in BlazoriseMigrationMappings.Components )
        {
            componentByOld[mapping.OldFullName] = mapping;
            if ( mapping.NewFullName is not null )
                componentByNew[mapping.NewFullName] = mapping;
        }

        context.RegisterOperationAction(
            ctx => AnalyzeBlock( ctx, renderTreeBuilder, componentBase, componentByOld, componentByNew ),
            OperationKind.Block );
    }

    private static void AnalyzeBlock(
        OperationAnalysisContext context,
        INamedTypeSymbol renderTreeBuilder,
        INamedTypeSymbol componentBase,
        IReadOnlyDictionary<string, ComponentMapping> componentByOld,
        IReadOnlyDictionary<string, ComponentMapping> componentByNew )
    {
        if ( context.Operation is not IBlockOperation block )
            return;

        var containingType = context.ContainingSymbol?.ContainingType;
        if ( containingType is null || !IsOrInheritsFrom( containingType, componentBase ) )
            return;

        var stack = new Stack<INamedTypeSymbol>();

        foreach ( var op in block.Operations )
        {
            if ( op is not IExpressionStatementOperation exprStatement )
                continue;

            if ( exprStatement.Operation is not IInvocationOperation invocation )
                continue;

            var target = invocation.TargetMethod;
            if ( !SymbolEqualityComparer.Default.Equals( target.ContainingType, renderTreeBuilder ) )
                continue;

            if ( target.Name.Equals( "OpenComponent", StringComparison.Ordinal ) && target.TypeArguments.Length == 1 )
            {
                var componentType = target.TypeArguments[0];
                if ( componentType is INamedTypeSymbol named )
                {
                    stack.Push( named );
                    ReportComponentRenameIfNeeded( context, invocation.Syntax.GetLocation(), named, componentByOld );
                }
                continue;
            }

            if ( target.Name.Equals( "CloseComponent", StringComparison.Ordinal ) )
            {
                if ( stack.Count > 0 )
                    stack.Pop();
                continue;
            }

            if ( target.Name is "AddAttribute" or "AddComponentParameter" )
            {
                if ( stack.Count == 0 )
                    continue;

                if ( target.Parameters.Length < 3 || invocation.Arguments.Length < 3 )
                    continue;

                var attributeNameValue = invocation.Arguments[1].Value.ConstantValue;
                if ( !attributeNameValue.HasValue || attributeNameValue.Value is not string attributeName )
                    continue;

                var currentComponent = stack.Peek();
                var mapping = LookupComponentMapping( currentComponent, componentByNew, componentByOld );
                if ( mapping is null )
                    continue;

                ReportParameterRenameIfNeeded( context, invocation.Arguments[1].Value.Syntax.GetLocation(), attributeName, mapping, currentComponent );
                ReportTValueShapeIfNeeded( context, invocation.Arguments[2].Value, attributeName, mapping, currentComponent );
            }
        }
    }

    private static void ReportComponentRenameIfNeeded(
        OperationAnalysisContext context,
        Location location,
        INamedTypeSymbol componentType,
        IReadOnlyDictionary<string, ComponentMapping> componentByOld )
    {
        var metadataName = GetMetadataName( componentType.ConstructedFrom );
        if ( componentByOld.TryGetValue( metadataName, out var mapping )
             && mapping.NewFullName is not null
             && !mapping.OldFullName.Equals( mapping.NewFullName, StringComparison.Ordinal ) )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                ComponentNameRule,
                location,
                mapping.OldFullName,
                mapping.NewFullName ) );
        }
    }

    private static void ReportParameterRenameIfNeeded(
        OperationAnalysisContext context,
        Location location,
        string attributeName,
        ComponentMapping mapping,
        INamedTypeSymbol componentType )
    {
        if ( mapping.ParameterRenames.TryGetValue( attributeName, out var newName ) )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                ParameterNameRule,
                location,
                attributeName,
                newName,
                componentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
        }
    }

    private static void ReportTValueShapeIfNeeded(
        OperationAnalysisContext context,
        IOperation valueOperation,
        string attributeName,
        ComponentMapping mapping,
        INamedTypeSymbol componentType )
    {
        if ( mapping.TValueShape == TValueShape.Any )
            return;

        if ( !string.Equals( attributeName, "Value", StringComparison.Ordinal ) )
            return;

        var valueType = valueOperation.Type;
        if ( valueType is null )
            return;

        if ( mapping.TValueShape == TValueShape.Single && IsMultiValueType( valueType ) )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                TValueShapeRule,
                valueOperation.Syntax.GetLocation(),
                componentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                mapping.TValueShape.ToString(),
                valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
        }
    }

    private static ComponentMapping? LookupComponentMapping(
        INamedTypeSymbol componentType,
        IReadOnlyDictionary<string, ComponentMapping> componentByNew,
        IReadOnlyDictionary<string, ComponentMapping> componentByOld )
    {
        var metadataName = GetMetadataName( componentType.ConstructedFrom );
        if ( componentByNew.TryGetValue( metadataName, out var mappingByNew ) )
            return mappingByNew;

        if ( componentByOld.TryGetValue( metadataName, out var mappingByOld ) )
            return mappingByOld;

        return null;
    }

    private static string GetMetadataName( INamedTypeSymbol type )
    {
        var ns = type.ContainingNamespace?.ToDisplayString();
        return string.IsNullOrEmpty( ns ) ? type.MetadataName : $"{ns}.{type.MetadataName}";
    }

    private static bool IsOrInheritsFrom( INamedTypeSymbol type, INamedTypeSymbol baseType )
    {
        for ( var current = type; current is not null; current = current.BaseType )
        {
            if ( SymbolEqualityComparer.Default.Equals( current, baseType ) )
                return true;
        }

        return false;
    }

    private static bool IsMultiValueType( ITypeSymbol type )
    {
        if ( type.TypeKind == TypeKind.Array )
            return true;

        if ( type is INamedTypeSymbol named )
        {
            if ( named.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IReadOnlyList_T )
                return true;

            foreach ( var iface in named.AllInterfaces )
            {
                if ( iface.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IReadOnlyList_T )
                    return true;
            }
        }

        return false;
    }
}
