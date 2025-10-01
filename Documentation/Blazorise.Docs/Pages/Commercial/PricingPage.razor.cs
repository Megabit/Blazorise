using System.Collections.Generic;
using Blazorise.Docs.Models;

namespace Blazorise.Docs.Pages.Commercial
{
    public partial class PricingPage
    {
        public class PriceInfo
        {
            public int Price { get; set; }

            public string CurrencySymbol { get; set; }

            public string CurrencyCode { get; set; }

            public string Unit { get; set; }

            public string ProductId { get; set; }

            public object Upsell { get; set; }
        }

        private Dictionary<string, PriceInfo> professionalPrices = new Dictionary<string, PriceInfo>()
        {
            { "one-time", new PriceInfo { Unit = "year", Price = 659, CurrencySymbol = "€", CurrencyCode = "EUR", ProductId = PaddlePrices.ProfessionalOneTimePerDeveloper } },
            { "annually", new PriceInfo { Unit = "year", Price = 590, CurrencySymbol = "€", CurrencyCode = "EUR", ProductId = PaddlePrices.ProfessionalYearlySubscriptionPerDeveloper } },
            { "monthly", new PriceInfo { Unit = "month", Price = 59, CurrencySymbol = "€", CurrencyCode = "EUR", ProductId = PaddlePrices.ProfessionalMonthlySubscriptionPerDeveloper } },
        };

        private Dictionary<string, PriceInfo> enterprisePrices = new Dictionary<string, PriceInfo>()
        {
            { "one-time", new PriceInfo { Unit = "year", Price = 1080, CurrencySymbol = "€", CurrencyCode = "EUR", ProductId = PaddlePrices.EnterpriseOneTimePerDeveloper } },
            { "annually", new PriceInfo { Unit = "year", Price = 990, CurrencySymbol = "€", CurrencyCode = "EUR", ProductId = PaddlePrices.EnterpriseYearlySubscriptionPerDeveloper } },
            { "monthly", new PriceInfo { Unit = "month", Price = 99, CurrencySymbol = "€", CurrencyCode = "EUR", ProductId = PaddlePrices.EnterpriseMonthlySubscriptionPerDeveloper } },
        };

        int quantity = 1;

        string plan = "annually";
    }
}
