namespace Blazorise.Docs.Models;

public class ThemeInfo
{
    public ThemeInfo( string priceId, string licenseName, string licenseDescription, decimal price, string currency )
    {
        PriceId = priceId;
        LicenseName = licenseName;
        LicenseDescription = licenseDescription;
        Price = price;
        Currency = currency;
    }

    public string PriceId { get; set; }

    public string LicenseName { get; set; }

    public string LicenseDescription { get; set; }

    public decimal Price { get; set; }

    public string Currency { get; set; }
}
