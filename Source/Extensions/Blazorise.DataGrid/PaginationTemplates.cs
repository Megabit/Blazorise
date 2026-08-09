#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Supports pagination templates behavior in DataGrid components.
/// </summary>
public class PaginationTemplates<TItem>
{
    /// <summary>
    /// Template used to render page button.
    /// </summary>
    public RenderFragment<PageButtonContext> PageButtonTemplate { get; set; }

    /// <summary>
    /// Template used to render first page button.
    /// </summary>
    public RenderFragment FirstPageButtonTemplate { get; set; }

    /// <summary>
    /// Template used to render last page button.
    /// </summary>
    public RenderFragment LastPageButtonTemplate { get; set; }

    /// <summary>
    /// Template used to render previous page button.
    /// </summary>
    public RenderFragment PreviousPageButtonTemplate { get; set; }

    /// <summary>
    /// Template used to render next page button.
    /// </summary>
    public RenderFragment NextPageButtonTemplate { get; set; }

    /// <summary>
    /// Template used to render items per page.
    /// </summary>
    public RenderFragment ItemsPerPageTemplate { get; set; }

    /// <summary>
    /// Template used to render total items short.
    /// </summary>
    public RenderFragment<PaginationContext<TItem>> TotalItemsShortTemplate { get; set; }

    /// <summary>
    /// Template used to render total items.
    /// </summary>
    public RenderFragment<PaginationContext<TItem>> TotalItemsTemplate { get; set; }

    /// <summary>
    /// Template used to render page selector.
    /// </summary>
    public RenderFragment<PaginationContext<TItem>> PageSelectorTemplate { get; set; }

    /// <summary>
    /// Template used to render page sizes.
    /// </summary>
    public RenderFragment<PaginationContext<TItem>> PageSizesTemplate { get; set; }
}