using Blazorise.Docs.Core;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Docs.Pages
{
    public class BasePage : ComponentBase
    {
        protected string MailToLink
           => $"mailto:{EmailOptions.ReplyToAddress}";

        protected string MailToAddress
            => EmailOptions.ReplyToAddress;

        [Inject] protected NavigationManager NavigationManager { get; set; }

        [Inject] protected IEmailOptions EmailOptions { get; set; }        
    }
}
