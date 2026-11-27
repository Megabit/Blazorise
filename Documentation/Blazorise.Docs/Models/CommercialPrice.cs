#region Using directives
using System;
#endregion

namespace Blazorise.Docs.Models;

public sealed record CommercialPrice(
    string Name,
    decimal Price,
    decimal? BasePriceIncludingVat,
    DateOnly? BasePriceDate,
    string Unit,
    string QuantityUnit,
    string ProductId )
{
    public const decimal CroatianVatRate = 0.25m;

    public decimal PriceIncludingVat => decimal.Round( Price * ( 1 + CroatianVatRate ), 2, MidpointRounding.AwayFromZero );

    // Historical VAT must remain independent of any future change to the current rate.
    public decimal BasePriceVatRate { get; init; } = 0.25m;

    public decimal? BasePriceExcludingVat => BasePriceIncludingVat / ( 1 + BasePriceVatRate );

    public bool IsPromotionalPrice { get; init; }

    public string CurrencySymbol => "€";

    public string CurrencyCode => "EUR";
}