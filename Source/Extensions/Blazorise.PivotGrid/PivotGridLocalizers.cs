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
    /// Custom localization handler for fields text.
    /// </summary>
    public TextLocalizerHandler FieldsLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for available fields text.
    /// </summary>
    public TextLocalizerHandler AvailableFieldsLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for field chooser drag instruction text.
    /// </summary>
    public TextLocalizerHandler DragFieldsLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for field chooser drop target text.
    /// </summary>
    public TextLocalizerHandler DropFieldLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for rows text.
    /// </summary>
    public TextLocalizerHandler RowsLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for columns text.
    /// </summary>
    public TextLocalizerHandler ColumnsLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for filters text.
    /// </summary>
    public TextLocalizerHandler FiltersLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for apply text.
    /// </summary>
    public TextLocalizerHandler ApplyLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for cancel text.
    /// </summary>
    public TextLocalizerHandler CancelLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for all text.
    /// </summary>
    public TextLocalizerHandler AllLocalizer { get; set; }

    /// <summary>
    /// Custom localization handler for aggregate text.
    /// </summary>
    public TextLocalizerHandler AggregateLocalizer { get; set; }

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