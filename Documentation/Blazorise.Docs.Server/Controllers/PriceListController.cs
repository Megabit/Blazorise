#region Using directives
using System.Text;
using Blazorise.Docs.Models;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace Blazorise.Docs.Server.Controllers;

[ApiController]
[Route( "pricing" )]
[ResponseCache( NoStore = true, Location = ResponseCacheLocation.None )]
public sealed class PriceListController : ControllerBase
{
    #region Methods

    [HttpGet( "price-list.csv" )]
    public IActionResult Current() => Download( CommercialPriceList.Current.FileName );

    [HttpGet( "price-lists/{fileName}" )]
    public IActionResult Download( string fileName )
    {
        var publication = CommercialPriceList.FindPriceList( fileName );

        if ( publication is null )
        {
            return NotFound();
        }

        var csv = CommercialPriceList.CreateCsv( publication.Prices, publication.VatRate );

        return File( Encoding.UTF8.GetBytes( csv ), "text/csv; charset=utf-8", publication.FileName );
    }

    #endregion
}