#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace Blazorise.Docs.Compiler;

internal static class DocsSourceFiles
{
    public static IEnumerable<string> Enumerate( string root, string pattern = "*" )
    {
        foreach ( var file in Directory.EnumerateFiles( root, pattern ) )
        {
            yield return file;
        }

        foreach ( var directory in Directory.EnumerateDirectories( root ) )
        {
            var name = Path.GetFileName( directory );

            if ( name.Equals( "bin", StringComparison.OrdinalIgnoreCase )
                || name.Equals( "obj", StringComparison.OrdinalIgnoreCase )
                || name.Equals( "__SOURCEGENERATED__", StringComparison.OrdinalIgnoreCase )
                || ( File.GetAttributes( directory ) & FileAttributes.ReparsePoint ) != 0 )
            {
                continue;
            }

            foreach ( var file in Enumerate( directory, pattern ) )
            {
                yield return file;
            }
        }
    }

    public static IEnumerable<string> ApiDirectories()
        => new[] { Paths.BlazoriseLibRoot }.Concat( Directory.EnumerateDirectories( Paths.BlazoriseExtensionsRoot )
            .Where( directory => Path.GetFileName( directory ).StartsWith( "Blazorise.", StringComparison.Ordinal )
                && !Path.GetFileName( directory ).StartsWith( "Blazorise.Icons", StringComparison.Ordinal ) ) );

    public static IEnumerable<string> Examples()
        => Enumerate( Paths.DirPath() ).Where( file => Path.GetExtension( file ) is ".razor" or ".snippet" or ".csharp" );
}