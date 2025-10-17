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
    /// Gets the GitHub personal access token used for authentication.
    /// </summary>
    public string GitHubToken { get; init; }

    /// <summary>
    /// Gets the folder path where runtime files are stored.
    /// </summary>
    public string RuntimeFolder { get; init; } = "runtime";

    /// <summary>
    /// Gets or initializes an optional absolute base URL for the runtime folder.
    /// </summary>
    public string RuntimeBaseUrlOverride { get; init; } // optional absolute base for the runtime folder

    /// <summary>
    /// Gets the interval at which the runtime version is polled for updates.
    /// </summary>
    public TimeSpan? RuntimeVersionPoll { get; init; } = TimeSpan.FromMinutes( 5 );

    /// <summary>
    /// Gets the secret used to authenticate runtime refresh requests.
    /// </summary>
    public string RuntimeRefreshSecret { get; init; }
}