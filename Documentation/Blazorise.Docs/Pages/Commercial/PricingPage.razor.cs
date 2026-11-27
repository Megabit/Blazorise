#region Using directives
using System.Collections.Generic;
using Blazorise.Docs.Models;
#endregion

namespace Blazorise.Docs.Pages.Commercial
{
    public partial class PricingPage
    {
        private readonly Dictionary<string, CommercialPrice> professionalPrices = new()
        {
            { "one-time", CommercialPriceList.ProfessionalOneTime },
            { "annually", CommercialPriceList.ProfessionalAnnual },
            { "monthly", CommercialPriceList.ProfessionalMonthly },
        };

        private readonly Dictionary<string, CommercialPrice> enterprisePrices = new()
        {
            { "one-time", CommercialPriceList.EnterpriseOneTime },
            { "annually", CommercialPriceList.EnterpriseAnnual },
            { "monthly", CommercialPriceList.EnterpriseMonthly },
        };

        int quantity = 1;

        string plan = "annually";
    }
}