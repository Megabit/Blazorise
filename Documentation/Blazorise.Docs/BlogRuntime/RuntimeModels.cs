using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.BlogRuntime;

public sealed class BlogIndexItem
{
    public required string Permalink { get; init; }
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string Summary { get; init; }
    public string PostedOn { get; init; }
    public string Category { get; init; }
    public string ImageUrl { get; init; }
    public string AuthorName { get; init; }
    public string AuthorImage { get; init; }
    public string ReadTime { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>(); // keep if you later add tags
    public string Root { get; init; }
}

public sealed class BlogPageModel
{
    public required string Permalink { get; init; }
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string Summary { get; init; }
    public string PostedOn { get; init; }
    public string ReadTime { get; init; }
    public string ImageUrl { get; init; }
    public string Category { get; init; }
    public string AuthorName { get; init; }
    public string AuthorImage { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public required RenderFragment Content { get; init; }
    public string Root { get; init; }
}
