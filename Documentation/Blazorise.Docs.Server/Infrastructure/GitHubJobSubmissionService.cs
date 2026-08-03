#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Blazorise.Docs.Options;
using Blazorise.Docs.Services;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.Server.Infrastructure;

internal sealed class GitHubJobSubmissionService : IJobSubmissionService
{
    #region Members

    private const string GitHubApiVersion = "2026-03-10";

    private static readonly HashSet<string> EmploymentTypes = new( StringComparer.Ordinal )
    {
        "Full-time",
        "Part-time",
        "Contract",
        "Internship",
        "Temporary",
        "Other"
    };

    private static readonly HashSet<string> SeniorityOptions = new( StringComparer.Ordinal )
    {
        "Intern",
        "Junior",
        "Mid",
        "Senior",
        "Lead",
        "Principal",
        "Other"
    };

    private readonly HttpClient httpClient;
    private readonly JobsOptions options;

    #endregion

    #region Constructors

    public GitHubJobSubmissionService( HttpClient httpClient, IOptions<JobsOptions> options )
    {
        this.httpClient = httpClient;
        this.options = options.Value ?? new JobsOptions();

        this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd( "BlazoriseDocsJobs/1.0" );
    }

    #endregion

    #region Methods

    public async Task<JobSubmissionResult> SubmitAsync( JobSubmission submission, CancellationToken cancellationToken = default )
    {
        ValidateConfiguration();
        ValidateSubmission( submission );

        string requestUrl = $"https://api.github.com/repos/{Uri.EscapeDataString( options.GitHubOwner )}/{Uri.EscapeDataString( options.GitHubRepository )}/issues";
        GitHubIssueRequest payload = new GitHubIssueRequest
        {
            Title = $"[JOB] {submission.RoleTitle.Trim()}",
            Body = BuildIssueBody( submission ),
            Labels = new[] { "type:job", "status:pending" }
        };

        using HttpRequestMessage request = new HttpRequestMessage( HttpMethod.Post, requestUrl )
        {
            Content = JsonContent.Create( payload )
        };
        request.Headers.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/vnd.github+json" ) );
        request.Headers.Authorization = new AuthenticationHeaderValue( "Bearer", options.GitHubToken );
        request.Headers.Add( "X-GitHub-Api-Version", GitHubApiVersion );

        using HttpResponseMessage response = await httpClient.SendAsync( request, cancellationToken );
        if ( !response.IsSuccessStatusCode )
        {
            string responseContent = await response.Content.ReadAsStringAsync( cancellationToken );
            throw new InvalidOperationException( $"GitHub rejected the job submission with status {(int)response.StatusCode}: {responseContent}" );
        }

        GitHubIssueResponse result = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>( cancellationToken: cancellationToken );
        if ( result is null || result.Number <= 0 || string.IsNullOrWhiteSpace( result.HtmlUrl ) )
            throw new InvalidOperationException( "GitHub returned an invalid response for the job submission." );

        return new JobSubmissionResult
        {
            IssueNumber = result.Number,
            IssueUrl = result.HtmlUrl
        };
    }

    private void ValidateConfiguration()
    {
        if ( string.IsNullOrWhiteSpace( options.GitHubOwner ) )
            throw new InvalidOperationException( "The GitHub jobs repository owner is not configured." );

        if ( string.IsNullOrWhiteSpace( options.GitHubRepository ) )
            throw new InvalidOperationException( "The GitHub jobs repository name is not configured." );

        if ( string.IsNullOrWhiteSpace( options.GitHubToken ) )
            throw new InvalidOperationException( "The GitHub token for job submissions is not configured." );
    }

    private static void ValidateSubmission( JobSubmission submission )
    {
        ArgumentNullException.ThrowIfNull( submission );

        RequireText( submission.CompanyName, "Company name", 200 );
        RequireText( submission.RoleTitle, "Role title", 200 );
        RequireText( submission.Location, "Location", 200 );
        RequireText( submission.Tags, "Tags/keywords", 500 );
        RequireText( submission.Description, "Description", 30000 );
        RequireOptionalText( submission.SalaryRange, "Salary range", 200 );
        RequireOptionalText( submission.ContactEmail, "Contact email", 320 );

        string[] tags = submission.Tags.Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );
        if ( tags.Length == 0 )
            throw new ArgumentException( "Tags/keywords must include at least one value.", nameof( submission ) );

        if ( submission.Remote is not ( "Yes" or "No" ) )
            throw new ArgumentException( "Remote must be Yes or No.", nameof( submission ) );

        if ( !EmploymentTypes.Contains( submission.EmploymentType ) )
            throw new ArgumentException( "Employment type is invalid.", nameof( submission ) );

        if ( !SeniorityOptions.Contains( submission.Seniority ) )
            throw new ArgumentException( "Seniority is invalid.", nameof( submission ) );

        RequireText( submission.ApplyUrl, "Apply URL", 2000 );
        if ( !Uri.TryCreate( submission.ApplyUrl.Trim(), UriKind.Absolute, out Uri applyUri )
             || ( !string.Equals( applyUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase )
                  && !string.Equals( applyUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase ) ) )
        {
            throw new ArgumentException( "Apply URL must be a valid HTTP or HTTPS URL.", nameof( submission ) );
        }

        if ( !string.IsNullOrWhiteSpace( submission.ContactEmail ) && !System.Net.Mail.MailAddress.TryCreate( submission.ContactEmail.Trim(), out _ ) )
            throw new ArgumentException( "Contact email is invalid.", nameof( submission ) );

        if ( !submission.ExpiryDate.HasValue || submission.ExpiryDate.Value.Date < DateTime.UtcNow.Date )
            throw new ArgumentException( "Expiry date must be today or later.", nameof( submission ) );

        if ( !submission.Confirmation )
            throw new ArgumentException( "Submission confirmation is required.", nameof( submission ) );
    }

    private static void RequireText( string value, string fieldName, int maximumLength )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            throw new ArgumentException( $"{fieldName} is required." );

        if ( value.Length > maximumLength )
            throw new ArgumentException( $"{fieldName} cannot exceed {maximumLength.ToString( CultureInfo.InvariantCulture )} characters." );
    }

    private static void RequireOptionalText( string value, string fieldName, int maximumLength )
    {
        if ( value is not null && value.Length > maximumLength )
            throw new ArgumentException( $"{fieldName} cannot exceed {maximumLength.ToString( CultureInfo.InvariantCulture )} characters." );
    }

    private static string BuildIssueBody( JobSubmission submission )
    {
        StringBuilder body = new StringBuilder();

        AppendField( body, "Company name", submission.CompanyName );
        AppendField( body, "Role title", submission.RoleTitle );
        AppendField( body, "Location", submission.Location );
        AppendField( body, "Remote", submission.Remote );
        AppendField( body, "Employment type", submission.EmploymentType );
        AppendField( body, "Seniority", submission.Seniority );
        AppendField( body, "Tags/keywords", submission.Tags );
        AppendField( body, "Apply URL", submission.ApplyUrl );
        AppendField( body, "Description", submission.Description );
        AppendField( body, "Salary range", submission.SalaryRange );
        AppendField( body, "Contact email", submission.ContactEmail );
        AppendField( body, "Expiry date", submission.ExpiryDate.Value.ToString( "yyyy-MM-dd", CultureInfo.InvariantCulture ) );
        body.AppendLine( "## Confirmation" );
        body.AppendLine();
        body.Append( "- [x] I confirm I have rights to post this job and it's not spam." );

        return body.ToString();
    }

    private static void AppendField( StringBuilder body, string heading, string value )
    {
        body.Append( "## " );
        body.AppendLine( heading );
        body.AppendLine();
        body.AppendLine( string.IsNullOrWhiteSpace( value ) ? "_No response_" : value.Trim() );
        body.AppendLine();
    }

    #endregion

    #region Nested types

    private sealed class GitHubIssueRequest
    {
        [JsonPropertyName( "title" )]
        public string Title { get; set; }

        [JsonPropertyName( "body" )]
        public string Body { get; set; }

        [JsonPropertyName( "labels" )]
        public IReadOnlyList<string> Labels { get; set; }
    }

    private sealed class GitHubIssueResponse
    {
        [JsonPropertyName( "number" )]
        public long Number { get; set; }

        [JsonPropertyName( "html_url" )]
        public string HtmlUrl { get; set; }
    }

    #endregion
}