#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#endregion

namespace Blazorise.Docs.Models;

public static class CommercialPriceList
{
    #region Members

    // Historical amounts include Croatian VAT. Never derive these from current prices or VAT rates.
    private static readonly DateOnly referenceDate = new( 2026, 9, 10 );

    #endregion

    #region Methods

    public static CommercialPrice FindByProductId( string productId )
        => PublishedPrices.FirstOrDefault( price => string.Equals( price.ProductId, productId, StringComparison.Ordinal ) );

    public static PublishedPriceList FindPriceList( string fileName )
    {
        if ( string.Equals( Current.FileName, fileName, StringComparison.Ordinal ) )
        {
            return Current;
        }

        return Previous.FirstOrDefault( publication => string.Equals( publication.FileName, fileName, StringComparison.Ordinal ) );
    }

    public static string CreateCsv( IReadOnlyList<CommercialPrice> prices, decimal vatRate = CommercialPrice.CroatianVatRate )
    {
        var culture = CultureInfo.GetCultureInfo( "hr-HR" );
        var builder = new StringBuilder( "Service name;Retail price (EUR, including VAT);Promotional price;Base price (EUR, including VAT)" );

        foreach ( var price in prices )
        {
            if ( price.BasePriceIncludingVat is null )
            {
                throw new InvalidOperationException( $"A confirmed base price is required for {price.Name}." );
            }

            var retailPrice = decimal.Round( price.Price * ( 1 + vatRate ), 2, MidpointRounding.AwayFromZero );

            builder.Append( "\r\n\"" );
            builder.Append( price.Name.Replace( "\"", "\"\"" ) );
            builder.Append( "\";" );
            builder.Append( retailPrice.ToString( "F2", culture ) );
            builder.Append( price.IsPromotionalPrice ? ";YES;" : ";NO;" );
            builder.Append( price.BasePriceIncludingVat.Value.ToString( "F2", culture ) );
        }

        return builder.ToString();
    }

    #endregion

    #region Properties

    // Indie is unreleased. Its first-offer baseline is €149 excluding VAT; no launch date is invented.
    public static CommercialPrice IndieAnnual { get; } = new( "Blazorise Indie, annual, per project", CommercialLicensePrices.IndieAnnualPerProject, 186.25m, null, "year", "project", PaddlePrices.IndieYearlySubscriptionPerProject );

    public static CommercialPrice ProfessionalAnnual { get; } = new( "Blazorise Professional, annual, per developer", CommercialLicensePrices.ProfessionalAnnualPerDeveloper, 737.50m, referenceDate, "year", "developer", PaddlePrices.ProfessionalYearlySubscriptionPerDeveloper );

    public static CommercialPrice ProfessionalMonthly { get; } = new( "Blazorise Professional, monthly, per developer", 59m, 73.75m, referenceDate, "month", "developer", PaddlePrices.ProfessionalMonthlySubscriptionPerDeveloper );

    public static CommercialPrice EnterpriseAnnual { get; } = new( "Blazorise Enterprise, annual, per developer", CommercialLicensePrices.EnterpriseAnnualPerDeveloper, 1237.50m, referenceDate, "year", "developer", PaddlePrices.EnterpriseYearlySubscriptionPerDeveloper );

    public static CommercialPrice EnterpriseMonthly { get; } = new( "Blazorise Enterprise, monthly, per developer", 99m, 123.75m, referenceDate, "month", "developer", PaddlePrices.EnterpriseMonthlySubscriptionPerDeveloper );

    // These legacy options are not offered by the billing selector or included in the published list.
    public static CommercialPrice ProfessionalOneTime { get; } = new( "Blazorise Professional, one-time", 659m, null, null, "year", "developer", PaddlePrices.ProfessionalOneTimePerDeveloper );

    public static CommercialPrice EnterpriseOneTime { get; } = new( "Blazorise Enterprise, one-time", 1080m, null, null, "year", "developer", PaddlePrices.EnterpriseOneTimePerDeveloper );

    public static CommercialPrice BlazeStackSingleSite { get; } = new( "BlazeStack, single site, one-time", 39m, 48.75m, referenceDate, null, "license", PaddlePrices.SinglesiteBlazeStackTheme );

    public static CommercialPrice BlazeStackMultisite { get; } = new( "BlazeStack, unlimited sites, one-time", 99m, 123.75m, referenceDate, null, "license", PaddlePrices.MultisiteBlazeStackTheme );

    public static IReadOnlyList<CommercialPrice> PublishedPrices { get; } = Array.AsReadOnly<CommercialPrice>(
    [
        ProfessionalAnnual,
        ProfessionalMonthly,
        EnterpriseAnnual,
        EnterpriseMonthly,
        IndieAnnual,
        BlazeStackSingleSite,
        BlazeStackMultisite,
    ] );

    // Set the actual Croatian local publication date and time when this draft is released.
    public static PublishedPriceList Current { get; } = new( 1, null, PublishedPrices, 0.25m );

    // Add published revisions here before changing their prices. Use literal historical amounts,
    // dates and VAT rates, rather than references to the current catalogue or price constants.
    public static IReadOnlyList<PublishedPriceList> Previous { get; } = Array.Empty<PublishedPriceList>();

    #endregion
}