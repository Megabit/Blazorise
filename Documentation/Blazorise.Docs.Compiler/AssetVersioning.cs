namespace Blazorise.Docs.Compiler;

internal static class AssetVersioning
{
    public const string VersionToken = "__BLAZORISE_VERSION__";

    public static string ReplaceVersionToken( string source )
        => source.Replace( VersionToken, typeof( AssetVersioning ).Assembly.GetName().Version.ToString( 4 ) );
}