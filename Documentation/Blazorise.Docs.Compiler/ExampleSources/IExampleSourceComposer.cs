namespace Blazorise.Docs.Compiler.ExampleSources;

internal enum ExampleSourceMode
{
    Display,
    Copy
}

internal interface IExampleSourceComposer
{
    bool CanHandle( string normalizedPath );

    string Prepare( string path, string source, ExampleSourceMode mode );
}