namespace Blazorise;

/// <summary>
/// Represents the options available for configuring the behavior of a Tooltip in Blazorise.
/// </summary>
public class BlazoriseTooltipOptions
{
    /// <summary>
    /// Specifies the delay in milliseconds (ms) after a trigger event before a Tooltip is shown. Default is <c>null</c> for no delay.
    /// </summary>
    public int? ShowDelay { get; set; }

    /// <summary>
    /// Specifies the delay in milliseconds (ms) after a trigger event before a Tooltip is hidden. Default is <c>null</c> for no delay.
    /// </summary>
    public int? HideDelay { get; set; }
}