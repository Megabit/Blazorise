#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
#endregion

namespace Blazorise.Build
{
    /// <summary>
    /// Generates static assets with the release version without modifying their sources.
    /// </summary>
    public class GenerateVersionedAssets : Task
    {
        #region Members

        private const string VersionToken = "__BLAZORISE_VERSION__";

        #endregion

        #region Methods

        public override bool Execute()
        {
            var originalFiles = new List<ITaskItem>();
            var generatedFiles = new List<ITaskItem>();

            try
            {
                var sourceRoot = Path.GetFullPath( SourceRoot ).TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar ) + Path.DirectorySeparatorChar;
                var outputRoot = Path.GetFullPath( OutputRoot ).TrimEnd( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar ) + Path.DirectorySeparatorChar;
                var pathComparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                var version = Version.Parse( AssetVersion ).ToString( 4 );

                foreach ( var source in SourceFiles )
                {
                    var sourcePath = source.GetMetadata( "FullPath" );

                    if ( !sourcePath.StartsWith( sourceRoot, pathComparison ) || !IsTextAsset( sourcePath ) )
                    {
                        continue;
                    }

                    var text = File.ReadAllText( sourcePath );

                    if ( !text.Contains( VersionToken ) )
                    {
                        continue;
                    }

                    var relativePath = sourcePath.Substring( sourceRoot.Length );
                    var outputPath = Path.Combine( outputRoot, relativePath );
                    var generatedText = text.Replace( VersionToken, version );

                    if ( generatedText.Contains( VersionToken ) )
                    {
                        Log.LogError( "Unresolved Blazorise asset version in '{0}'.", sourcePath );
                        return false;
                    }

                    if ( !File.Exists( outputPath ) || File.ReadAllText( outputPath ) != generatedText )
                    {
                        Directory.CreateDirectory( Path.GetDirectoryName( outputPath ) );
                        File.WriteAllText( outputPath, generatedText, new UTF8Encoding( false ) );
                    }

                    var generatedFile = new TaskItem( source );
                    generatedFile.ItemSpec = outputPath;
                    generatedFile.SetMetadata( "Link", "wwwroot/" + relativePath.Replace( '\\', '/' ) );
                    generatedFile.SetMetadata( "TargetPath", "wwwroot/" + relativePath.Replace( '\\', '/' ) );
                    generatedFile.SetMetadata( "ContentRoot", outputRoot );
                    generatedFile.SetMetadata( "OriginalItemSpec", outputPath );

                    originalFiles.Add( source );
                    generatedFiles.Add( generatedFile );
                }
            }
            catch ( Exception exception )
            {
                Log.LogErrorFromException( exception, true );
                return false;
            }

            OriginalFiles = originalFiles.ToArray();
            GeneratedFiles = generatedFiles.ToArray();

            return !Log.HasLoggedErrors;
        }

        private static bool IsTextAsset( string path )
        {
            var extension = Path.GetExtension( path );

            return string.Equals( extension, ".js", StringComparison.OrdinalIgnoreCase )
                || string.Equals( extension, ".mjs", StringComparison.OrdinalIgnoreCase )
                || string.Equals( extension, ".css", StringComparison.OrdinalIgnoreCase )
                || string.Equals( extension, ".html", StringComparison.OrdinalIgnoreCase );
        }

        #endregion

        #region Properties

        [Required] public ITaskItem[] SourceFiles { get; set; }

        [Required] public string SourceRoot { get; set; }

        [Required] public string OutputRoot { get; set; }

        [Required] public string AssetVersion { get; set; }

        [Output] public ITaskItem[] OriginalFiles { get; set; }

        [Output] public ITaskItem[] GeneratedFiles { get; set; }

        #endregion
    }
}