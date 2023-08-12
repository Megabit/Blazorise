namespace Blazorise.Docs.Models;

public class ThemeInfo
{
    public ThemeInfo( int id, string licenseName, string licenseDescription, decimal price, string currency )
    {
        Id = id;
        LicenseName = licenseName;
        LicenseDescription = licenseDescription;
        Price = price;
        Currency = currency;
    }

    public int Id { get; set; }

    public string LicenseName { get; set; }

    public string LicenseDescription { get; set; }

    public decimal Price { get; set; }

    public string Currency { get; set; }
}
