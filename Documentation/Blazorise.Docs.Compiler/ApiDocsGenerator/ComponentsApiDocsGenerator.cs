#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    readonly List<(string Segment, string Category)> Categories = [
        (Segment: "Source/Blazorise/Themes/", Category: "Theme")
        // Add more categories here if needed
    ];

    readonly string[] skipMethods = ["Dispose", "DisposeAsync", "Equals", "GetHashCode", "GetType", "MemberwiseClone", "ToString", "GetEnumerator"];

    readonly SearchHelper searchHelper;
    private readonly string apiDocsOutputPath;

    #endregion

    #region Constructors

    public ComponentsApiDocsGenerator( string apiDocsOutputPath = null )
    {
        this.apiDocsOutputPath = apiDocsOutputPath;
        searchHelper = new SearchHelper();
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

        string outputRoot = GetApiDocsOutputPath();
        if ( string.IsNullOrWhiteSpace( outputRoot ) )
        {
            Console.WriteLine( "Error generating ApiDocs. Output path is not set." );
            return false;
        }

        PrepareApiDocsOutput( outputRoot );

        List<ApiDocsForComponent> allComponentsData = new List<ApiDocsForComponent>();

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

            ImmutableArray<ComponentInfo> componentInfo = [.. GetComponentsInfo( compilation, namespaceToSearch )];
            List<ApiDocsForComponent> componentsData = BuildComponentsData( compilation, componentInfo );
            allComponentsData.AddRange( componentsData );
            string sourceText = GenerateComponentsApiSource( componentsData, assemblyName );

            string outputPath = Path.Join( outputRoot, $"{assemblyName}.ApiDocs.cs" );

            File.WriteAllText( outputPath, sourceText );
            Console.WriteLine( $"API Docs generated for {assemblyName} at {outputPath}. {sourceText.Length} characters." );
        }

        WriteDocsApiIndex( allComponentsData );
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
            Subcategory: typeQualification.Subcategory,
            SearchUrl: searchHelper.GetSearchUrl( type ),
            Summary:
            $"""" 
                    $"""
                    {StringHelpers.ExtractFromXmlComment( type, ExtractorParts.Summary )}
                    """
             """"
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
            if ( SymbolEqualityComparer.Default.Equals( type, baseType ) )
            {
                inheritsFromChain.Add( type );
                return new( true, skipParamAndComponentCheck, inheritsFromChain, Category: category.category, Subcategory: category.subcategory );
            }

            if ( type?.Name.Split( "." ).Last() == "ComponentBase" ) //for this to work, the inheritance (:ComponentBase) must be specified in .cs file.
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

            foreach ( var (Segment, Category) in Categories )
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

    private static List<ApiDocsForComponent> BuildComponentsData( Compilation compilation, ImmutableArray<ComponentInfo> components )
    {
        List<ApiDocsForComponent> componentsData = new List<ApiDocsForComponent>();

        foreach ( ComponentInfo component in components )
        {
            string componentType = component.Type.ToStringWithGenerics();
            string componentTypeName = StringHelpers.GetSimplifiedTypeName( component.Type, withoutGenerics: true );

            List<ApiDocsForComponentProperty> propertiesData = component.Properties
                .Select( property => InfoExtractor.GetPropertyDetails( compilation, property ) )
                .Where( x => !x.Summary.Contains( ShouldOnlyBeUsedInternally ) )
                .ToList();

            List<ApiDocsForComponentMethod> methodsData = component.PublicMethods
                .Select( InfoExtractor.GetMethodDetails )
                .Where( x => !x.Summary.Contains( ShouldOnlyBeUsedInternally ) )
                .ToList();

            ApiDocsForComponent comp = new ApiDocsForComponent( type: componentType, typeName: componentTypeName,
                properties: propertiesData, methods: methodsData,
                inheritsFromChain: component.InheritsFromChain.Select( type => type.ToStringWithGenerics() ),
                component.Category, component.Subcategory, component.SearchUrl, component.Summary );

            componentsData.Add( comp );
        }

        return componentsData;
    }

    private static string GenerateComponentsApiSource( IEnumerable<ApiDocsForComponent> componentsData, string assemblyName )
    {
        return
              $$"""
              using System;
              using System.Collections;
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
                                             {{FilterApiDocsInheritsFromChain( comp.InheritsFromChain ).Select( x => $"typeof({x})" ).StringJoin( "," )}}
                                             },
                                             {{comp.Summary}}
                                             
                                             {{( comp.Category is null ? "" : $""","{comp.Category}" {( comp.Subcategory is null ? "" : $""", "{comp.Subcategory}" """ )} """ )}}
                                             {{( string.IsNullOrWhiteSpace( comp.SearchUrl ) ? "" : $""", searchUrl:"{comp.SearchUrl}" """ )}}
                                       )},

                                   """;
                      }
                      ).StringJoin( "\n" )}}
                  };
              }
              """;
    }

    private static void WriteDocsApiIndex( List<ApiDocsForComponent> componentsData )
    {
        if ( componentsData is null || componentsData.Count == 0 )
            return;

        Dictionary<string, ApiDocsForComponent> componentsByType = new Dictionary<string, ApiDocsForComponent>( StringComparer.Ordinal );

        foreach ( ApiDocsForComponent component in componentsData )
        {
            if ( component is null )
                continue;

            if ( !componentsByType.ContainsKey( component.Type ) )
                componentsByType.Add( component.Type, component );
        }

        List<DocsApiComponent> docsComponents = new List<DocsApiComponent>();

        foreach ( ApiDocsForComponent component in componentsByType.Values )
        {
            if ( !ShouldIncludeInDocsApiIndex( component ) )
                continue;

            DocsApiComponent docsComponent = BuildDocsApiComponent( component, componentsByType );
            if ( docsComponent is not null )
                docsComponents.Add( docsComponent );
        }

        DocsApiIndex index = new DocsApiIndex
        {
            GeneratedUtc = DateTime.UtcNow.ToString( "O", CultureInfo.InvariantCulture ),
            Components = docsComponents
        };

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string json = JsonSerializer.Serialize( index, options );
        string outputPath = Paths.DocsApiIndexFilePath();
        string outputDirectory = Path.GetDirectoryName( outputPath );

        if ( !Directory.Exists( outputDirectory ) )
            Directory.CreateDirectory( outputDirectory );

        GeneratedJsonFileWriter.WriteIfChangedIgnoringGeneratedUtc( outputPath, json );
    }

    private static bool ShouldIncludeInDocsApiIndex( ApiDocsForComponent component )
    {
        if ( component is null )
            return false;

        if ( string.IsNullOrWhiteSpace( component.TypeName ) )
            return false;

        if ( component.TypeName.StartsWith( "Base", StringComparison.Ordinal ) )
            return false;

        return true;
    }

    private static IEnumerable<string> FilterApiDocsInheritsFromChain( IEnumerable<string> inheritsFromChain )
    {
        if ( inheritsFromChain is null )
            return Array.Empty<string>();

        List<string> filtered = new List<string>();

        foreach ( string typeName in inheritsFromChain )
        {
            if ( IsBaseComponentType( typeName ) )
                continue;

            filtered.Add( typeName );
        }

        return filtered;
    }

    private static bool IsBaseComponentType( string typeName )
    {
        if ( string.IsNullOrWhiteSpace( typeName ) )
            return false;

        string normalized = typeName.Trim();

        if ( normalized.StartsWith( "global::", StringComparison.Ordinal ) )
            normalized = normalized.Substring( "global::".Length );

        return normalized == "Blazorise.BaseComponent"
            || normalized.StartsWith( "Blazorise.BaseComponent<", StringComparison.Ordinal );
    }

    private static DocsApiComponent BuildDocsApiComponent( ApiDocsForComponent component, Dictionary<string, ApiDocsForComponent> componentsByType )
    {
        if ( component is null )
            return null;

        List<DocsApiProperty> mergedProperties = BuildMergedProperties( component, componentsByType );
        List<DocsApiProperty> parameters = new List<DocsApiProperty>();
        List<DocsApiProperty> events = new List<DocsApiProperty>();

        foreach ( DocsApiProperty property in mergedProperties )
        {
            if ( IsEventType( property.Type ) )
                events.Add( property );
            else
                parameters.Add( property );
        }

        List<DocsApiMethod> methods = BuildMergedMethods( component, componentsByType );

        DocsApiComponent docsComponent = new DocsApiComponent
        {
            Type = component.Type,
            TypeName = component.TypeName,
            Summary = NormalizeDocText( component.Summary ),
            Category = component.Category,
            Subcategory = component.Subcategory,
            SearchUrl = component.SearchUrl,
            Parameters = parameters,
            Events = events,
            Methods = methods
        };

        return docsComponent;
    }

    private static List<DocsApiProperty> BuildMergedProperties( ApiDocsForComponent component, Dictionary<string, ApiDocsForComponent> componentsByType )
    {
        List<DocsApiProperty> properties = new List<DocsApiProperty>();

        if ( component.Properties is not null )
        {
            foreach ( ApiDocsForComponentProperty property in component.Properties )
            {
                DocsApiProperty docsProperty = BuildDocsApiProperty( property, null, component.TypeName );
                properties.Add( docsProperty );
            }
        }

        if ( component.InheritsFromChain is null )
            return properties;

        foreach ( string baseType in component.InheritsFromChain )
        {
            if ( string.IsNullOrWhiteSpace( baseType ) )
                continue;

            if ( !componentsByType.TryGetValue( baseType, out ApiDocsForComponent baseComponent ) )
                continue;

            if ( baseComponent.Properties is null )
                continue;

            string baseTypeName = baseComponent.TypeName;

            foreach ( ApiDocsForComponentProperty property in baseComponent.Properties )
            {
                DocsApiProperty docsProperty = BuildDocsApiProperty( property, baseTypeName, component.TypeName );
                properties.Add( docsProperty );
            }
        }

        return properties;
    }

    private static DocsApiProperty BuildDocsApiProperty( ApiDocsForComponentProperty property, string baseTypeName, string derivedTypeName )
    {
        string summary = ReplaceTypeName( property.Summary, baseTypeName, derivedTypeName );
        string remarks = ReplaceTypeName( property.Remarks, baseTypeName, derivedTypeName );

        DocsApiProperty docsProperty = new DocsApiProperty
        {
            Name = property.Name,
            Type = property.Type,
            TypeName = property.TypeName,
            DefaultValue = property.DefaultValue,
            Summary = NormalizeDocText( summary ),
            Remarks = NormalizeDocText( remarks ),
            IsBlazoriseEnum = property.IsBlazoriseEnum
        };

        return docsProperty;
    }

    private static List<DocsApiMethod> BuildMergedMethods( ApiDocsForComponent component, Dictionary<string, ApiDocsForComponent> componentsByType )
    {
        List<DocsApiMethod> methods = new List<DocsApiMethod>();

        if ( component.Methods is not null )
        {
            foreach ( ApiDocsForComponentMethod method in component.Methods )
            {
                methods.Add( BuildDocsApiMethod( method ) );
            }
        }

        if ( component.InheritsFromChain is null )
            return methods;

        foreach ( string baseType in component.InheritsFromChain )
        {
            if ( string.IsNullOrWhiteSpace( baseType ) )
                continue;

            if ( !componentsByType.TryGetValue( baseType, out ApiDocsForComponent baseComponent ) )
                continue;

            if ( baseComponent.Methods is null )
                continue;

            foreach ( ApiDocsForComponentMethod method in baseComponent.Methods )
            {
                methods.Add( BuildDocsApiMethod( method ) );
            }
        }

        return methods;
    }

    private static DocsApiMethod BuildDocsApiMethod( ApiDocsForComponentMethod method )
    {
        List<DocsApiMethodParameter> parameters = new List<DocsApiMethodParameter>();

        if ( method.Parameters is not null )
        {
            foreach ( ApiDocsForComponentMethodParameter parameter in method.Parameters )
            {
                DocsApiMethodParameter docsParameter = new DocsApiMethodParameter
                {
                    Name = parameter.Name,
                    TypeName = parameter.TypeName
                };
                parameters.Add( docsParameter );
            }
        }

        DocsApiMethod docsMethod = new DocsApiMethod
        {
            Name = method.Name,
            ReturnTypeName = method.ReturnTypeName,
            Summary = NormalizeDocText( method.Summary ),
            Remarks = NormalizeDocText( method.Remarks ),
            Parameters = parameters
        };

        return docsMethod;
    }

    private static string ReplaceTypeName( string value, string baseTypeName, string derivedTypeName )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return value;

        if ( string.IsNullOrWhiteSpace( baseTypeName ) || string.IsNullOrWhiteSpace( derivedTypeName ) )
            return value;

        return value.Replace( baseTypeName, derivedTypeName );
    }

    private static string NormalizeDocText( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return null;

        return value.Trim();
    }

    private static bool IsEventType( string typeName )
    {
        if ( string.IsNullOrWhiteSpace( typeName ) )
            return false;

        string trimmed = typeName.Trim();

        if ( trimmed.StartsWith( "global::", StringComparison.Ordinal ) )
            trimmed = trimmed.Substring( "global::".Length );

        int lastDot = trimmed.LastIndexOf( '.' );
        if ( lastDot >= 0 && lastDot + 1 < trimmed.Length )
            trimmed = trimmed.Substring( lastDot + 1 );

        int genericIndex = trimmed.IndexOf( '<' );
        if ( genericIndex >= 0 )
            trimmed = trimmed.Substring( 0, genericIndex );

        return trimmed is "EventCallback" or "Action" or "Func";
    }

    private string GetApiDocsOutputPath()
    {
        string outputPath = string.IsNullOrWhiteSpace( apiDocsOutputPath )
            ? Paths.ApiDocsPath
            : apiDocsOutputPath;

        if ( string.IsNullOrWhiteSpace( outputPath ) )
            return null;

        return Path.GetFullPath( outputPath );
    }

    private static void PrepareApiDocsOutput( string outputPath )
    {
        if ( string.IsNullOrWhiteSpace( outputPath ) )
            return;

        if ( Directory.Exists( outputPath ) )
        {
            string[] existingFiles = Directory.GetFiles( outputPath, "*.ApiDocs.cs", SearchOption.TopDirectoryOnly );
            foreach ( string file in existingFiles )
                File.Delete( file );
        }

        Directory.CreateDirectory( outputPath );
    }

    #endregion
}