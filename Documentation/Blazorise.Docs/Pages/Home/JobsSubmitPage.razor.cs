using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace Blazorise.Docs.Pages.Home;

public partial class JobsSubmitPage
{
    private static readonly IReadOnlyList<string> EmploymentTypes = new[]
    {
        "Full-time",
        "Part-time",
        "Contract",
        "Internship",
        "Temporary",
        "Other"
    };

    private static readonly IReadOnlyList<string> SeniorityOptions = new[]
    {
        "Intern",
        "Junior",
        "Mid",
        "Senior",
        "Lead",
        "Principal",
        "Other"
    };

    private Validations validationsRef;
    private JobSubmission submission = CreateSubmission();
    private JobSubmissionResult submissionResult;
    private string submissionError;
    private bool notARobot;
    private bool isSubmitting;

    [Inject] public IJobSubmissionService JobSubmissionService { get; set; }
    [Inject] public ILogger<JobsSubmitPage> Logger { get; set; }

    private async Task SubmitAsync()
    {
        if ( isSubmitting || !await validationsRef.ValidateAll() )
            return;

        isSubmitting = true;
        submissionError = null;

        try
        {
            submissionResult = await JobSubmissionService.SubmitAsync( submission );
        }
        catch ( Exception exc )
        {
            Logger.LogError( exc, "Error submitting a job to GitHub." );
            submissionError = "We could not submit this job right now. Please try again later.";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private void SubmitAnother()
    {
        submission = CreateSubmission();
        submissionResult = null;
        submissionError = null;
        notARobot = false;
    }

    private static JobSubmission CreateSubmission()
    {
        return new JobSubmission
        {
            ExpiryDate = DateTime.UtcNow.Date.AddDays( 30 )
        };
    }

    private static void ValidateApplyUrl( ValidatorEventArgs eventArgs )
    {
        string value = eventArgs.Value?.ToString();
        bool valid = Uri.TryCreate( value, UriKind.Absolute, out Uri uri )
                     && ( string.Equals( uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase )
                          || string.Equals( uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase ) );

        eventArgs.Status = valid ? ValidationStatus.Success : ValidationStatus.Error;
        eventArgs.ErrorText = valid ? null : "Enter a valid HTTP or HTTPS application URL.";
    }

    private static void ValidateOptionalEmail( ValidatorEventArgs eventArgs )
    {
        string value = eventArgs.Value?.ToString();
        bool valid = string.IsNullOrWhiteSpace( value ) || MailAddress.TryCreate( value, out _ );

        eventArgs.Status = valid ? ValidationStatus.Success : ValidationStatus.Error;
        eventArgs.ErrorText = valid ? null : "Enter a valid email address.";
    }

    private static void ValidateExpiryDate( ValidatorEventArgs eventArgs )
    {
        bool valid = eventArgs.Value is DateTime expiryDate && expiryDate.Date >= DateTime.UtcNow.Date;

        eventArgs.Status = valid ? ValidationStatus.Success : ValidationStatus.Error;
        eventArgs.ErrorText = valid ? null : "Choose an expiry date that is today or later.";
    }
}