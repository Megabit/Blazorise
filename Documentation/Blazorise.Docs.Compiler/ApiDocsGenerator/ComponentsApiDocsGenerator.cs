#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Blazorise.Docs.Compiler.ApiDocsGenerator.Dtos;
using Blazorise.Docs.Compiler.ApiDocsGenerator.Extensions;
using Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator;

public class ComponentsApiDocsGenerator
{
    #region Members

    private Assembly aspNetCoreComponentsAssembly;

    private CSharpCompilation blazoriseCompilation;

    private XmlDocumentationProvider aspnetCoreDocumentationProvider;


    private Assembly systemRuntimeAssembly;
    private XmlDocumentationProvider systemRuntimeDocumentationProvider;

    const string ShouldOnlyBeUsedInternally = "This method is intended for internal framework use only and should not be called directly by user code";

    readonly List<(string Segment, string Category)> categories = [
        (Segment: "Source/Blazorise/Themes/", Category: "Theme")
        // Add more categories here if needed
    ];

    readonly string[] skipMethods = ["Dispose", "DisposeAsync", "Equals", "GetHashCode", "GetType", "MemberwiseClone", "ToString", "GetEnumerator"];

    #endregion

    #region Constructors

    public ComponentsApiDocsGenerator()
    {
        var aspnetCoreAssemblyName = typeof( Microsoft.AspNetCore.Components.ParameterAttribute ).Assembly.GetName().Name;
        aspNetCoreComponentsAssembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault( a => a.GetName().Name == aspnetCoreAssemblyName );

        systemRuntimeAssembly = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault( a => a.GetName().Name == "System.Runtime" );

        if ( systemRuntimeAssembly is not null )
        {
            systemRuntimeDocumentationProvider = XmlDocumentationProvider.CreateFromFile( $"{Path.GetFullPath( "." )}/System.Runtime.xml" );
        }
        if ( aspNetCoreComponentsAssembly != null )
        {
            // Replace the .dll extension with .xml to get the documentation file path
            string xmlDocumentationPath = Path.ChangeExtension( aspNetCoreComponentsAssembly.Location, ".xml" );
            aspnetCoreDocumentationProvider = XmlDocumentationProvider.CreateFromFile( xmlDocumentationPath );
        }
        //get the blazorise compilation, it's needed for every extension.
        blazoriseCompilation = GetCompilation( Paths.BlazoriseLibRoot, "Blazorise", true );
    }

    #endregion

    #region Methods

    public bool Execute()
    {
        if ( aspNetCoreComponentsAssembly is null )
        {
            Console.WriteLine( $"Error generating ApiDocs. Cannot find ASP.NET Core assembly." );
            return false;
        }
        if ( blazoriseCompilation is null )
        {
            Console.WriteLine( $"Error generating ApiDocs. Cannot find Blazorise assembly." );
            return false;
        }
        if ( !Directory.Exists( Paths.BlazoriseExtensionsRoot ) )
        {
            Console.WriteLine( $"Directory for extensions does not exist: {Paths.BlazoriseExtensionsRoot}" );
            return false;
        }

        //directories where to load the source code from one by one
        string[] inputLocations = [Paths.BlazoriseLibRoot, .. Directory.GetDirectories( Paths.BlazoriseExtensionsRoot )];

        foreach ( var inputLocation in inputLocations )
        {
            string assemblyName = Path.GetFileName( inputLocation ); // Use directory name as assembly name

            CSharpCompilation compilation = inputLocation.EndsWith( "Blazorise" )
                ? blazoriseCompilation // the case for getting components from Blazorise
                : GetCompilation( inputLocation, assemblyName );

            INamespaceSymbol namespaceToSearch = FindNamespace( compilation, assemblyName ); // e.g. Blazorise.Animate

            if ( namespaceToSearch is null || namespaceToSearch.ToDisplayString().Contains( "Blazorise.Icons" ) )
                continue;

            IEnumerable<ComponentInfo> componentInfo = GetComponentsInfo( compilation, namespaceToSearch );
            string sourceText = GenerateComponentsApiSource( compilation, [.. componentInfo], assemblyName );

            if ( !Directory.Exists( Paths.ApiDocsPath ) ) // BlazoriseDocs.ApiDocs
                Directory.CreateDirectory( Paths.ApiDocsPath );

            string outputPath = Path.Join( Paths.ApiDocsPath, $"{assemblyName}.ApiDocs.cs" );

            File.WriteAllText( outputPath, sourceText );
            Console.WriteLine( $"API Docs generated for {assemblyName} at {outputPath}. {sourceText.Length} characters." );
        }

        return true;
    }

    //namespace are divided in chunks (Blazorise.Animate is under Blazorise...)
    INamespaceSymbol FindNamespace( Compilation compilation, string namespaceName, INamespaceSymbol namespaceToSearch = null )
    {
        namespaceToSearch ??= compilation.GlobalNamespace
            .GetNamespaceMembers()
            .FirstOrDefault( ns => ns.Name == "Blazorise" );

        if ( namespaceToSearch is null )
            throw new Exception( $"Unable to find namespace {namespaceName}." );

        if ( namespaceToSearch.ToDisplayString() == namespaceName )
            return namespaceToSearch;

        foreach ( var childNamespace in namespaceToSearch.GetNamespaceMembers() )
        {
            var result = FindNamespace( compilation, namespaceName, childNamespace );

            if ( result is not null )
                return result;
        }

        return null;
    }

    private CSharpCompilation GetCompilation( string inputLocation, string assemblyName, bool isBlazoriseAssembly = false )
    {
        var sourceFiles = Directory.GetFiles( inputLocation, "*.cs", SearchOption.AllDirectories );

        List<MetadataReference> references =
        [
            MetadataReference.CreateFromFile( systemRuntimeAssembly.Location, documentation:systemRuntimeDocumentationProvider ), // Microsoft.AspNetCore.Components
            MetadataReference.CreateFromFile( aspNetCoreComponentsAssembly.Location, documentation:aspnetCoreDocumentationProvider ), // Microsoft.AspNetCore.Components
        ];
        if ( !isBlazoriseAssembly ) //get Blazorise assembly as reference (for extensions)
            references.Add( blazoriseCompilation.ToMetadataReference() );

        var syntaxTrees = sourceFiles.Select( file => CSharpSyntaxTree.ParseText( File.ReadAllText( file ), path: file ) );

        var compilation = CSharpCompilation.Create(
        assemblyName,
        syntaxTrees,
        references.ToImmutableArray(),
        new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary )
        );
        return compilation;
    }

    private IEnumerable<ComponentInfo> GetComponentsInfo( Compilation compilation, INamespaceSymbol namespaceToSearch )
    {
        var baseComponentSymbol = compilation.GetTypeByMetadataName( "Blazorise.BaseComponent" );

        foreach ( var type in namespaceToSearch.GetTypeMembers().OfType<INamedTypeSymbol>() )
        {
            TypeQualification typeQualification = QualifiesForApiDocs( type, baseComponentSymbol );
            if ( !typeQualification.QualifiesForApiDocs )
                continue;

            // Retrieve properties
            var parameterProperties = type.GetMembers()
                .OfType<IPropertySymbol>()
                .Where( p =>
                    p.DeclaredAccessibility == Accessibility.Public &&
                    ( typeQualification.SkipParamCheck || p.GetAttributes().Any( attr =>
                        attr.AttributeClass?.ToDisplayString() == "Microsoft.AspNetCore.Components.ParameterAttribute" ) ) &&
                    p.OverriddenProperty == null );

            // Retrieve methods
            var publicMethods = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where( m => m.DeclaredAccessibility == Accessibility.Public &&
                    !m.IsImplicitlyDeclared &&
                    m.MethodKind == MethodKind.Ordinary &&
                    m.OverriddenMethod == null &&
                    !skipMethods.Contains( m.Name )
                    );

            yield return new ComponentInfo
            (
            Type: type,
            PublicMethods: publicMethods,
            Properties: parameterProperties,
            InheritsFromChain: typeQualification.NamedTypeSymbols ?? [],
            Category: typeQualification.Category,
            Subcategory: typeQualification.Subcategory
            );
        }
    }

    record TypeQualification( bool QualifiesForApiDocs, bool SkipParamCheck = false, IEnumerable<INamedTypeSymbol> NamedTypeSymbols = null, string Category = null, string Subcategory = null );

    /// <summary>
    /// get the chain of inheritance to the BaseComponent or ComponentBase
    /// Only return true if implements IComponent (that is the case for all BaseComponent and ComponentBase)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    private TypeQualification QualifiesForApiDocs( INamedTypeSymbol type,
        INamedTypeSymbol baseType )
    {

        var category = GetCategoryAndSubcategory( type );

        (bool continueProcessing, bool skipParamAndComponentCheck) = type switch
        {
            _ when type.TypeKind != TypeKind.Class || type.DeclaredAccessibility != Accessibility.Public => (false, false),
            _ when type.Name.StartsWith( '_' ) => (false, false),
            _ when category.category is not null => (true, true),
            _ when type.Name.EndsWith( "Options" ) => (true, true),
            _ when type.Name.EndsWith( "RouterTabsPageAttribute" ) => (true, true),
            _ when !type.AllInterfaces.Any( i => i.Name == "IComponent" ) => (false, false),
            _ => (true, false)
        };

        if ( !continueProcessing )
            return new( false );

        List<INamedTypeSymbol> inheritsFromChain = [];
        while ( type != null )
        {
            type = type.BaseType;
            if ( type?.Name.Split( "." ).Last() == "ComponentBase" //for this to work, the inheritance (:ComponentBase) must be specified in .cs file.
                || SymbolEqualityComparer.Default.Equals( type, baseType )
                )
                return new( true, skipParamAndComponentCheck, inheritsFromChain, Category: category.category, Subcategory: category.subcategory );
            inheritsFromChain.Add( type );
        }
        return new( true, SkipParamCheck: skipParamAndComponentCheck, Category: category.category, Subcategory: category.subcategory );
    }

    (string category, string subcategory) GetCategoryAndSubcategory( INamedTypeSymbol typeSymbol )
    {
        foreach ( var syntaxRef in typeSymbol.DeclaringSyntaxReferences )
        {
            string filePath = syntaxRef.SyntaxTree.FilePath;
            string normalizedFilePath = Path.GetFullPath( filePath ).Replace( Path.DirectorySeparatorChar, '/' );

            foreach ( var (Segment, Category) in categories )
            {
                if ( normalizedFilePath.Contains( Segment ) )
                {
                    string subcategory = null;
                    int startIndex = normalizedFilePath.IndexOf( Segment ) + Segment.Length;
                    string remainingPath = normalizedFilePath[startIndex..];

                    // Extract only the first folder after "Themes/" (ignore file names)
                    string[] pathParts = remainingPath.Split( new[] { '/' }, StringSplitOptions.RemoveEmptyEntries );

                    if ( pathParts.Length > 1 ) // Ensures it's not just the filename
                        subcategory = pathParts[0]; // First folder after the segment

                    return (Category, subcategory);
                }
            }
        }

        return (null, null);
    }

    private static string GenerateComponentsApiSource( Compilation compilation, ImmutableArray<ComponentInfo> components, string assemblyName )
    {
        IEnumerable<ApiDocsForComponent> componentsData = components.Select( component =>
        {
            string componentType = component.Type.ToStringWithGenerics();
            string componentTypeName = StringHelpers.GetSimplifiedTypeName( component.Type, withoutGenerics: true );

            var propertiesData = component.Properties.Select( property =>
                    InfoExtractor.GetPropertyDetails( compilation, property ) )
                .Where( x => !x.Summary.Contains( ShouldOnlyBeUsedInternally ) );

            var methodsData = component.PublicMethods.Select( InfoExtractor.GetMethodDetails )
                .Where( x => !x.Summary.Contains( ShouldOnlyBeUsedInternally ) );

            ApiDocsForComponent comp = new( type: componentType, typeName: componentTypeName,
            properties: propertiesData, methods: methodsData,
            inheritsFromChain: component.InheritsFromChain.Select( type => type.ToStringWithGenerics() ), component.Category, component.Subcategory );

            return comp;
        } );

        return
            $$"""
              using System;
              using System.Collections.Generic;
              using System.Collections.Generic;
              using System.Linq.Expressions;
              using System.Windows.Input;
              using System.Threading.Tasks;
              using Microsoft.AspNetCore.Components.Forms;
              using Blazorise.Docs.Models.ApiDocsDtos;
              using Blazorise.Charts;

              namespace Blazorise.Docs.ApiDocs;

              public class ComponentApiSource_ForNamespace_{{assemblyName.Replace( ".", "_" )}}:IComponentsApiDocsSource
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
                                               {{comp.Properties.Select( prop =>
                                                   $"""

                                                    new ("{prop.Name}",typeof({prop.Type}), "{prop.TypeName}",{prop.DefaultValueString}, "{prop.Summary}","{prop.Remarks}", {( prop.IsBlazoriseEnum ? "true" : "false" )}),
                                                    """ ).StringJoin( " " )}}},
                                             new List<ApiDocsForComponentMethod>{ 
                                             {{comp.Methods.Select( method =>
                                                 $$"""

                                                   new ("{{method.Name}}","{{method.ReturnTypeName}}", "{{method.Summary}}" ,"{{method.Remarks}}",
                                                        new List<ApiDocsForComponentMethodParameter>{
                                                   {{method.Parameters.Select( param =>
                                                       $"""
                                                        new ("{param.Name}","{param.TypeName}" ),
                                                        """
                                                   ).StringJoin( " " )}} }),
                                                   """ ).StringJoin( " " )}} 
                                             }, 
                                             new List<Type>{  
                                             {{comp.InheritsFromChain.Select( x => $"typeof({x})" ).StringJoin( "," )}}
                                             }
                                             
                                             {{( comp.Category is null ? "" : $""","{comp.Category}" {( comp.Subcategory is null ? "" : $""", "{comp.Subcategory}" """ )} """ )}}
                                       )},

                                   """;
                      }
                      ).StringJoin( "\n" )}}
                  };
              }
              """;
    }

    #endregion
}