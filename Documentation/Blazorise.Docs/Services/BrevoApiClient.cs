#region Using directives
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Models.BrevoApi;
using Blazorise.Docs.Options;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.Services;

public interface IBrevoApiClient
{
    Task<BrevoContactCreateResponse> CreateContactAsync( string email, Dictionary<string, object> attributes = null, List<int> listIds = null, CancellationToken cancellationToken = default );
}

public class BrevoApiClient : IBrevoApiClient
{
    #region Members

    private readonly HttpClient httpClient;

    private readonly BrevoApiOptions options;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    #endregion

    #region Constructors

    public BrevoApiClient( HttpClient httpClient, IOptions<BrevoApiOptions> options )
    {
        this.httpClient = httpClient;
        this.options = options.Value;

        httpClient.BaseAddress = new Uri( this.options.BaseUrl.TrimEnd( '/' ) + "/" );
        httpClient.DefaultRequestHeaders.Add( "api-key", this.options.ApiKey );
        httpClient.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
    }

    #endregion

    #region Methods

    public async Task<BrevoContactCreateResponse> CreateContactAsync(
            string email,
            Dictionary<string, object> attributes = null,
            List<int> listIds = null,
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

    #endregion
}