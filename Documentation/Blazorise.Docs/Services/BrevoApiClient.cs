using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Models.BrevoApi;

namespace Blazorise.Docs.Services;

public interface IBrevoApiClient
{
    Task<BrevoContactCreateResponse> CreateContactAsync(
            string email,
            Dictionary<string, object>? attributes = null,
            Dictionary<string, object>? listIds = null,
            CancellationToken cancellationToken = default );
}

public class BrevoApiClient : IBrevoApiClient
{
    private readonly HttpClient httpClient;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BrevoApiClient( HttpClient httpClient )
    {
        this.httpClient = httpClient;
    }

    public async Task<BrevoContactCreateResponse> CreateContactAsync(
            string email,
            Dictionary<string, object>? attributes = null,
            Dictionary<string, object>? listIds = null,
            CancellationToken cancellationToken = default )
    {
        var body = new Dictionary<string, object>
        {
            ["email"] = email
        };
        if ( attributes is not null )
            body["attributes"] = attributes;
        if ( listIds is not null )
            body["listIds"] = listIds;

        var json = JsonSerializer.Serialize( body );
        using var content = new StringContent( json, Encoding.UTF8, "application/json" );
        using var response = await httpClient.PostAsync( "contacts", content, cancellationToken );
        var payload = await response.Content.ReadAsStringAsync( cancellationToken );

        if ( !response.IsSuccessStatusCode )
        {
            return default;
        }

        var result = JsonSerializer.Deserialize<BrevoContactCreateResponse>( payload, JsonOpts )
                     ?? new BrevoContactCreateResponse();
       
        return result;
    }
}