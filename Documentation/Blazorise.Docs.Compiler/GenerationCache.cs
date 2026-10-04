#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
#endregion

namespace Blazorise.Docs.Compiler;

internal static class GenerationCache
{
    #region Members

    private static readonly Lazy<string> compilerFingerprint = new( () => FingerprintFiles( Directory.EnumerateFiles( AppContext.BaseDirectory )
        .Where( file => Path.GetExtension( file ) is ".dll" or ".xml" or ".json" ), Environment.Version.ToString() ) );

    #endregion

    #region Methods

    public static bool Run( string stage, IEnumerable<string> inputs, Func<IEnumerable<string>> outputs, Func<bool> generate, string context = "", bool force = false )
    {
        var fingerprint = FingerprintFiles( inputs, CompilerFingerprint + context );
        var cachePath = Path.Combine( Paths.GeneratedOutputPath, ".cache", stage + ".json" );
        var cache = Read<StageCache>( cachePath );

        if ( !force && cache is not null && cache.Fingerprint == fingerprint && OutputsMatch( cache.Outputs, outputs() ) )
        {
            Console.WriteLine( $"Blazorise.Docs.Compiler: {stage} is up to date." );
            return true;
        }

        if ( !generate() )
        {
            return false;
        }

        var outputHashes = outputs().ToDictionary( Path.GetFullPath, FileFingerprint, StringComparer.Ordinal );
        Write( cachePath, new StageCache( fingerprint, outputHashes ) );
        return true;
    }

    public static string FingerprintFiles( IEnumerable<string> files, string context = "" )
    {
        using ( var hash = IncrementalHash.CreateHash( HashAlgorithmName.SHA256 ) )
        {
            Append( hash, context );

            foreach ( var file in files.Select( Path.GetFullPath ).Distinct( StringComparer.Ordinal ).OrderBy( file => file, StringComparer.Ordinal ) )
            {
                Append( hash, file );
                Append( hash, FileFingerprint( file ) );
            }

            return Convert.ToHexString( hash.GetHashAndReset() );
        }
    }

    public static string ContentFingerprint( string source )
        => Convert.ToHexString( SHA256.HashData( Encoding.UTF8.GetBytes( CompilerFingerprint + source ) ) );

    public static string FileFingerprint( string path )
    {
        using ( var stream = File.OpenRead( path ) )
        {
            return Convert.ToHexString( SHA256.HashData( stream ) );
        }
    }

    public static T Read<T>( string path ) where T : class
    {
        if ( !File.Exists( path ) )
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>( File.ReadAllText( path ) );
        }
        catch ( JsonException )
        {
            return null;
        }
    }

    public static void Write<T>( string path, T value )
    {
        var json = JsonSerializer.Serialize( value );
        WriteTextIfChanged( path, json );
    }

    public static void WriteTextIfChanged( string path, string text )
    {
        if ( File.Exists( path ) && File.ReadAllText( path ) == text )
        {
            return;
        }

        Directory.CreateDirectory( Path.GetDirectoryName( path ) );
        File.WriteAllText( path, text );
    }

    private static bool OutputsMatch( Dictionary<string, string> expected, IEnumerable<string> outputs )
    {
        if ( expected is null || expected.Count == 0 )
        {
            return false;
        }

        var paths = outputs.Select( Path.GetFullPath ).ToHashSet( StringComparer.Ordinal );

        return paths.SetEquals( expected.Keys )
            && expected.All( output => File.Exists( output.Key ) && FileFingerprint( output.Key ) == output.Value );
    }

    private static void Append( IncrementalHash hash, string value )
    {
        hash.AppendData( Encoding.UTF8.GetBytes( value ) );
        hash.AppendData( new byte[] { 0 } );
    }

    #endregion

    #region Properties

    public static string CompilerFingerprint => compilerFingerprint.Value;

    #endregion

    public sealed record StageCache( string Fingerprint, Dictionary<string, string> Outputs );
}