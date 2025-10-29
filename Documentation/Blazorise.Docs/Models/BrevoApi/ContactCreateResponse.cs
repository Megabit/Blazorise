using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazorise.Docs.Models.BrevoApi;

public class BrevoContactCreateResponse
{
    [JsonPropertyName( "id" )]
    public long Id { get; set; }

    [JsonPropertyName( "email" )]
    public string Email { get; set; }

    [JsonPropertyName( "emailBlacklisted" )]
    public bool EmailBlacklisted { get; set; }

    [JsonPropertyName( "smsBlacklisted" )]
    public bool SmsBlacklisted { get; set; }

    [JsonPropertyName( "createdAt" )]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName( "modifiedAt" )]
    public DateTime? ModifiedAt { get; set; }

    [JsonPropertyName( "listIds" )]
    public List<int> ListIds { get; set; }

    [JsonPropertyName( "attributes" )]
    public Dictionary<string, object> Attributes { get; set; }

    [JsonPropertyName( "statistics" )]
    public BrevoContactStatistics Statistics { get; set; }
}