#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
#endregion

namespace Blazorise.Docs.Compiler.ApiDocsGenerator.Helpers;

public class SearchHelper
{
    /// <summary>
    /// Resolves the appropriate search URL for a given type based on its source file location.
    /// The resolution is driven by a mapping of file path segments to base documentation URLs,
    /// combined with configurable strategies that define how the final URL should be constructed
    /// (e.g., appending the type name or using the directory name).
    /// </summary>
    /// <param name="typeSymbol">The type symbol representing the component or class to resolve the URL for.</param>
    /// <returns>The resolved documentation search URL, or <c>null</c> if no matching path was found.</returns>
    public string GetSearchUrl( INamedTypeSymbol typeSymbol )
    {
        IEnumerable<SyntaxReference> orderedSyntaxReferences = typeSymbol.DeclaringSyntaxReferences
            .OrderBy( syntaxRef => NormalizePathForOrdering( syntaxRef.SyntaxTree.FilePath ), StringComparer.Ordinal );

        foreach ( SyntaxReference syntaxRef in orderedSyntaxReferences )//the symbol can be in multiple files (partial classes), this kinda ignores the case when different location would break the search 
        {
            string filePath = syntaxRef.SyntaxTree.FilePath;
            string normalizedFilePath = Path.GetFullPath( filePath ).Replace( Path.DirectorySeparatorChar, '/' );

            var link = sortedPathToSearchUrlsMap.FirstOrDefault( entry => normalizedFilePath.Contains( entry.Segment ) );
            if ( link is null )
                continue;

            return link.Strategy switch
            {
                PathResolverStrategy.DirectoryNameToKebabCase => CombineUrl( link.SearchUrlBase, ToKebabCase( Path.GetFileName( Path.GetDirectoryName( normalizedFilePath ) ) ?? "" ) ),
                PathResolverStrategy.Default => link.SearchUrlBase,
                _ => link.SearchUrlBase
            };
        }

        return null;
    }

    // Match the most specific (longest) path segment first.
    // For example, a file in "Source/Blazorise/Themes/Colors/ThemeColor.cs"
    // will match the segment "Source/Blazorise/Themes/Colors/" over "Source/Blazorise/Themes/"
    readonly List<SearchPathMapping> sortedPathToSearchUrlsMap = PathToSearchUrlsMap
                                                                  .OrderByDescending( entry => entry.Segment.Length )
                                                                  .ToList();

    static readonly List<SearchPathMapping> PathToSearchUrlsMap =
    [
        new( "Source/Blazorise/Components/", "docs/components/" , PathResolverStrategy.DirectoryNameToKebabCase),
        new( "Source/Blazorise/Themes/Colors/", "docs/theming/api/colors" ),
        new( "Source/Blazorise/Themes/Options/", "docs/theming/api/options" ),
        new( "Source/Blazorise/Themes/Palettes/", "docs/theming/api/palettes" ),
        new( "Source/Blazorise/Themes/", "docs/theming/api" )
    ];

    public enum PathResolverStrategy
    {
        Default, // Use the base URL only
        DirectoryNameToKebabCase // Append the containing folder name to the base URL // Append the type name to the base URL
    }

    //FileInput -> file-edit
    static string ToKebabCase( string input ) =>
        string.Concat(
        input.Select( ( c, i ) =>
        char.IsUpper( c )
            ? ( i > 0 ? "-" : "" ) + char.ToLowerInvariant( c )
            : c.ToString()
        ) );

    static string CombineUrl( string baseUrl, string segment )
    {
        string left = ( baseUrl ?? string.Empty ).TrimEnd( '/' );
        string right = ( segment ?? string.Empty ).TrimStart( '/' );

        if ( left.Length == 0 )
            return right;

        if ( right.Length == 0 )
            return left;

        return $"{left}/{right}";
    }

    static string NormalizePathForOrdering( string path )
    {
        if ( string.IsNullOrWhiteSpace( path ) )
            return string.Empty;

        return path.Replace( '\\', '/' );
    }
}

public record SearchPathMapping( string Segment, string SearchUrlBase, SearchHelper.PathResolverStrategy Strategy = SearchHelper.PathResolverStrategy.Default );