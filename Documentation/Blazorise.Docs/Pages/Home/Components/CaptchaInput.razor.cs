#region Using directives
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Captcha;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
#endregion

namespace Blazorise.Docs.Pages.Home.Components;

public partial class CaptchaInput : BaseInputComponent<bool>
{
    #region Members

    public class GoogleResponse
    {
        public bool Success { get; set; }
        public double Score { get; set; } //V3 only - The score for this request (0.0 - 1.0)
        public string Action { get; set; } //v3 only - An identifier
        public string Challenge_ts { get; set; }
        public string Hostname { get; set; }
        public string ErrorCodes { get; set; }
    }

    protected Captcha.Captcha captchaRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            if ( parameters.TryGetValue<bool>( nameof( Value ), out var paramChecked ) && !paramChecked.IsEqual( Value ) )
            {
                ExecuteAfterRender( Revalidate );
            }
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation is not null )
        {
            if ( parameters.TryGetValue<Expression<Func<bool>>>( nameof( ValueExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
        }

        if ( Rendered && captchaRef.State.Valid && !Value )
        {
            await captchaRef.Reset();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.CheckValidation( ParentValidation?.Status ?? ValidationStatus.None ) );

        base.BuildClasses( builder );
    }

    protected async Task Solved( CaptchaState state )
    {
        await CurrentValueHandler( state.Valid.ToString() );
    }

    protected async Task Expired()
    {
        await CurrentValueHandler( false.ToString() );
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

    protected override Task<ParseValue<bool>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<bool>( true, bool.Parse( value ), null ) );
    }

    protected override Task OnInternalValueChanged( bool value )
    {
        return ValueChanged.InvokeAsync( value );
    }

    public static void ValidateRobot( ValidatorEventArgs eventArgs )
    {
        eventArgs.Status = bool.Parse( eventArgs.Value.ToString() ) ? ValidationStatus.Success : ValidationStatus.Error;

        if ( eventArgs.Status == ValidationStatus.Error )
            eventArgs.ErrorText = "Please check to confirm you're a real human!";
        else
            eventArgs.ErrorText = null;
    }

    /// <inheritdoc/>
    protected override string GetFormatedValueExpression()
    {
        if ( ValueExpression is null )
            return null;

        return HtmlFieldPrefix is not null
            ? HtmlFieldPrefix.GetFieldName( ValueExpression )
            : ExpressionFormatter.FormatLambda( ValueExpression );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override bool InternalValue { get => Value; set => Value = value; }

    [Inject] IOptions<AppSettings> AppSettings { get; set; }
    [Inject] IHttpClientFactory HttpClientFactory { get; set; }

    [Parameter] public bool Value { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the captcha valid value.
    /// </summary>
    [Parameter] public Expression<Func<bool>> ValueExpression { get; set; }

    #endregion
}
