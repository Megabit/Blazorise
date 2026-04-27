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

    /// <summary>
    /// Custom localization handler for pagination aria-label text.
    /// </summary>
    public TextLocalizerHandler PaginationLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for first page button text.
    /// </summary>
    public TextLocalizerHandler FirstPageButtonLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for previous page button text.
    /// </summary>
    public TextLocalizerHandler PreviousPageButtonLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for next page button text.
    /// </summary>
    public TextLocalizerHandler NextPageButtonLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for last page button text.
    /// </summary>
    public TextLocalizerHandler LastPageButtonLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for items per page text.
    /// </summary>
    public TextLocalizerHandler ItemsPerPageLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for total items text.
    /// </summary>
    public TextLocalizerHandler NumbersOfItemsLocalizer { get; set; }
}