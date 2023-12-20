using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using static Blazorise.Docs.Pages.Commercial.PricingPage;

namespace Blazorise.Docs.Components.Commercial
{
    public partial class PlansComparer
    {
        [Parameter] public int Quantity { get; set; }

        [Parameter] public string Plan { get; set; }

        [Parameter] public Dictionary<string, PriceInfo> ProfessionalPrices { get; set; }

        [Parameter] public Dictionary<string, PriceInfo> EnterprisePrices { get; set; }
    }
}
