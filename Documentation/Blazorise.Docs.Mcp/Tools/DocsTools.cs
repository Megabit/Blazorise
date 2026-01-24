using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;

namespace Blazorise.Docs.Mcp;

internal sealed class DocsTools
{
    [McpServerTool]
    [Description( "List Blazorise docs pages. Optional prefix defaults to /docs." )]
    public List<DocsPageSummary> ListDocsPages(
        [Description( "URL prefix to filter by." )] string startsWith = "/docs" )
    {
        DocsIndex index = DocsIndexProvider.GetIndex();
        string prefix = NormalizeRoutePrefix( startsWith );

        List<DocsPageSummary> results = index.Pages
            .Where( page => page.Route.StartsWith( prefix, StringComparison.OrdinalIgnoreCase ) )
            .OrderBy( page => page.Route, StringComparer.OrdinalIgnoreCase )
            .Select( page => new DocsPageSummary( page.Route, page.Title, page.Description ) )
            .ToList();

        return results;
    }

    [McpServerTool]
    [Description( "Get docs page details and example names by route." )]
    public DocsPageDetails GetDocsPage(
        [Description( "Docs route, for example /docs/components/button." )] string route )
    {
        if ( string.IsNullOrWhiteSpace( route ) )
            throw new ArgumentException( "Route is required.", nameof( route ) );

        DocsIndex index = DocsIndexProvider.GetIndex();
        string normalized = NormalizeRoute( route );

        DocsPage page = index.Pages.FirstOrDefault( p => string.Equals( p.Route, normalized, StringComparison.OrdinalIgnoreCase ) );

        if ( page is null )
            throw new InvalidOperationException( $"No docs page found for route '{normalized}'." );

        List<string> examples = page.Examples is not null
            ? page.Examples.Select( example => example.Code ).ToList()
            : new List<string>();

        List<DocsExampleSummary> exampleDetails = page.Examples is not null
            ? page.Examples
                .Select( example => new DocsExampleSummary( example.Code, example.Title, example.Description, example.Kind, example.SourcePath ) )
                .ToList()
            : new List<DocsExampleSummary>();

        return new DocsPageDetails( page.Route, page.Title, page.Description, page.PagePath, examples, exampleDetails );
    }

    [McpServerTool]
    [Description( "Get example code by example name." )]
    public DocsExampleDetails GetExampleCode(
        [Description( "Example code name, for example ButtonExample." )] string code )
    {
        if ( string.IsNullOrWhiteSpace( code ) )
            throw new ArgumentException( "Example code name is required.", nameof( code ) );

        DocsIndex index = DocsIndexProvider.GetIndex();

        DocsExample example = index.Pages
            .SelectMany( page => page.Examples ?? new List<DocsExample>() )
            .FirstOrDefault( item => string.Equals( item.Code, code, StringComparison.OrdinalIgnoreCase ) );

        if ( example is null )
            throw new InvalidOperationException( $"No example found for code '{code}'." );

        return new DocsExampleDetails( example.Code, example.Kind, example.SourcePath, example.Content, example.Title, example.Description );
    }

    [McpServerTool]
    [Description( "Search docs pages by route, title, or description." )]
    public List<DocsPageSummary> SearchDocs(
        [Description( "Search query." )] string query,
        [Description( "Max number of results to return." )] int maxResults = 20 )
    {
        if ( string.IsNullOrWhiteSpace( query ) )
            return new List<DocsPageSummary>();

        if ( maxResults <= 0 )
            return new List<DocsPageSummary>();

        DocsIndex index = DocsIndexProvider.GetIndex();
        string trimmed = query.Trim();

        List<DocsPageSummary> results = index.Pages
            .Where( page => ContainsIgnoreCase( page.Route, trimmed )
                            || ContainsIgnoreCase( page.Title, trimmed )
                            || ContainsIgnoreCase( page.Description, trimmed ) )
            .OrderBy( page => page.Route, StringComparer.OrdinalIgnoreCase )
            .Take( maxResults )
            .Select( page => new DocsPageSummary( page.Route, page.Title, page.Description ) )
            .ToList();

        return results;
    }

    private static string NormalizeRoutePrefix( string routePrefix )
    {
        if ( string.IsNullOrWhiteSpace( routePrefix ) )
            return "/docs";

        string trimmed = routePrefix.Trim();
        return trimmed.StartsWith( "/", StringComparison.Ordinal ) ? trimmed : "/" + trimmed;
    }

    private static string NormalizeRoute( string route )
    {
        string trimmed = route.Trim();
        return trimmed.StartsWith( "/", StringComparison.Ordinal ) ? trimmed : "/" + trimmed;
    }

    private static bool ContainsIgnoreCase( string value, string query )
    {
        if ( string.IsNullOrEmpty( value ) )
            return false;

        return value.IndexOf( query, StringComparison.OrdinalIgnoreCase ) >= 0;
    }
}

internal sealed class DocsPageSummary
{
    public DocsPageSummary( string route, string title, string description )
    {
        Route = route;
        Title = title;
        Description = description;
    }

    public string Route { get; }
    public string Title { get; }
    public string Description { get; }
}

internal sealed class DocsPageDetails
{
    public DocsPageDetails(
        string route,
        string title,
        string description,
        string pagePath,
        List<string> examples,
        List<DocsExampleSummary> exampleDetails )
    {
        Route = route;
        Title = title;
        Description = description;
        PagePath = pagePath;
        Examples = examples;
        ExampleDetails = exampleDetails;
    }

    public string Route { get; }
    public string Title { get; }
    public string Description { get; }
    public string PagePath { get; }
    public List<string> Examples { get; }
    public List<DocsExampleSummary> ExampleDetails { get; }
}

internal sealed class DocsExampleSummary
{
    public DocsExampleSummary( string code, string title, string description, string kind, string sourcePath )
    {
        Code = code;
        Title = title;
        Description = description;
        Kind = kind;
        SourcePath = sourcePath;
    }

    public string Code { get; }
    public string Title { get; }
    public string Description { get; }
    public string Kind { get; }
    public string SourcePath { get; }
}

internal sealed class DocsExampleDetails
{
    public DocsExampleDetails( string code, string kind, string sourcePath, string content, string title, string description )
    {
        Code = code;
        Kind = kind;
        SourcePath = sourcePath;
        Content = content;
        Title = title;
        Description = description;
    }

    public string Code { get; }
    public string Kind { get; }
    public string SourcePath { get; }
    public string Content { get; }
    public string Title { get; }
    public string Description { get; }
}