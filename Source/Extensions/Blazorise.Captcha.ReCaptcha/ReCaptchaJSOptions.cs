namespace Blazorise.Captcha.ReCaptcha;

/// <summary>
/// Represents JavaScript options for configuring a reCAPTCHA component.
/// </summary>
public class ReCaptchaJSOptions
{
    /// <summary>
    /// Gets or sets the site key provided by reCAPTCHA for authentication and validation.
    /// </summary>
    public string SiteKey { get; set; }

    /// <summary>
    /// Gets or sets the theme of the reCAPTCHA widget (e.g., "light" or "dark").
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// Gets or sets the size of the reCAPTCHA widget (e.g., "normal", "compact", or "invisible").
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// Gets or sets the position of the badge for invisible reCAPTCHA (e.g., "bottomright", "bottomleft", or "inline").
    /// </summary>
    public string Badge { get; set; }

    /// <summary>
    /// Gets or sets the language code for the reCAPTCHA widget, allowing customization of the widget's language.
    /// </summary>
    public string Language { get; set; }
}