using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static Blazorise.Docs.Pages.Commercial.PricingPage;

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

    Task OnProductOrderClicked()
    {
        NavigationManager.NavigateTo( $"purchase-order" );

        return Task.CompletedTask;
    }

    private string DevLabel => Quantity == 1 ? "developer" : "developers";

    [Inject] IJSRuntime JSRuntime { get; set; }

    [Inject] NavigationManager NavigationManager { get; set; }

    [Parameter] public int Quantity { get; set; }

    [Parameter] public EventCallback<int> QuantityChanged { get; set; }

    [Parameter] public string Plan { get; set; }

    [Parameter] public EventCallback<string> PlanChanged { get; set; }

    [Parameter] public Dictionary<string, PriceInfo> ProfessionalPrices { get; set; }

    [Parameter] public Dictionary<string, PriceInfo> EnterprisePrices { get; set; }
}
