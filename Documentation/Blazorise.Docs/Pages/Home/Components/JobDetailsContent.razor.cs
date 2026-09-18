using System;
using System.Globalization;
using Blazorise.Docs.Models;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Pages.Home.Components;

public partial class JobDetailsContent
{
    [Parameter, EditorRequired] public JobPost Job { get; set; }
    [Parameter] public string LocationText { get; set; }
    [Parameter] public MarkupString DescriptionMarkup { get; set; }

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

    private static string GetOptionalText( string value )
    {
        if ( string.IsNullOrWhiteSpace( value ) )
            return "Not specified";

        return value;
    }
}