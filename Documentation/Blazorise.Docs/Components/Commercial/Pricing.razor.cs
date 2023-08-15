using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Docs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Docs.Components.Commercial;

public partial class Pricing
{
    private class PriceInfo
    {
        public int Price { get; set; }

        public int HigherPrice { get; set; }

        public string Unit { get; set; }

        public int ProductId { get; set; }

        public object Upsell { get; set; }
    }

    private Dictionary<string, PriceInfo> professionalPrices = new Dictionary<string, PriceInfo>()
    {
        { "one-time", new PriceInfo { Unit = "year", Price = 659, ProductId = PaddlePrices.ProfessionalOneTimePerDeveloper, Upsell = new { Id = PaddlePrices.ProfessionalBundle, Title = "Upgrade to Professional Bundle", Text = "<p>Consider the <strong>Blazorise Professional Bundle</strong>!</p><p>Besides standard features, it offers exclusive <strong>themes</strong> for a richer, more engaging user experience.</p><p>Stand out with stunning design, all in one upgrade. Enhance your project now!</p>", Action = "Buy Bundle »" } } },
        { "annually", new PriceInfo { Unit = "year", Price = 599, HigherPrice = 708, ProductId = PaddlePrices.ProfessionalYearlySubscriptionPerDeveloper } },
        { "monthly", new PriceInfo { Unit = "month", Price = 59, ProductId = PaddlePrices.ProfessionalMonthlySubscriptionPerDeveloper } },
    };

    private Dictionary<string, PriceInfo> enterprisePrices = new Dictionary<string, PriceInfo>()
    {
        { "one-time", new PriceInfo { Unit = "year", Price = 1099, ProductId = PaddlePrices.EnterpriseOneTimePerDeveloper } },
        { "annually", new PriceInfo { Unit = "year", Price = 999, HigherPrice = 1188, ProductId = PaddlePrices.EnterpriseYearlySubscriptionPerDeveloper } },
        { "monthly", new PriceInfo { Unit = "month", Price = 99, ProductId = PaddlePrices.EnterpriseMonthlySubscriptionPerDeveloper } },
    };

    string plan = "annually";

    int quantity = 1;

    string selectedTab = "commercial";

    private Task OnSelectedTabChanged( string name )
    {
        selectedTab = name;

        return Task.CompletedTask;
    }

    protected async Task OnProfessionalClicked()
    {
        await JSRuntime.InvokeVoidAsync( "blazorisePRO.paddle.openCheckout", professionalPrices[plan].ProductId, quantity, professionalPrices[plan].Upsell );
    }

    protected async Task OnEnterpriseClicked()
    {
        await JSRuntime.InvokeVoidAsync( "blazorisePRO.paddle.openCheckout", enterprisePrices[plan].ProductId, quantity );
    }

    Task OnProductOrderClicked()
    {
        NavigationManager.NavigateTo( $"purchase-order" );

        return Task.CompletedTask;
    }

    [Inject] IJSRuntime JSRuntime { get; set; }

    [Inject] NavigationManager NavigationManager { get; set; }
}
