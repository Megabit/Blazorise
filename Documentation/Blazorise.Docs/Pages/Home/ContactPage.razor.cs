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

    #endregion

    #region Methods

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
                    MessageSubject = "Product questions";
                    MessageBody = null;
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

    public string MessageSubject { get; set; } = "Product questions";

    public string MessageBody { get; set; }

    public bool NotARobot { get; set; }

    [Inject] public EmailSender EmailSender { get; set; }

    [Inject] public IEmailOptions EmailOptions { get; set; }

    [Inject] public ILogger<ContactPage> Logger { get; set; }

    [Inject] public IMessageService MessageService { get; set; }

    #endregion
}
