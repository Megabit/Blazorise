using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Docs.Models;

namespace Blazorise.Docs.Pages.Commercial
{
    public partial class PricingPage
    {
        public class PriceInfo
        {
            public int Price { get; set; }

            public string Currency { get; set; }

            public int HigherPrice { get; set; }

            public string Unit { get; set; }

            public string ProductId { get; set; }

            public object Upsell { get; set; }
        }

        private Dictionary<string, PriceInfo> professionalPrices = new Dictionary<string, PriceInfo>()
        {
            { "one-time", new PriceInfo { Unit = "year", Price = 659, Currency = "€", ProductId = PaddlePrices.ProfessionalOneTimePerDeveloper, Upsell = new { Id = PaddlePrices.ProfessionalBundle, Title = "Upgrade to Professional Bundle", Text = "<p>Consider the <strong>Blazorise Professional Bundle</strong>!</p><p>Besides standard features, it offers exclusive <strong>themes</strong> for a richer, more engaging user experience.</p><p>Stand out with stunning design, all in one upgrade. Enhance your project now!</p>", Action = "Buy Bundle »" } } },
            { "annually", new PriceInfo { Unit = "year", Price = 590, Currency = "€", HigherPrice = 708, ProductId = PaddlePrices.ProfessionalYearlySubscriptionPerDeveloper } },
            { "monthly", new PriceInfo { Unit = "month", Price = 59, Currency = "€", ProductId = PaddlePrices.ProfessionalMonthlySubscriptionPerDeveloper } },
        };

        private Dictionary<string, PriceInfo> enterprisePrices = new Dictionary<string, PriceInfo>()
        {
            { "one-time", new PriceInfo { Unit = "year", Price = 1080, Currency = "€", ProductId = PaddlePrices.EnterpriseOneTimePerDeveloper } },
            { "annually", new PriceInfo { Unit = "year", Price = 990, Currency = "€", HigherPrice = 1188, ProductId = PaddlePrices.EnterpriseYearlySubscriptionPerDeveloper } },
            { "monthly", new PriceInfo { Unit = "month", Price = 99, Currency = "€", ProductId = PaddlePrices.EnterpriseMonthlySubscriptionPerDeveloper } },
        };

        int quantity = 1;

        string plan = "annually";
    }
}
