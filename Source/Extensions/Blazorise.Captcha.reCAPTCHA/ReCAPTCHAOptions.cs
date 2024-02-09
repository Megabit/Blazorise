namespace Blazorise.Captcha.ReCaptcha;
public class ReCaptchaOptions
{
    public string SiteKey { get; set; }

    public ReCaptchaSize Size { get; set; } = ReCaptchaSize.Normal;

    public ReCaptchaTheme Theme { get; set; } = ReCaptchaTheme.Light;

    /// <summary>
    /// Refer to: https://developers.google.com/recaptcha/docs/language
    /// <para>Defaults to: 'en' (English (US))</para>
    /// </summary>
    public string LanguageCode { get; set; } = "en";
}
