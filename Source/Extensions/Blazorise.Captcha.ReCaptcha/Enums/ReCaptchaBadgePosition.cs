namespace Blazorise.Captcha.ReCaptcha;

/// <summary>
/// Reposition the reCAPTCHA badge. 'inline' lets you position it with CSS.
/// <para>This option is only available for invisible reCAPTCHA.</para>
/// </summary>
public enum ReCaptchaBadgePosition
{
    /// <summary>
    /// Repositions the reCAPTCHA badge to the bottom end.
    /// </summary>
    BottomEnd,

    /// <summary>
    /// Repositions the reCAPTCHA badge to the bottom start.
    /// </summary>
    BottomStart,

    /// <summary>
    /// Repositions the reCAPTCHA badge inline.
    /// <para>This option lets you position it with CSS</para>
    /// </summary>
    Inline,
}
