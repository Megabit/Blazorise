using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Models;

namespace Blazorise.Docs.Services;

/// <summary>
/// Writes job submissions to the moderation queue.
/// </summary>
public interface IJobSubmissionService
{
    /// <summary>
    /// Submits a job for moderation.
    /// </summary>
    /// <param name="submission">The job to submit.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Information about the created moderation item.</returns>
    Task<JobSubmissionResult> SubmitAsync( JobSubmission submission, CancellationToken cancellationToken = default );
}

/// <summary>
/// Identifies a job that was submitted for moderation.
/// </summary>
public sealed class JobSubmissionResult
{
    public long IssueNumber { get; set; }
    public string IssueUrl { get; set; } = string.Empty;
}