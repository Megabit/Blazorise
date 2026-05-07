#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the validation feedback fragment that matches the current <see cref="ValidationStatus"/>.
/// </summary>
public partial class ValidationFeedback : BaseValidationResult
{
    #region Properties

    /// <summary>
    /// Content rendered when the validation status is <see cref="ValidationStatus.None"/>.
    /// </summary>
    [Parameter] public RenderFragment None { get; set; }

    /// <summary>
    /// Content rendered when the validation status is <see cref="ValidationStatus.Success"/>.
    /// </summary>
    [Parameter] public RenderFragment Success { get; set; }

    /// <summary>
    /// Content rendered when the validation status is <see cref="ValidationStatus.Warning"/>.
    /// </summary>
    [Parameter] public RenderFragment Warning { get; set; }

    /// <summary>
    /// Content rendered when the validation status is <see cref="ValidationStatus.Error"/>.
    /// </summary>
    [Parameter] public RenderFragment Error { get; set; }

    /// <summary>
    /// If true, shows multiline validation messages.
    /// </summary>
    [Parameter] public bool Multiline { get; set; }

    /// <summary>
    /// If true, shows the validation message as a tooltip.
    /// </summary>
    [Parameter] public bool Tooltip { get; set; }

    #endregion
}