using System.ComponentModel.DataAnnotations;

namespace Blazorise.Docs.Models;

public class CustomWorkQuote
{
    [Required]
    [Display( Name = "Full Name" )]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [Display( Name = "Work Email" )]
    public string Email { get; set; }

    [Required]
    public string Company { get; set; }

    [Required]
    [Display( Name = "Engagement" )]
    public string Engagement { get; set; } = "Not sure";

    [Required]
    [Display( Name = "Request Title" )]
    public string Title { get; set; }

    [Required]
    [Display( Name = "Target Timeline" )]
    public string Timeline { get; set; }

    [Display( Name = "Budget Range" )]
    public string BudgetRange { get; set; } = "Not sure";

    [Display( Name = "Upstream Preference" )]
    public string UpstreamPreference { get; set; } = "Upstream (default)";

    [Display( Name = "Exclusivity Duration (months)" )]
    public int? ExclusivityMonths { get; set; } = 6;

    [Display( Name = "Blazorise Version" )]
    public string BlazoriseVersion { get; set; }

    [Display( Name = ".NET Version" )]
    public string DotNetVersion { get; set; }

    [Display( Name = "CSS Providers" )]
    public string CssProviders { get; set; }

    public string Dependencies { get; set; }

    [Required]
    [Display( Name = "Details" )]
    public string Details { get; set; }
}