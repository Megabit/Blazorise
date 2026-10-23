#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Demo.Pages.Apps.InvoiceBuilder;

public record InvoiceParty
{
    public string CompanyName { get; set; }

    public string TaxRegistrationNumber { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string AddressLine1 { get; set; }

    public string AddressLine2 { get; set; }

    public string PostalCode { get; set; }

    public string City { get; set; }

    public string Country { get; set; } = string.Empty;

    public string CallingCode { get; set; } = "+93";

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public string Website { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string AdditionalInfo { get; set; }

    public string FormatContactDetails()
    {
        var details = new[]
        {
            $"{FirstName} {LastName}".Trim(),
            AddressLine1,
            AddressLine2,
            $"{PostalCode} {City}".Trim(),
            Country,
            string.IsNullOrWhiteSpace( PhoneNumber ) ? null : $"{CallingCode} {PhoneNumber}",
            Email,
            Website,
            string.IsNullOrWhiteSpace( TaxRegistrationNumber ) ? null : $"Tax Registration Number: {TaxRegistrationNumber}",
            AdditionalInfo,
        };

        return string.Join( Environment.NewLine, details.Where( detail => !string.IsNullOrWhiteSpace( detail ) ) );
    }
}