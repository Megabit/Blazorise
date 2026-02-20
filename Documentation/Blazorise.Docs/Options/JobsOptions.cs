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
    /// Gets or sets the URL for submitting a job post.
    /// </summary>
    public string SubmitJobUrl { get; set; } = "https://github.com/your-org/jobs/issues/new?template=job.yml";

    /// <summary>
    /// Gets or sets the GitHub personal access token used for authentication.
    /// </summary>
    public string GitHubToken { get; set; }

    /// <summary>
    /// Gets or sets the secret used to authenticate refresh requests.
    /// </summary>
    public string RefreshSecret { get; set; }
}