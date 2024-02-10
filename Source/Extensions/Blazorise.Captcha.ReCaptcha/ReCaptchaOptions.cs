namespace Blazorise.Captcha.ReCaptcha;

/// <summary>
/// Please refer to https://developers.google.com/recaptcha/intro for proper guidance.
/// </summary>
public class ReCaptchaOptions
{
    /// <summary>
    /// The client side api key for google reCAPTCHA.
    /// </summary>
    public string SiteKey { get; set; }

    /// <summary>
    /// The size of the widget.
    /// </summary>
    public ReCaptchaSize Size { get; set; } = ReCaptchaSize.Normal;

    /// <summary>
    /// The color theme of the widget.
    /// </summary>
    public ReCaptchaTheme Theme { get; set; } = ReCaptchaTheme.Light;

    /// <summary>
    /// Reposition the reCAPTCHA badge. 'inline' lets you position it with CSS.
    /// <para>This option is only available for invisible reCAPTCHA.</para>
    /// </summary>
    public ReCaptchaBadgePosition BadgePosition { get; set; } = ReCaptchaBadgePosition.BottomEnd;

    /// <summary>
    /// Refer to: https://developers.google.com/recaptcha/docs/language
    /// <para>Defaults to: 'en' (English (US))</para>
    /// </summary>
    public string LanguageCode { get; set; } = "en";
}
