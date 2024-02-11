namespace Blazorise.Captcha;

/// <summary>
/// Provides contextual information about the Captcha state.
/// </summary>
public class CaptchaState
{
    /// <summary>
    /// Gets or sets the Captcha validation result.
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// Gets or sets the Captcha response from the API.
    /// </summary>
    public string Response { get; set; }
}
