#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Docs.Components.Commercial;

public partial class Pricing
{
    protected async Task OnProfessionalClicked()
    {
        await JSRuntime.InvokeVoidAsync( "blazorisePRO.paddle.openCheckout", ProfessionalPrices[Plan].ProductId, Quantity );
    }

    protected async Task OnEnterpriseClicked()
    {
        await JSRuntime.InvokeVoidAsync( "blazorisePRO.paddle.openCheckout", EnterprisePrices[Plan].ProductId, Quantity );
    }

    private string DevLabel => Quantity == 1 ? "developer" : "developers";

    [Inject] IJSRuntime JSRuntime { get; set; }

    [Parameter] public int Quantity { get; set; }

    [Parameter] public EventCallback<int> QuantityChanged { get; set; }

    [Parameter] public string Plan { get; set; }

    [Parameter] public EventCallback<string> PlanChanged { get; set; }

    [Parameter] public Dictionary<string, CommercialPrice> ProfessionalPrices { get; set; }

    [Parameter] public Dictionary<string, CommercialPrice> EnterprisePrices { get; set; }
}