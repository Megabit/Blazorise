#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Core;
using Blazorise.Docs.Domain;
using Blazorise.Docs.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
#endregion

namespace Blazorise.Docs.Pages.Home;

public partial class ContactPage
{
    #region Members

    private Validations validationsRef;

    private Result result;

    private const string DefaultSubject = "Product questions";

    private string messageSubject = DefaultSubject;

    private string messageBody;

    private string messageBodyTemplate;

    #endregion

    #region Methods

    protected override void OnInitialized()
    {
        ApplySubjectTemplate();

        base.OnInitialized();
    }

    protected async Task SendEmail()
    {
        try
        {
            if ( await validationsRef.ValidateAll() )
            {
                var message = new StringBuilder();

                message.AppendLine( $"From: {User.FirstName} {User.LastName}" );
                message.AppendLine( $"Email: {User.Email}" );
                message.AppendLine();
                message.AppendLine( "Message: " + MessageBody );

                var emailMessage = new EmailMessage( EmailOptions.FromName, EmailOptions.FromAddress )
                {
                    Subject = MessageSubject,
                    Body = message.ToString(),
                    IsHtml = false,
                };

                //emailMessage.From.Add( new EmailAddress( $"{User.FirstName} {User.LastName}", User.Email ) );

                result = await EmailSender.Send( emailMessage, CancellationToken.None );

                if ( result.Succeeded )
                {
                    await MessageService.Success( "Your message is sent." );

                    User = new User();
                    MessageSubject = DefaultSubject;
                    NotARobot = false;

                    await validationsRef.ClearAll();
                }
                else
                    await MessageService.Error( "Failed to send the message." );
            }
        }
        catch ( Exception exc )
        {
            Logger.LogError( exc, "Error sending email." );
        }
    }

    #endregion

    #region Properties

    public List<string> Subjects { get; set; } = new List<string>
    {
        "Product questions",
        "Licensing/Pricing/Quotes",
        "Invoicing/Billing",
        "Renewal",
        "General Inquiries",
    };

    public User User { get; set; } = new User();

    public string MessageSubject
    {
        get => messageSubject;
        set
        {
            if ( messageSubject == value )
                return;

            messageSubject = value;

            ApplySubjectTemplate();
        }
    }

    public string MessageBody
    {
        get => messageBody;
        set => messageBody = value;
    }

    public bool NotARobot { get; set; }

    [Inject] public EmailSender EmailSender { get; set; }

    [Inject] public IEmailOptions EmailOptions { get; set; }

    [Inject] public ILogger<ContactPage> Logger { get; set; }

    [Inject] public IMessageService MessageService { get; set; }

    #endregion

    #region Helpers

    private void ApplySubjectTemplate()
    {
        messageBodyTemplate = GetTemplateForSubject( MessageSubject );
        MessageBody = messageBodyTemplate;
    }

    private static string GetTemplateForSubject( string subject )
        => subject switch
        {
            "Product questions" => """
                Please include:
                - Blazorise version:
                - .NET version:
                - CSS provider(s):
                - What are you trying to achieve?
                - What did you expect to happen?
                - What happened instead? (include error text/logs if relevant)
                - Minimal repro or code snippet (if possible):
                """,

            "Licensing/Pricing/Quotes" => """
                Please include:
                - Company name:
                - Interested in: Professional / Enterprise / Enterprise Plus
                - Number of developers:
                - Billing preference: annual / one-time
                - Purchase method: card / bank transfer / purchase order
                - VAT ID (EU, optional):
                - Target start date:

                If you're requesting paid delivery (Feature Sponsorship or Custom Development), please share your target timeline and a short scope/acceptance criteria.
                """,

            "Invoicing/Billing" => """
                Please include:
                - Company name:
                - Invoice number (if applicable):
                - What do you need us to change/confirm?
                - Purchase order reference (if applicable):
                - Any relevant context or attachments (links):
                """,

            "Renewal" => """
                Please include:
                - Company name:
                - Current plan and quantity:
                - Renewal date (if known):
                - Any changes requested (quantity, billing, PO requirements):
                """,

            _ => """
                Please include:
                - Topic:
                - Details:
                - Links/screenshots (if applicable):
                """,
        };

    #endregion
}