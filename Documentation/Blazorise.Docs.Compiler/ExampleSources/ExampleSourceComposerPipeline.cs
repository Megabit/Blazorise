using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blazorise.Docs.Compiler.ExampleSources;

internal static class ExampleSourceComposerPipeline
{
    private static readonly IExampleSourceComposer[] Composers =
    [
        new DataGridExampleSourceComposer(),
        new CountryExampleSourceComposer(),
        new FluentValidationExampleSourceComposer(),
        new ProviderExampleSourceComposer()
    ];

    public static string PrepareForDisplay( string path, string source )
        => Prepare( path, source, ExampleSourceMode.Display );

    public static string PrepareForCopy( string path, string source )
        => Prepare( path, source, ExampleSourceMode.Copy );

    private static string Prepare( string path, string source, ExampleSourceMode mode )
    {
        string normalizedPath = path.Replace( '\\', '/' );
        IExampleSourceComposer composer = Composers.FirstOrDefault( candidate => candidate.CanHandle( normalizedPath ) );

        if ( composer is not null )
        {
            source = composer.Prepare( path, source, mode );
        }

        if ( source.Contains( ExampleSourceComposerHelpers.CopyPasteReadyMarker, StringComparison.Ordinal ) )
        {
            ValidateCopyPasteReadySource( path, source );
            source = Regex.Replace(
                source,
                $"{Regex.Escape( ExampleSourceComposerHelpers.CopyPasteReadyMarker )}\\r?\\n",
                string.Empty );
        }

        return source;
    }

    private static void ValidateCopyPasteReadySource( string path, string source )
    {
        string[] docsOnlyDependencies =
        [
            "@inject ",
            "[Inject]",
            "Blazorise.Shared",
            "EmployeeData"
        ];

        string docsOnlyDependency = docsOnlyDependencies.FirstOrDefault( dependency => source.Contains( dependency, StringComparison.Ordinal ) );

        if ( docsOnlyDependency is not null )
            throw new InvalidOperationException( $"Copy-paste-ready example '{path}' references docs-only dependency '{docsOnlyDependency}'." );

        if ( !source.Contains( "@code", StringComparison.Ordinal ) )
            throw new InvalidOperationException( $"Copy-paste-ready example '{path}' must include its component state in an @code block." );
    }
}