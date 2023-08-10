#region Using directives
using System;
using System.Linq;
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

public partial class PurchaseOrderPage
{
    #region Members

    Validations validationsRef;

    private string[] SupportedProductNames = new string[]
    {
        "Blazorise Professional",
        "Blazorise Enterprise",
    };

    private string[] SupportedPaymentTypes = new string[]
    {
        "Bank Transfer",
        "Stripe Payment Link",
    };

    #endregion

    #region Methods

    protected override Task OnInitializedAsync()
    {
        if ( SupportedProductNames.Any( x => x == ProductName ) )
        {
            InputModel.Product = ProductName;
        }

        return base.OnInitializedAsync();
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

            message.AppendLine( $"Product: {InputModel.Product}" );
            message.AppendLine( $"Quantity: {InputModel.Quantity}" );
            message.AppendLine();

            message.AppendLine( $"From: {InputModel.FullName}" );
            message.AppendLine( $"Email: {InputModel.Email}" );
            message.AppendLine( $"Phone Number: {InputModel.PhoneNumber}" );
            message.AppendLine();

            message.AppendLine( $"Company: {InputModel.Company}" );
            message.AppendLine( $"Address: {InputModel.Address}" );

            if ( !string.IsNullOrEmpty( InputModel.Address2 ) )
                message.AppendLine( $"Address 2: {InputModel.Address2}" );

            message.AppendLine( $"City: {InputModel.City}" );
            message.AppendLine( $"State: {InputModel.State}" );
            message.AppendLine( $"Zip: {InputModel.Zip}" );
            message.AppendLine( $"Country: {InputModel.Country}" );
            message.AppendLine( $"VAT: {InputModel.VAT}" );
            message.AppendLine();

            message.AppendLine( $"Payment Type: {InputModel.PaymentType}" );
            message.AppendLine();

            message.AppendLine( $"Additional Notes: {InputModel.Note}" );

            var emailMessage = new EmailMessage( EmailOptions.FromName, EmailOptions.FromAddress )
            {
                Subject = $"Product Order Request: {InputModel.FullName} - {InputModel.Company}",
                Body = message.ToString(),
                IsHtml = false,
            };

            var result = await EmailSender.Send( emailMessage, CancellationToken.None );

            if ( result.Succeeded )
            {
                await MessageService.Success( (MarkupString)"Order is sent.<br>We will contact you shortly." );

                InputModel = new ProductOrder();

                await validationsRef.ClearAll();
            }
            else
                await MessageService.Error( "Failed to send the order." );
        }
        catch ( Exception exc )
        {
            Logger.LogError( exc, "Error sending email." );
        }
    }

    #endregion

    #region Properties

    private ProductOrder InputModel { get; set; } = new ProductOrder()
    {
        Product = "Blazorise Professional",
    };

    [Inject] public EmailSender EmailSender { get; set; }

    [Inject] public IEmailOptions EmailOptions { get; set; }

    [Inject] public ILogger<PurchaseOrderPage> Logger { get; set; }

    [Inject] public IMessageService MessageService { get; set; }

    [Parameter] public string ProductName { get; set; }

    #endregion
}
