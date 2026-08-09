namespace Blazorise.Docs.Options;

/// <summary>
/// Configuration settings for the jobs feed integration.
/// </summary>
public class JobsOptions
{
    /// <summary>
    /// The configuration section name for jobs settings.
    /// </summary>
    public const string SectionName = "Jobs";

    /// <summary>
    /// Gets or sets the jobs feed URL.
    /// </summary>
    public string FeedUrl { get; set; } = "https://example.com/jobs/jobs.json";

    /// <summary>
    /// Gets or sets the GitHub organization or user that owns the jobs repository.
    /// </summary>
    public string GitHubOwner { get; set; } = "Blazorise";

    /// <summary>
    /// Gets or sets the GitHub jobs repository name.
    /// </summary>
    public string GitHubRepository { get; set; } = "Blazorise.Jobs";

    /// <summary>
    /// Gets or sets the GitHub token used to create pending job issues.
    /// </summary>
    public string GitHubToken { get; set; }

    /// <summary>
    /// Gets or sets the secret used to authenticate refresh requests.
    /// </summary>
    public string RefreshSecret { get; set; }
}