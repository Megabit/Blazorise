namespace Blazorise.Captcha.ReCaptcha;

/// <summary>
/// Reposition the reCAPTCHA badge. 'inline' lets you position it with CSS.
/// <para>This option is only available for invisible reCAPTCHA.</para>
/// </summary>
public enum ReCaptchaBadge
{
    /// <summary>
    /// Repositions the reCAPTCHA badge to the bottom right.
    /// </summary>
    BottomRight,

    /// <summary>
    /// Repositions the reCAPTCHA badge to the bottom left.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Repositions the reCAPTCHA badge inline.
    /// <para>This option lets you position it with CSS</para>
    /// </summary>
    Inline
}
