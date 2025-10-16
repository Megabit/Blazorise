using System;

namespace Blazorise.Docs.BlogRuntime;

/// <summary>
/// Represents configuration options for a blog hosted in a GitHub repository.
/// </summary>
/// <remarks>This class provides settings for specifying the repository details, content location, caching
/// durations,  and authentication token required to access and manage blog posts stored in a GitHub
/// repository.</remarks>
public sealed class BlogOptions
{
    /// <summary>
    /// Defines the organization or user that owns the repository.
    /// </summary>
    public required string Owner { get; init; }

    /// <summary>
    /// Defines the name of the repository containing the blog posts.
    /// </summary>
    public required string Repo { get; init; }

    /// <summary>
    /// Defines the branch to use.
    /// </summary>
    public string Branch { get; init; } = "main";

    /// <summary>
    /// Defines the folder(s) within the repository where the blog posts are located.
    /// </summary>
    public string[] ContentRoot { get; init; } = { "blog" };

    /// <summary>
    /// Gets the duration for which the list cache remains valid.
    /// </summary>
    public TimeSpan ListCache { get; init; } = TimeSpan.FromMinutes( 10 );

    /// <summary>
    /// Gets the duration for which posts are cached.
    /// </summary>
    public TimeSpan PostCache { get; init; } = TimeSpan.FromMinutes( 30 );

    /// <summary>
    /// Gets the GitHub personal access token used for authentication.
    /// </summary>
    public string GitHubToken { get; init; }
}