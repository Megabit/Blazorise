#region Using directives
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Core;
using Blazorise.Docs.Domain;
using Blazorise.Docs.Models;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
#endregion

namespace Blazorise.Docs.Pages.Commercial;

public partial class CustomWorkQuotePage
{
    #region Members

    Validations validationsRef;

    private static readonly string[] EngagementOptions =
    {
        "Feature Sponsorship (fixed price)",
        "Custom Development (Time & Materials)",
        "Not sure",
    };

    private static readonly string[] BudgetOptions =
    {
        "Not sure",
        "Small (3.500–7.500 EUR)",
        "Medium (8.000–18.000 EUR)",
        "Large (18.000–45.000+ EUR)",
    };

    private static readonly string[] UpstreamOptions =
    {
        "Upstream (default)",
        "Private delivery",
        "Temporary exclusivity",
    };

    private const string ExclusivityOption = "Temporary exclusivity";

    private const string DetailsTemplate = """
        Please describe:
        - Goal and use case:
        - Expected behavior / acceptance criteria:
        - Edge cases and constraints (a11y, performance, security):
        - Dependencies (APIs, libraries, data/contracts):
        - Upstream/privacy preference: upstream by default / private / exclusivity (duration)
        """;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        InputModel.Details = DetailsTemplate;

        base.OnInitialized();
    }

    async Task OnSubmit()
    {
        if ( await validationsRef.ValidateAll() )
        {
            await SendEmail();
        }
    }

    protected async Task SendEmail()
    {
        try
        {
            var message = new StringBuilder();

            message.AppendLine( "Custom work quote request" );
            message.AppendLine();

            message.AppendLine( $"Company: {InputModel.Company}" );
            message.AppendLine( $"From: {InputModel.FullName}" );
            message.AppendLine( $"Email: {InputModel.Email}" );
            message.AppendLine();

            message.AppendLine( $"Engagement: {InputModel.Engagement}" );
            message.AppendLine( $"Budget Range: {InputModel.BudgetRange}" );
            message.AppendLine( $"Upstream Preference: {InputModel.UpstreamPreference}" );

            if ( InputModel.UpstreamPreference == ExclusivityOption )
                message.AppendLine( $"Exclusivity Duration (months): {InputModel.ExclusivityMonths}" );

            message.AppendLine();

            message.AppendLine( $"Request Title: {InputModel.Title}" );
            message.AppendLine( $"Target Timeline: {InputModel.Timeline}" );
            message.AppendLine();

            if ( !string.IsNullOrWhiteSpace( InputModel.BlazoriseVersion )
                 || !string.IsNullOrWhiteSpace( InputModel.DotNetVersion )
                 || !string.IsNullOrWhiteSpace( InputModel.CssProviders ) )
            {
                message.AppendLine( "Target setup:" );

                if ( !string.IsNullOrWhiteSpace( InputModel.BlazoriseVersion ) )
                    message.AppendLine( $"- Blazorise: {InputModel.BlazoriseVersion}" );
                if ( !string.IsNullOrWhiteSpace( InputModel.DotNetVersion ) )
                    message.AppendLine( $"- .NET: {InputModel.DotNetVersion}" );
                if ( !string.IsNullOrWhiteSpace( InputModel.CssProviders ) )
                    message.AppendLine( $"- CSS providers: {InputModel.CssProviders}" );

                message.AppendLine();
            }

            if ( !string.IsNullOrWhiteSpace( InputModel.Dependencies ) )
            {
                message.AppendLine( "Dependencies:" );
                message.AppendLine( InputModel.Dependencies );
                message.AppendLine();
            }

            message.AppendLine( "Details:" );
            message.AppendLine( InputModel.Details );

            var emailMessage = new EmailMessage( EmailOptions.FromName, EmailOptions.FromAddress )
            {
                Subject = $"Custom work quote request: {InputModel.Company} - {InputModel.Title}",
                Body = message.ToString(),
                IsHtml = false,
            };

            var result = await EmailSender.Send( emailMessage, CancellationToken.None );

            if ( result.Succeeded )
            {
                await MessageService.Success( (MarkupString)"Request is sent.<br>We will contact you shortly." );

                InputModel = new CustomWorkQuote();
                InputModel.Details = DetailsTemplate;

                await validationsRef.ClearAll();
            }
            else
                await MessageService.Error( "Failed to send the request." );
        }
        catch ( Exception exc )
        {
            Logger.LogError( exc, "Error sending custom work quote request." );
        }
    }

    private void ValidateExclusivityMonths( ValidatorEventArgs eventArgs )
    {
        if ( InputModel.UpstreamPreference != ExclusivityOption )
        {
            eventArgs.Status = ValidationStatus.Success;
            eventArgs.ErrorText = null;
            return;
        }

        if ( eventArgs.Value is int value && value >= 1 )
        {
            eventArgs.Status = ValidationStatus.Success;
            eventArgs.ErrorText = null;
        }
        else
        {
            eventArgs.Status = ValidationStatus.Error;
            eventArgs.ErrorText = "Please provide an exclusivity duration in months.";
        }
    }

    private void ValidateDetails( ValidatorEventArgs eventArgs )
    {
        var value = eventArgs.Value?.ToString() ?? string.Empty;

        if ( string.IsNullOrWhiteSpace( value ) )
        {
            eventArgs.Status = ValidationStatus.Error;
            eventArgs.ErrorText = "Please provide request details.";
            return;
        }

        if ( Normalize( value ) == Normalize( DetailsTemplate ) )
        {
            eventArgs.Status = ValidationStatus.Error;
            eventArgs.ErrorText = "Please replace the template with your specific requirements.";
            return;
        }

        eventArgs.Status = ValidationStatus.Success;
        eventArgs.ErrorText = null;
    }

    private static string Normalize( string value )
        => value.Replace( "\r\n", "\n", StringComparison.Ordinal ).Trim();

    #endregion

    #region Properties

    private CustomWorkQuote InputModel { get; set; } = new();

    [Inject] public EmailSender EmailSender { get; set; }

    [Inject] public IEmailOptions EmailOptions { get; set; }

    [Inject] public ILogger<CustomWorkQuotePage> Logger { get; set; }

    [Inject] public IMessageService MessageService { get; set; }

    #endregion
}