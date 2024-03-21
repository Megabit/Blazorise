#region Using directives
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Captcha;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.Pages.Home;

public class CaptchaPage : ComponentBase
{
    protected Captcha.Captcha captcha;
    protected bool? captchaValid;

    [Inject] IOptions<AppSettings> AppSettings { get; set; }
    [Inject] IHttpClientFactory HttpClientFactory { get; set; }

    protected void Solved( CaptchaState state )
    {
        captchaValid = state.Valid;
    }

    protected void Expired()
    {
        captchaValid = false;
    }

    protected async Task<bool> Validate( CaptchaState state )
    {
        //Perform server side validation
        //You should make sure to implement server side validation
        //https://developers.google.com/recaptcha/docs/verify
        //Here's a simple example:
        var content = new FormUrlEncodedContent( new[]
        {
            new KeyValuePair<string, string>("secret", AppSettings.Value.ReCaptchaServerKey),
            new KeyValuePair<string, string>("response", state.Response),
         } );

        var httpClient = HttpClientFactory.CreateClient();
        var response = await httpClient.PostAsync( "https://www.google.com/recaptcha/api/siteverify", content );

        var result = await response.Content.ReadAsStringAsync();
        var googleResponse = JsonSerializer.Deserialize<GoogleResponse>( result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        } );

        return googleResponse.Success;
    }

    public class GoogleResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; } //V3 only - The score for this request (0.0 - 1.0)
        public string Action { get; set; } //v3 only - An identifier
        public string Challenge_ts { get; set; }
        public string Hostname { get; set; }
        public string ErrorCodes { get; set; }
    }
}
