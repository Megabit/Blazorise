using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.ApiDocsGenerator.Helpers;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Blazorise.ApiDocsGenerator.Dtos;
using Blazorise.ApiDocsGenerator.Extensions;

namespace Blazorise.ApiDocsGenerator;

[Generator]
public class ComponentsApiDocsGenerator : IIncrementalGenerator
{
    public void Initialize( IncrementalGeneratorInitializationContext context )
    {
        var componentProperties = context.CompilationProvider
            .Select( ( compilation, _ ) => ( compilation,
                components: GetComponentProperties( compilation, GetNamespaceToSearch( compilation ) ).ToImmutableArray() ) );



        context.RegisterSourceOutput( componentProperties, ( ctx, source ) =>
        {
            var (compilation, components) = source;
            INamespaceSymbol namespaceToSearch = GetNamespaceToSearch( compilation );
            var sourceText = GenerateComponentsApiSource( compilation, components, namespaceToSearch );
            ctx.AddSource( "ComponentsApiSource.g.cs", SourceText.From( sourceText, Encoding.UTF8 ) );

            ctx.AddSource( "Log.txt", SourceText.From( Logger.LogMessages, Encoding.UTF8 ) );

        } );
    }

    private INamespaceSymbol GetNamespaceToSearch( Compilation compilation )
    {
        Logger.LogAlways( DateTime.Now.ToLongTimeString() );

        var blazoriseNamespace = compilation.GlobalNamespace
            .GetNamespaceMembers()
            .FirstOrDefault( ns => ns.Name == $"Blazorise" );


        if ( blazoriseNamespace is null ) return null;

        var namespaceToSearch = compilation.Assembly.Name == "Blazorise" ? blazoriseNamespace
            : blazoriseNamespace.GetNamespaceMembers().FirstOrDefault( ns => ns.Name == compilation.Assembly.Name.Split( '.' ).Last() );

        return namespaceToSearch;

    }

    private static IEnumerable<FoundComponent> GetComponentProperties( Compilation compilation, INamespaceSymbol namespaceToSearch )
    {

        Logger.LogAlways( $"Local Namespace:{compilation.Assembly.Name} " );

        var parameterAttributeSymbol = compilation.GetTypeByMetadataName( "Microsoft.AspNetCore.Components.ParameterAttribute" );
        if ( parameterAttributeSymbol == null )
            yield break;

        var baseComponentSymbol = compilation.GetTypeByMetadataName( "Blazorise.BaseComponent" );
        if ( baseComponentSymbol == null )
            yield break;


        Logger.LogAlways( $"To look for {namespaceToSearch} " );
        if ( namespaceToSearch is null )
            yield break;

        foreach ( var type in namespaceToSearch.GetTypeMembers().OfType<INamedTypeSymbol>() )
        {

            if ( type.TypeKind is not TypeKind.Class )
                continue;

            var (inheritsFromBaseComponent, inheritsFromChain) = InheritsFrom( type, baseComponentSymbol );
            if ( !inheritsFromBaseComponent ) continue;

            // Retrieve properties
            var parameterProperties = type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where( p =>
                    p.DeclaredAccessibility == Accessibility.Public &&// Skip accessibility check for interfaces
                    p.GetAttributes().Any( attr => SymbolEqualityComparer.Default.Equals( attr.AttributeClass, parameterAttributeSymbol ) ) &&
                    p.OverriddenProperty == null );

            // Retrieve methods
            var publicMethods = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where( m => m.DeclaredAccessibility == Accessibility.Public &&
                    !m.IsImplicitlyDeclared &&
                    m.MethodKind == MethodKind.Ordinary &&
                    m.OverriddenMethod == null );

            // Logger.LogAlways($"\n------------Class {type.Name}");
            // Logger.LogAlways($"Properties: {string.Join(", ", parameterProperties.Select(p => $"{p.Name} ({p.Type.ToDisplayString()})"))}");
            // Logger.LogAlways($"Methods: {string.Join(", ", publicMethods.Select(m => $"{m.Name} ({m.ReturnType.ToDisplayString()})"))}");
            // Logger.LogAlways($"Inheritance Chain: {string.Join(" -> ", inheritsFromChain.Select(t => t.Name))}");

            yield return new FoundComponent
            (
            Type: type,
            PublicMethods: publicMethods,
            Properties: parameterProperties,
            InheritsFromChain: inheritsFromChain
            );
        }
    }

    private static (bool, IEnumerable<INamedTypeSymbol>) InheritsFrom( INamedTypeSymbol type,
        INamedTypeSymbol baseType )
    {
        List<INamedTypeSymbol> inheritsFromChain = [];
        while ( type != null )
        {
            if ( SymbolEqualityComparer.Default.Equals( type.BaseType, baseType ) )
                return ( true, inheritsFromChain );

            // // Include interfaces from the specified namespace
            // var blazoriseInterfaces = type.Interfaces
            //     .Where( i => i.ContainingNamespace.ToDisplayString().StartsWith( "Blazorise" ) );
            // inheritsFromChain.AddRange( blazoriseInterfaces );

            type = type.BaseType;
            inheritsFromChain.Add( type );
        }
        return ( false, inheritsFromChain.Where( t => t != null ) );
    }

    const string ShouldOnlyBeUsedInternally = "This method is intended for internal framework use only and should not be called directly by user code";
    private static string GenerateComponentsApiSource( Compilation compilation, ImmutableArray<FoundComponent> components, INamespaceSymbol namespaceToSearch )
    {

        IEnumerable<ApiDocsForComponent> componentsData = components.Select( component =>
        {

            string componentType = component.Type.ToStringWithGenerics();
            string componentTypeName = OtherHelpers.GetSimplifiedTypeName( component.Type );
            Logger.IsOn = component.Type.Name == "Button";
            Logger.Log( component.Type.Name );

            var propertiesData = component.Properties.Select( property =>
                InfoExtractor.GetPropertyDetails( compilation, property ) )
                .Where(x=>!x.Summary.Contains(ShouldOnlyBeUsedInternally));

            var methodsData = component.PublicMethods.Select( InfoExtractor.GetMethodDetails )
                .Where(x=>!x.Summary.Contains(ShouldOnlyBeUsedInternally)); ;

            ApiDocsForComponent comp = new(type: componentType, typeName: componentTypeName,
            properties: propertiesData, methods: methodsData,
            inheritsFromChain: component.InheritsFromChain.Select( type => type.ToStringWithGenerics() ));

            return comp;
        } );

        return
            $$"""
              using System;
              using System.Collections.Generic;

              namespace Blazorise.Docs;

              public class ComponentApiSource_ForNamespace_{{namespaceToSearch.Name}}:IComponentsApiDocsSource
              {
                  public  Dictionary<Type, ApiDocsForComponent> Components { get;  } =  
                  new Dictionary<Type, ApiDocsForComponent>
                  {
                      {{componentsData.Where( comp => comp is not null ).Select( comp =>
                      {
                          return $$"""
                                           { typeof({{comp.Type}}),new ApiDocsForComponent(typeof({{comp.Type}}), 
                                           "{{comp.TypeName}}",
                                           new List<ApiDocsForComponentProperty>{
                                               {{
                                                   comp.Properties.Select( prop =>
                                                       $"""

                                                        new ("{prop.Name}",typeof({prop.Type}), "{prop.TypeName}", {prop.DefaultValue},{prop.DefaultValueString}, "{prop.Summary}","{prop.Remarks}", {( prop.IsBlazoriseEnum ? "true" : "false" )}),
                                                        """ ).StringJoin( " " )
                                               }}},
                                             new List<ApiDocsForComponentMethod>{ 
                                             {{
                                                 comp.Methods.Select( method =>
                                                     $$"""

                                                       new ("{{method.Name}}","{{method.ReturnTypeName}}", "{{method.Summary}}" ,"{{method.Remarks}}",
                                                            new List<ApiDocsForComponentMethodParameter>{
                                                       {{
                                                           method.Parameters.Select( param =>
                                                               $"""
                                                                new ("{param.Name}","{param.TypeName}" ),
                                                                """
                                                           ).StringJoin( " " )
                                                       }} }),
                                                       """ ).StringJoin( " " )
                                             }} 
                                             }, 
                                             new List<Type>{  
                                             {{comp.InheritsFromChain.Select( x => $"typeof({x})" ).StringJoin( "," )}}
                                             }
                                       )},

                                   """; 
                      }
                      ).StringJoin( "\n" )}}
                  };
              }
              """;
    }
}