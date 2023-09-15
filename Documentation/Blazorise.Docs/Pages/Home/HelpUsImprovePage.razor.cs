#region Using directives
using System;
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

public partial class HelpUsImprovePage
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
            if (await validationsRef.ValidateAll())
            {
                var message = new StringBuilder();

                message.AppendLine( $"From: {User.FirstName} {User.LastName}" );
                message.AppendLine( $"Email: {User.Email}" );
                message.AppendLine();
                message.AppendLine( "Message: " + MessageBody );

                var emailMessage = new EmailMessage( EmailOptions.FromName, EmailOptions.FromAddress )
                {
                    Subject = "Help Us Improve!",
                    Body = message.ToString(),
                    IsHtml = false,
                };

                result = await EmailSender.Send( emailMessage, CancellationToken.None );

                if (result.Succeeded)
                {
                    await MessageService.Success( "Your message was sent successfully." );

                    User = new User();
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

    private void ValidateRobot( ValidatorEventArgs eventArgs )
    {
        eventArgs.Status = NotARobot ? ValidationStatus.Success : ValidationStatus.Error;

        if (eventArgs.Status == ValidationStatus.Error)
            eventArgs.ErrorText = "Please check to confirm you're a real human!";
        else
            eventArgs.ErrorText = null;
    }

    #endregion

    #region Properties



    public User User { get; set; } = new User();

    public string MessageBody { get; set; }

    public bool NotARobot { get; set; }

    [Inject] public EmailSender EmailSender { get; set; }

    [Inject] public IEmailOptions EmailOptions { get; set; }

    [Inject] public ILogger<HelpUsImprovePage> Logger { get; set; }

    [Inject] public IMessageService MessageService { get; set; }

    #endregion
}
