#region Using directives
using System;
using System.Linq;
using System.Reflection;
#endregion

namespace Blazorise.Docs.Compiler;

internal static class AssetVersioning
{
    public const string VersionToken = "__BLAZORISE_VERSION__";

    public const string PackageVersionToken = "__BLAZORISE_PACKAGE_VERSION__";

    private static readonly string packageVersion = typeof( AssetVersioning ).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .Single( attribute => attribute.Key == "BlazorisePackageVersion" ).Value;

    public static bool HasVersionToken( string source )
        => source.Contains( VersionToken, StringComparison.Ordinal )
           || source.Contains( PackageVersionToken, StringComparison.Ordinal );

    public static string ReplaceVersionToken( string source )
        => source.Replace( VersionToken, typeof( AssetVersioning ).Assembly.GetName().Version.ToString( 4 ) )
            .Replace( PackageVersionToken, packageVersion );
}