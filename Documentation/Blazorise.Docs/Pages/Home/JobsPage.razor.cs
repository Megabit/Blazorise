using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Blazorise.Docs.Options;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Blazorise.Docs.Pages.Home;

public partial class JobsPage
{
    private const string AllFilterOption = "All";

    private IReadOnlyList<JobPost> jobs = new List<JobPost>();
    private IReadOnlyList<string> employmentTypeOptions = new List<string> { AllFilterOption };
    private IReadOnlyList<string> seniorityOptions = new List<string> { AllFilterOption };

    private bool isLoading = true;
    private string loadError;
    private string searchText = string.Empty;
    private bool remoteOnly;
    private string selectedEmploymentType = AllFilterOption;
    private string selectedSeniority = AllFilterOption;
    private JobsSortOption selectedSort = JobsSortOption.MostRecent;

    private JobsOptions jobsOptions = new JobsOptions();
    private JobPost selectedJob;
    private bool detailsVisible;

    [Inject] public IJobsService JobsService { get; set; }
    [Inject] public IOptions<JobsOptions> JobsOptions { get; set; }

    private IReadOnlyList<string> EmploymentTypeOptions => employmentTypeOptions;
    private IReadOnlyList<string> SeniorityOptions => seniorityOptions;
    private string SubmitJobUrl => jobsOptions.SubmitJobUrl;
    private string DetailsTitle => selectedJob?.Title ?? "Job details";

    protected override async Task OnInitializedAsync()
    {
        jobsOptions = JobsOptions?.Value ?? new JobsOptions();
        await LoadJobsAsync();
    }

    private async Task ReloadAsync()
    {
        await LoadJobsAsync();
    }

    private async Task LoadJobsAsync()
    {
        isLoading = true;
        loadError = null;

        try
        {
            IReadOnlyList<JobPost> result = await JobsService.GetJobsAsync();
            jobs = result ?? new List<JobPost>();
            employmentTypeOptions = BuildFilterOptions( jobs, job => job.EmploymentType );
            seniorityOptions = BuildFilterOptions( jobs, job => job.Seniority );

            if ( !OptionListContains( employmentTypeOptions, selectedEmploymentType ) )
                selectedEmploymentType = AllFilterOption;

            if ( !OptionListContains( seniorityOptions, selectedSeniority ) )
                selectedSeniority = AllFilterOption;
        }
        catch ( Exception )
        {
            loadError = "We could not load jobs right now. Please try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private void ShowDetails( JobPost job )
    {
        if ( job is null )
            return;

        selectedJob = job;
        detailsVisible = true;
    }

    private void HideDetails()
    {
        detailsVisible = false;
    }

    private IReadOnlyList<JobPost> GetFilteredJobs()
    {
        IEnumerable<JobPost> query = jobs;

        if ( remoteOnly )
            query = query.Where( job => job.Remote );

        if ( !IsAllFilter( selectedEmploymentType ) )
            query = query.Where( job => string.Equals( job.EmploymentType, selectedEmploymentType, StringComparison.OrdinalIgnoreCase ) );

        if ( !IsAllFilter( selectedSeniority ) )
            query = query.Where( job => string.Equals( job.Seniority, selectedSeniority, StringComparison.OrdinalIgnoreCase ) );

        if ( !string.IsNullOrWhiteSpace( searchText ) )
            query = query.Where( job => MatchesSearch( job, searchText ) );

        List<JobPost> filtered = query.ToList();
        return SortJobs( filtered );
    }

    private string BuildResultsSummary( IReadOnlyList<JobPost> filteredJobs )
    {
        if ( isLoading )
            return "Loading jobs...";

        if ( loadError is not null )
            return "Jobs feed unavailable.";

        int total = jobs.Count;
        int filtered = filteredJobs.Count;

        if ( total == 0 )
            return "No roles listed yet.";

        if ( filtered == total )
            return $"{total.ToString( CultureInfo.InvariantCulture )} roles available";

        return $"{filtered.ToString( CultureInfo.InvariantCulture )} of {total.ToString( CultureInfo.InvariantCulture )} roles";
    }

    private List<JobPost> SortJobs( List<JobPost> items )
    {
        IEnumerable<JobPost> sorted;

        if ( selectedSort == JobsSortOption.CompanyAscending )
        {
            sorted = items.OrderBy( job => job.Company ?? string.Empty, StringComparer.OrdinalIgnoreCase )
                .ThenBy( job => job.Title ?? string.Empty, StringComparer.OrdinalIgnoreCase );
        }
        else
        {
            sorted = items.OrderByDescending( GetSortDate )
                .ThenBy( job => job.Company ?? string.Empty, StringComparer.OrdinalIgnoreCase );
        }

        return sorted.ToList();
    }

    private static DateTimeOffset GetSortDate( JobPost job )
    {
        if ( job is null )
            return DateTimeOffset.MinValue;

        if ( job.UpdatedAt.HasValue )
            return job.UpdatedAt.Value;

        if ( job.CreatedAt.HasValue )
            return job.CreatedAt.Value;

        return DateTimeOffset.MinValue;
    }

    private static bool MatchesSearch( JobPost job, string term )
    {
        if ( job is null || string.IsNullOrWhiteSpace( term ) )
            return false;

        string trimmed = term.Trim();

        if ( ContainsValue( job.Title, trimmed ) )
            return true;

        if ( ContainsValue( job.Company, trimmed ) )
            return true;

        if ( ContainsValue( job.Location, trimmed ) )
            return true;

        if ( job.Tags is null )
            return false;

        foreach ( string tag in job.Tags )
        {
            if ( ContainsValue( tag, trimmed ) )
                return true;
        }

        return false;
    }

    private static bool ContainsValue( string value, string term )
    {
        return !string.IsNullOrWhiteSpace( value ) && value.Contains( term, StringComparison.OrdinalIgnoreCase );
    }

    private static string GetLocationText( JobPost job )
    {
        if ( job is null || string.IsNullOrWhiteSpace( job.Location ) )
            return "Location not specified";

        return job.Location;
    }

    private static string GetRemoteText( JobPost job )
    {
        if ( job is null )
            return "Not specified";

        return job.Remote ? "Yes" : "No";
    }

    private static string GetEmploymentText( JobPost job )
    {
        return GetOptionalText( job?.EmploymentType );
    }

    private static string GetSeniorityText( JobPost job )
    {
        return GetOptionalText( job?.Seniority );
    }

    private static string GetSalaryText( JobPost job )
    {
        return GetOptionalText( job?.SalaryRange );
    }

    private static string FormatUpdatedText( JobPost job )
    {
        if ( job is null )
            return string.Empty;

        if ( job.UpdatedAt.HasValue )
            return $"Updated {job.UpdatedAt.Value.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture )}";

        if ( job.CreatedAt.HasValue )
            return $"Posted {job.CreatedAt.Value.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture )}";

        return "Date not specified";
    }

    private static string FormatDate( DateTimeOffset? date )
    {
        if ( !date.HasValue )
            return "Not specified";

        return date.Value.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture );
    }

    private static string FormatDate( DateTime? date )
    {
        if ( !date.HasValue )
            return "Not specified";

        return date.Value.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture );
    }

    private static IReadOnlyList<string> GetDescriptionLines( JobPost job )
    {
        if ( job is null || string.IsNullOrWhiteSpace( job.Description ) )
            return new List<string> { "No description provided." };

        string normalized = job.Description.Replace( "\r\n", "\n" ).Replace( '\r', '\n' );
        string[] parts = normalized.Split( '\n', StringSplitOptions.RemoveEmptyEntries );
        List<string> lines = new List<string>();

        foreach ( string part in parts )
        {
            string trimmed = part.Trim();
            if ( trimmed.Length == 0 )
                continue;

            lines.Add( trimmed );
        }

        if ( lines.Count == 0 )
            lines.Add( job.Description.Trim() );

        return lines;
    }

    private static string GetOptionalText( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return "Not specified";

        return value;
    }

    private static bool IsAllFilter( string value )
    {
        return string.IsNullOrWhiteSpace( value )
               || string.Equals( value, AllFilterOption, StringComparison.OrdinalIgnoreCase );
    }

    private static IReadOnlyList<string> BuildFilterOptions( IReadOnlyList<JobPost> source, Func<JobPost, string> selector )
    {
        HashSet<string> values = new HashSet<string>( StringComparer.OrdinalIgnoreCase );

        foreach ( JobPost job in source )
        {
            string value = selector( job );
            if ( string.IsNullOrWhiteSpace( value ) )
                continue;

            values.Add( value.Trim() );
        }

        List<string> options = values
            .OrderBy( value => value, StringComparer.OrdinalIgnoreCase )
            .ToList();

        options.Insert( 0, AllFilterOption );

        return options;
    }

    private static bool OptionListContains( IReadOnlyList<string> options, string value )
    {
        foreach ( string option in options )
        {
            if ( string.Equals( option, value, StringComparison.OrdinalIgnoreCase ) )
                return true;
        }

        return false;
    }

    private enum JobsSortOption
    {
        MostRecent,
        CompanyAscending
    }
}