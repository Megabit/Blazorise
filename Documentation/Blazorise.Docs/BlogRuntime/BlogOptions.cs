using System;

namespace Blazorise.Docs.BlogRuntime;

public sealed class BlogOptions
{
    public required string Owner { get; init; }           // eg. "my-org"
    public required string Repo { get; init; }            // eg. "company-blog"
    public string Branch { get; init; } = "main";         // default branch
    public string ContentRoot { get; init; } = "posts";   // folder with .md
    public TimeSpan ListCache { get; init; } = TimeSpan.FromMinutes( 10 );
    public TimeSpan PostCache { get; init; } = TimeSpan.FromMinutes( 30 );
    public string GitHubToken { get; init; }             // optional
}