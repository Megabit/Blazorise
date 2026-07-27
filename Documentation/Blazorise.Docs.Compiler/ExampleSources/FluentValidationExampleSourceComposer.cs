using System;
using System.IO;
using System.Text.RegularExpressions;
using static Blazorise.Docs.Compiler.ExampleSources.ExampleSourceComposerHelpers;

namespace Blazorise.Docs.Compiler.ExampleSources;

internal sealed class FluentValidationExampleSourceComposer : IExampleSourceComposer
{
    public bool CanHandle( string normalizedPath )
        => normalizedPath.Contains( "/Extensions/FluentValidation/Examples/", StringComparison.Ordinal );

    public string Prepare( string path, string source, ExampleSourceMode mode )
    {
        string filename = Path.GetFileName( path );

        if ( filename.Equals( "PersonValidator.csharp", StringComparison.OrdinalIgnoreCase )
            || filename.Equals( "Person.csharp", StringComparison.OrdinalIgnoreCase ) )
        {
            source = Regex.Replace( source, "(?m)^using Blazorise[.]Shared[.]Models;\\r?\\n", string.Empty );
            source = Regex.Replace( source, "(?m)^namespace .+;\\r?\\n", string.Empty );
            return source;
        }

        if ( mode != ExampleSourceMode.Copy || !filename.Equals( "BasicFluentValidationExample.razor", StringComparison.OrdinalIgnoreCase ) )
            return source;

        source = AddRequiredUsings( path, source, ["@using FluentValidation"] );

        string examplesDirectory = Path.GetDirectoryName( path );
        string supportTypes = string.Join(
            Environment.NewLine + Environment.NewLine,
            ExtractSupportTypes( Path.Combine( examplesDirectory, "Person.csharp" ) ),
            ExtractSupportTypes( Path.Combine( examplesDirectory, "PersonValidator.csharp" ) ) );

        source = AppendToCodeBlock( path, source, supportTypes );
        ValidateComposedSource( path, source, ["Blazorise.Shared"] );

        return source;
    }
}