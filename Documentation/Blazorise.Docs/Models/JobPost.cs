using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazorise.Docs.Models;

/// <summary>
/// Represents a job post from the jobs feed.
/// </summary>
public sealed class JobPost
{
    /// <summary>
    /// Gets or sets the job identifier.
    /// </summary>
    [JsonPropertyName( "id" )]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the job creation date.
    /// </summary>
    [JsonPropertyName( "createdAt" )]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the job update date.
    /// </summary>
    [JsonPropertyName( "updatedAt" )]
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the job title.
    /// </summary>
    [JsonPropertyName( "title" )]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the company name.
    /// </summary>
    [JsonPropertyName( "company" )]
    public string Company { get; set; }

    /// <summary>
    /// Gets or sets the job location.
    /// </summary>
    [JsonPropertyName( "location" )]
    public string Location { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the role is remote.
    /// </summary>
    [JsonPropertyName( "remote" )]
    public bool Remote { get; set; }

    /// <summary>
    /// Gets or sets the employment type.
    /// </summary>
    [JsonPropertyName( "employmentType" )]
    public string EmploymentType { get; set; }

    /// <summary>
    /// Gets or sets the seniority level.
    /// </summary>
    [JsonPropertyName( "seniority" )]
    public string Seniority { get; set; }

    /// <summary>
    /// Gets or sets the job tags.
    /// </summary>
    [JsonPropertyName( "tags" )]
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the apply URL.
    /// </summary>
    [JsonPropertyName( "applyUrl" )]
    public string ApplyUrl { get; set; }

    /// <summary>
    /// Gets or sets the job description.
    /// </summary>
    [JsonPropertyName( "description" )]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the salary range.
    /// </summary>
    [JsonPropertyName( "salaryRange" )]
    public string SalaryRange { get; set; }

    /// <summary>
    /// Gets or sets the job expiry date.
    /// </summary>
    [JsonPropertyName( "expiryDate" )]
    public DateTime? ExpiryDate { get; set; }
}