#region Using directives
using Blazorise.Localization;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Handlers for custom PivotGrid localizations.
/// </summary>
public class PivotGridLocalizers
{
    /// <summary>
    /// Custom localization handler for empty text.
    /// </summary>
    public TextLocalizerHandler EmptyLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for no data text.
    /// </summary>
    public TextLocalizerHandler NoDataLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for missing value fields text.
    /// </summary>
    public TextLocalizerHandler MissingValuesLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for grand total text.
    /// </summary>
    public TextLocalizerHandler GrandTotalLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for total text.
    /// </summary>
    public TextLocalizerHandler TotalLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for values text.
    /// </summary>
    public TextLocalizerHandler ValuesLocalizer { get; set; }
}