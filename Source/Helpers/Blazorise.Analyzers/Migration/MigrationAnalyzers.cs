using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Blazorise.Analyzers.Migration;

[DiagnosticAnalyzer( LanguageNames.CSharp )]
public sealed class ComponentMigrationAnalyzer : DiagnosticAnalyzer
{
    private sealed class ComponentContext
    {
        public ComponentContext( INamedTypeSymbol componentType, ComponentMapping? mapping, Location componentLocation )
        {
            ComponentType = componentType;
            Mapping = mapping;
            ComponentLocation = componentLocation;
            TValueType = componentType.TypeArguments.Length > 0 ? componentType.TypeArguments[0] : null;
        }

        public INamedTypeSymbol ComponentType { get; }

        public ComponentMapping? Mapping { get; }

        public ITypeSymbol? TValueType { get; }

        public Location ComponentLocation { get; }

        public bool IsMultiple { get; set; }

        public ITypeSymbol? ValueType { get; set; }

        public Location? ValueLocation { get; set; }

        public bool HasReportedTValueShape { get; set; }
    }

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

        var stack = new Stack<ComponentContext>();

        Traverse( block );

        void Traverse( IOperation operation )
        {
            if ( operation is IInvocationOperation invocation
                 && SymbolEqualityComparer.Default.Equals( invocation.TargetMethod.ContainingType, renderTreeBuilder ) )
            {
                var target = invocation.TargetMethod;

                if ( target.Name.Equals( "OpenComponent", StringComparison.Ordinal ) && target.TypeArguments.Length == 1 )
                {
                    var componentType = target.TypeArguments[0];
                    if ( componentType is INamedTypeSymbol named )
                    {
                        var mapping = LookupComponentMapping( named, componentByNew, componentByOld );
                        stack.Push( new ComponentContext( named, mapping, invocation.Syntax.GetLocation() ) );
                        ReportComponentRenameIfNeeded( context, invocation.Syntax.GetLocation(), named, componentByOld );
                    }
                }
                else if ( target.Name.Equals( "CloseComponent", StringComparison.Ordinal ) )
                {
                    if ( stack.Count > 0 )
                    {
                        ReportTValueShapeIfNeeded( context, stack.Peek() );
                        stack.Pop();
                    }
                }
                else if ( target.Name is "AddAttribute" or "AddComponentParameter" )
                {
                    if ( stack.Count > 0 && target.Parameters.Length >= 3 && invocation.Arguments.Length >= 3 )
                    {
                        var attributeNameValue = invocation.Arguments[1].Value.ConstantValue;
                        if ( attributeNameValue.HasValue && attributeNameValue.Value is string attributeName )
                        {
                            var currentComponent = stack.Peek();
                            var mapping = currentComponent.Mapping;
                            if ( mapping is not null )
                            {
                                ReportParameterRenameIfNeeded( context, invocation.Arguments[1].Value.Syntax.GetLocation(), attributeName, mapping, currentComponent.ComponentType );
                                UpdateMultipleFlag( attributeName, invocation.Arguments[2].Value, mapping, currentComponent );
                                UpdateValueBinding( attributeName, invocation.Arguments[2].Value, mapping, currentComponent );
                                ReportTValueShapeIfNeeded( context, currentComponent );
                            }
                        }
                    }
                }
            }

            foreach ( var child in operation.ChildOperations )
            {
                Traverse( child );
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

    private static void UpdateValueBinding(
        string attributeName,
        IOperation valueOperation,
        ComponentMapping mapping,
        ComponentContext componentContext )
    {
        if ( string.Equals( attributeName, "Value", StringComparison.Ordinal )
             || ( mapping.ParameterRenames.TryGetValue( attributeName, out var newName )
                  && string.Equals( newName, "Value", StringComparison.Ordinal ) ) )
        {
            componentContext.ValueType = valueOperation.Type;
            componentContext.ValueLocation = valueOperation.Syntax.GetLocation();
        }
    }

    private static void UpdateMultipleFlag(
        string attributeName,
        IOperation valueOperation,
        ComponentMapping mapping,
        ComponentContext componentContext )
    {
        if ( string.Equals( attributeName, "Multiple", StringComparison.Ordinal )
             || ( mapping.ParameterRenames.TryGetValue( attributeName, out var newName )
                  && string.Equals( newName, "Multiple", StringComparison.Ordinal ) ) )
        {
            var constant = GetBooleanConstant( valueOperation );

            // Presence of the attribute enables Multiple unless it is explicitly false.
            componentContext.IsMultiple |= constant is null || constant.Value;
            return;
        }

        if ( string.Equals( attributeName, "SelectionMode", StringComparison.Ordinal )
             || ( mapping.ParameterRenames.TryGetValue( attributeName, out var renamedSelectionMode )
                  && string.Equals( renamedSelectionMode, "SelectionMode", StringComparison.Ordinal ) ) )
        {
            var isMultiple = EvaluateSelectionMode( valueOperation );
            componentContext.IsMultiple |= isMultiple ?? true; // if unknown, err on the side of multiple
        }
    }

    private static void ReportTValueShapeIfNeeded( OperationAnalysisContext context, ComponentContext componentContext )
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

        if ( mapping.TValueShape == TValueShape.Single && IsMultiValueType( valueType ) )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                TValueShapeRule,
                location,
                componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                mapping.TValueShape.ToString(),
                valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
            componentContext.HasReportedTValueShape = true;
            return;
        }

        // Prefer the declared TValue shape when evaluating multi-selection to avoid false positives
        var multiShapeType = componentContext.TValueType ?? componentContext.ValueType ?? valueType;

        if ( mapping.TValueShape == TValueShape.SingleOrMultiListOrArray
             && componentContext.IsMultiple
             && multiShapeType is not null
             && !IsMultiValueType( multiShapeType ) )
        {
            context.ReportDiagnostic( Diagnostic.Create(
                TValueShapeRule,
                location,
                componentContext.ComponentType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ),
                TValueShape.SingleOrMultiListOrArray.ToString(),
                valueType.ToDisplayString( SymbolDisplayFormat.MinimallyQualifiedFormat ) ) );
            componentContext.HasReportedTValueShape = true;
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

    private static bool IsOrInheritsFrom( INamedTypeSymbol type, INamedTypeSymbol baseType )
    {
        for ( var current = type; current is not null; current = current.BaseType )
        {
            if ( SymbolEqualityComparer.Default.Equals( current, baseType ) )
                return true;
        }

        return false;
    }

    private static bool? GetBooleanConstant( IOperation valueOperation )
    {
        switch ( valueOperation )
        {
            case IConversionOperation conversion when conversion.Operand is not null:
                return GetBooleanConstant( conversion.Operand );
            case IParenthesizedOperation parenthesized when parenthesized.Operand is not null:
                return GetBooleanConstant( parenthesized.Operand );
            case IUnaryOperation unary when unary.OperatorKind == UnaryOperatorKind.Not && unary.Operand is not null:
                {
                    var operand = GetBooleanConstant( unary.Operand );
                    return operand.HasValue ? !operand.Value : null;
                }
            default:
                var constantValue = valueOperation.ConstantValue;
                return constantValue.HasValue && constantValue.Value is bool boolValue ? boolValue : null;
        }
    }

    private static bool? EvaluateSelectionMode( IOperation valueOperation )
    {
        switch ( valueOperation )
        {
            case IFieldReferenceOperation fieldReference:
                {
                    var fieldName = fieldReference.Field?.Name;
                    if ( fieldName is not null )
                    {
                        if ( IsMultipleOrNonSingleName( fieldName ) )
                            return true;

                        if ( IsSingleName( fieldName ) )
                            return false;
                    }
                    break;
                }
            case IConversionOperation conversion:
                if ( conversion.Operand is not null )
                    return EvaluateSelectionMode( conversion.Operand );
                break;
        }

        if ( valueOperation.Type is INamedTypeSymbol enumType
             && enumType.TypeKind == TypeKind.Enum
             && valueOperation.ConstantValue.HasValue )
        {
            var constant = valueOperation.ConstantValue.Value;

            foreach ( var member in enumType.GetMembers().OfType<IFieldSymbol>() )
            {
                if ( member.HasConstantValue
                     && Equals( member.ConstantValue, constant )
                     && IsSingleName( member.Name ) )
                    return false;

                if ( member.HasConstantValue
                     && Equals( member.ConstantValue, constant )
                     && IsMultipleOrNonSingleName( member.Name ) )
                    return true;
            }

            var singleMember = enumType.GetMembers().OfType<IFieldSymbol>()
                .FirstOrDefault( x => x.HasConstantValue && string.Equals( x.Name, "Single", StringComparison.OrdinalIgnoreCase ) );

            if ( singleMember is not null && !Equals( singleMember.ConstantValue, constant ) )
                return true;

            // Value matches none of the enum members we can analyze
            return null;
        }

        return null;
    }

    private static bool IsMultipleOrNonSingleName( string name )
    {
        if ( string.IsNullOrEmpty( name ) )
            return false;

        if ( name.IndexOf( "Multiple", StringComparison.OrdinalIgnoreCase ) >= 0 )
            return true;

        return !name.Equals( "Single", StringComparison.OrdinalIgnoreCase );
    }

    private static bool IsSingleName( string name )
        => string.Equals( name, "Single", StringComparison.OrdinalIgnoreCase );

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
