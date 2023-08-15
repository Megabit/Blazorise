using System.ComponentModel.DataAnnotations;

namespace Blazorise.Docs.Models;

public class ProductOrder
{
    [Required]
    public string Product { get; set; }

    [Required]
    [Display( Name = "Full Name" )]
    public string FullName { get; set; }

    [Required]
    public string Company { get; set; }

    [Required]
    public string VAT { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Display( Name = "Phone Number" )]
    public string PhoneNumber { get; set; }

    [Required]
    public string Address { get; set; }

    [Display( Name = "Address line 2" )]
    public string Address2 { get; set; }

    [Required]
    public string Country { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    [Display( Name = "Zip code" )]
    public string Zip { get; set; }

    public string Note { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [Display( Name = "Payment Type" )]
    public string PaymentType { get; set; } = "Bank Transfer";
}
