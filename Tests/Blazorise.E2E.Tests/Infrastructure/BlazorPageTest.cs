using Microsoft.AspNetCore.Mvc.Testing;

namespace Blazorise.E2E.Tests.Infrastructure;

/// <summary>
/// Credit to : https://www.youtube.com/watch?v=lJa3YlUliEs
/// </summary>
public class BlazorPageTest : PageTest
{

    protected static readonly Uri RootUri = new( "http://localhost:14695" );

    private readonly WebApplicationFactory<BasicTestApp.Server.Program> _webApplicationFactory = new() { };
    private HttpClient _httpClient;

    [SetUp]
    public async Task Setup()
    {
        _httpClient = _webApplicationFactory.CreateClient( new()
        {
            BaseAddress = RootUri,
        } );

        await Context.RouteAsync( $"{RootUri.AbsoluteUri}**", async route =>
        {
            var request = route.Request;
            var content = request.PostDataBuffer is { } postDataBuffer
                ? new ByteArrayContent( postDataBuffer )
                : null;

            var requestMessage = new HttpRequestMessage( new( request.Method ), request.Url )
            {
                Content = content
            };

            foreach ( var header in request.Headers )
            {
                requestMessage.Headers.Add( header.Key, header.Value );
            }

            var response = await _httpClient.SendAsync( requestMessage );
            var responseBody = await response.Content.ReadAsByteArrayAsync();
            var responseHeaders = response.Content.Headers.Select( x => KeyValuePair.Create( x.Key, x.Value.FirstOrDefault() ?? string.Empty ) );

            await route.FulfillAsync( new()
            {
                BodyBytes = responseBody,
                Headers = responseHeaders,
                Status = (int)response.StatusCode
            } );

        } );
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }

}
