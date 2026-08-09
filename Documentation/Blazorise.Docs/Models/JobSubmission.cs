using System;

namespace Blazorise.Docs.Models;

/// <summary>
/// Represents a job submitted for moderation.
/// </summary>
public sealed class JobSubmission
{
    public string CompanyName { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Remote { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = string.Empty;
    public string Seniority { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string ApplyUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SalaryRange { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public bool Confirmation { get; set; }
}