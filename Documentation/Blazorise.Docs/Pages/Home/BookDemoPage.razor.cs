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

public partial class BookDemoPage
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
                message.AppendLine( $"Company: {User.Company}" );
                message.AppendLine();
                message.AppendLine( "Job Role: " + JobRole );
                message.AppendLine( "Product: " + Product );
                message.AppendLine();
                message.AppendLine( "Message: " + MessageBody );

                var emailMessage = new EmailMessage( EmailOptions.FromName, EmailOptions.FromAddress )
                {
                    Subject = "Book a Demo",
                    Body = message.ToString(),
                    IsHtml = false,
                };

                result = await EmailSender.Send( emailMessage, CancellationToken.None );

                if ( result.Succeeded )
                {
                    await MessageService.Success( "Your message is sent." );

                    User = new User();
                    JobRole = null;
                    Product = null;
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

    public List<string> JobRoles { get; set; } = new List<string>
    {
        "Developer",
        "Senior/Lead Developer",
        "Manager/Director Development",
        "VP/SVP/Head of Development",
        "CTO/CIO/CEO",
        "Architect",
        "DevOps/Developer Experience",
        "Project Management/Program Management",
        "IT Developer",
        "IT Manager",
        "Consultant",
        "Sales/Marketing/Business Development",
        "Student/Researcher",
        "Other",
    };

    public List<string> Products { get; set; } = new List<string>
    {
        "Blazorise Professional",
        "Blazorise Enterprise",
        "Blazorise Enterprise Plus",
        "Mentoring",
        "Other",
    };

    public User User { get; set; } = new User();

    public string JobRole { get; set; } = "Developer";

    public string Product { get; set; } = "Blazorise Professional";

    public string MessageBody { get; set; }

    public bool NotARobot { get; set; }

    [Inject] public EmailSender EmailSender { get; set; }

    [Inject] public IEmailOptions EmailOptions { get; set; }

    [Inject] public ILogger<ContactPage> Logger { get; set; }

    [Inject] public IMessageService MessageService { get; set; }

    #endregion
}
