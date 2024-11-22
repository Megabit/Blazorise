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
            .Select( ( compilation, _ ) => ( compilation, components: GetComponentProperties( compilation,GetNamespaceToSearch( compilation) ).ToImmutableArray() ) );



        context.RegisterSourceOutput( componentProperties, ( ctx, source ) =>
        {
            Logger.LogAlways( DateTime.Now.ToLongTimeString() );
            var (compilation, components) = source;
            INamespaceSymbol namespaceToSearch = GetNamespaceToSearch( compilation);
            var sourceText = GenerateComponentsApiSource( compilation, components, namespaceToSearch );
            ctx.AddSource( "ComponentsApiSource.g.cs", SourceText.From( sourceText, Encoding.UTF8 ) );

            ctx.AddSource( "Log.txt", SourceText.From( Logger.LogMessages, Encoding.UTF8 ) );

        } );
    }

    private INamespaceSymbol GetNamespaceToSearch( Compilation compilation )
    {
        var blazoriseNamespace = compilation.GlobalNamespace
            .GetNamespaceMembers()
            .FirstOrDefault( ns => ns.Name == $"Blazorise" );


        if ( blazoriseNamespace is null ) return null;
            
        var namespaceToSearch = compilation.Assembly.Name == "Blazorise" ? blazoriseNamespace
            : blazoriseNamespace.GetNamespaceMembers().FirstOrDefault( ns => ns.Name == compilation.Assembly.Name.Split( '.' ).Last() );
        
        return namespaceToSearch;        

    }

    private static IEnumerable< FoundComponent> GetComponentProperties( Compilation compilation, INamespaceSymbol namespaceToSearch )
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

            if ( type.TypeKind != TypeKind.Class )
                continue;
            var (inheritsFromBaseComponent, inheritsFromChain) = InheritsFrom( type, baseComponentSymbol ) ;
            if(!inheritsFromBaseComponent) continue;
            
            Logger.LogAlways( $"Type {type.Name} base {type.BaseType}" );
            var parameterProperties = type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where( p => p.DeclaredAccessibility == Accessibility.Public &&
                    p.GetAttributes().Any( attr => SymbolEqualityComparer.Default.Equals( attr.AttributeClass, parameterAttributeSymbol ) ) );

            // Retrieve public methods
            var publicMethods = type
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where( m => m.DeclaredAccessibility == Accessibility.Public &&
                    !m.IsImplicitlyDeclared &&
                    m.MethodKind == MethodKind.Ordinary );


            yield return new FoundComponent
            (
                Type : type,  
                PublicMethods : publicMethods,
                Properties : parameterProperties,
                InheritsFromChain : inheritsFromChain
            );
        }
    }

    private static (bool, IEnumerable<INamedTypeSymbol>) InheritsFrom( INamedTypeSymbol type, INamedTypeSymbol baseType )
    {
        List<INamedTypeSymbol> inheritsFromChain = [];
        while ( type != null )
        {
            if ( SymbolEqualityComparer.Default.Equals( type.BaseType, baseType ) )
                return ( true, inheritsFromChain );

            type = type.BaseType;
            inheritsFromChain.Add( type );
        }
        return ( false, [] );
    }

    private static string GenerateComponentsApiSource( Compilation compilation, ImmutableArray<FoundComponent> components, INamespaceSymbol namespaceToSearch )
    {
        
        Logger.LogAlways( components.Count().ToString() );
        IEnumerable<ApiDocsForComponent> componentsData = components.Select( component =>
        {

            string componentType = component.Type.ToStringWithGenerics();
            string componentTypeName = OtherHelpers.GetSimplifiedTypeName( component.Type );
            Logger.IsOn = component.Type.Name == "Button";
            Logger.Log( component.Type.Name );

            var propertiesData = component.Properties.Select( property =>
                InfoExtractor.GetPropertyDetails( compilation, property ) );

            var methodsData = component.PublicMethods.Select( method =>
                InfoExtractor.GetMethodDetails( compilation, method ) );

            ApiDocsForComponent comp = new(type: componentType, typeName: componentTypeName,
            properties: propertiesData, methods: methodsData,
            inheritsFromChain: component.InheritsFromChain.Select( type => type.ToStringWithGenerics() ) );

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

                                                        new ("{prop.Name}",typeof({prop.Type}), "{prop.TypeName}", {prop.DefaultValue},{prop.DefaultValueString}, "{prop.Description}", {( prop.IsBlazoriseEnum ? "true" : "false" )}),
                                                        """ ).StringJoin( " " )
                                               }}},
                                             new List<ApiDocsForComponentMethod>{
                                             {{
                                                 comp.Methods.Select( method =>
                                                     $$"""

                                                       new ("{{method.Name}}","{{method.ReturnTypeName}}", "{{method.Description}}",
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
                                             {{comp.InheritsFromChain.Select(x=> $"typeof({x})").StringJoin(",") }}
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

public record FoundComponent( INamedTypeSymbol Type, IEnumerable<IPropertySymbol> Properties, IEnumerable<IMethodSymbol> PublicMethods, IEnumerable<INamedTypeSymbol> InheritsFromChain )
{
    public INamedTypeSymbol Type { get; } = Type;
    public IEnumerable<IPropertySymbol> Properties  { get; } = Properties;
    public IEnumerable<IMethodSymbol> PublicMethods { get; } = PublicMethods;
    public IEnumerable<INamedTypeSymbol> InheritsFromChain { get; } = InheritsFromChain;
}