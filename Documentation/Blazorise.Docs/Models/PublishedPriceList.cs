#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Docs.Models;

public sealed record PublishedPriceList( int Number, DateTime? PublishedAt, IReadOnlyList<CommercialPrice> Prices, decimal VatRate )
{
    public string FileName => PublishedAt is { } publishedAt
        ? FormattableString.Invariant( $"11987636635_ZA_SVA_PRODAJNA_MJESTA_USLUGE_{Number:D6}_{publishedAt:yyyyMMdd_HHmm}.csv" )
        : "price-list-preview.csv";
}