#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid;

partial class _DataGridPagination<TItem> : BaseComponent, IDisposable
{
    #region Members 

    private bool dropdownColumnChooserVisible;

    #endregion

    #region Methods

    private bool ShowButtonRow()
        => ButtonRowTemplate is not null && ParentDataGrid.IsButtonRowVisible;

    private bool ShowColumnChooser()
        => ParentDataGrid.ShowColumnChooser;

    private PagerElementPosition GetButtonRowPosition()
        => ParentDataGrid.PagerOptions?.ButtonRowPosition ?? PagerElementPosition.Default;

    private PagerElementPosition GetColumnChooserPosition()
        => ParentDataGrid.PagerOptions?.ColumnChooserPosition ?? PagerElementPosition.Default;

    private PagerElementPosition GetPaginationPosition()
        => ParentDataGrid.PagerOptions?.PaginationPosition ?? PagerElementPosition.Default;

    private PagerElementPosition GetTotalItemsPosition()
        => ParentDataGrid.PagerOptions?.TotalItemsPosition ?? PagerElementPosition.Default;

    private Size GetSize()
        => ParentDataGrid.PagerOptions?.ButtonSize ?? ParentDataGrid.Theme?.PaginationOptions?.Size ?? Size.Default;

    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }

        base.Dispose( disposing );
    }

    private async Task ColumnDisplayingChanged( DataGridColumn<TItem> dataGridColumn, bool displaying )
    {
        dataGridColumn.Displaying = displaying;
        await ParentDataGrid.Refresh();
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.FieldJustifyContent( JustifyContent.Between ) );
        base.BuildClasses( builder );
    }

    protected EventCallback NewClick
        => EventCallback.Factory.Create( this, ParentDataGrid.New );

    protected EventCallback EditClick
        => EventCallback.Factory.Create( this, () => ParentDataGrid.Edit( SelectedRow ) );

    protected EventCallback DeleteClick
        => EventCallback.Factory.Create( this, () => ParentDataGrid.Delete( SelectedRow ) );

    protected EventCallback ClearFilterClick
        => EventCallback.Factory.Create( this, ParentDataGrid.ClearFilter );

    protected EventCallback SaveBatchClick
         => EventCallback.Factory.Create( this, ParentDataGrid.SaveBatch );

    protected EventCallback CancelBatchClick
        => EventCallback.Factory.Create( this, ParentDataGrid.Cancel );

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets content of button row of pager.
    /// </summary>
    public RenderFragment<ButtonRowContext<TItem>> ButtonRowTemplate
        => ParentDataGrid?.ButtonRowTemplate;

    /// <summary>
    /// Gets or sets content of column chooser of pager.
    /// </summary>
    public RenderFragment<ColumnChooserContext<TItem>> ColumnChooserTemplate
        => ParentDataGrid?.ColumnChooserTemplate;


    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    [Inject] protected ITextLocalizer<DataGrid<TItem>> Localizer { get; set; }

    /// <summary>
    /// Gets or sets the pagination context.
    /// </summary>
    [Parameter] public PaginationContext<TItem> PaginationContext { get; set; }

    /// <summary>
    /// Gets or sets the pagination templates.
    /// </summary>
    [Parameter]
    public PaginationTemplates<TItem> PaginationTemplates
    {
        get
        {
            return new()
            {
                PageButtonTemplate = PageButtonTemplate,
                FirstPageButtonTemplate = FirstPageButtonTemplate,
                LastPageButtonTemplate = LastPageButtonTemplate,
                PreviousPageButtonTemplate = PreviousPageButtonTemplate,
                NextPageButtonTemplate = NextPageButtonTemplate,
                ItemsPerPageTemplate = ItemsPerPageTemplate,
                TotalItemsShortTemplate = TotalItemsShortTemplate,
                TotalItemsTemplate = TotalItemsTemplate,
                PageSelectorTemplate = PageSelectorTemplate,
                PageSizesTemplate = PageSizesTemplate
            };
        }
        set
        {
            PageButtonTemplate = value.PageButtonTemplate;
            FirstPageButtonTemplate = value.FirstPageButtonTemplate;
            LastPageButtonTemplate = value.LastPageButtonTemplate;
            PreviousPageButtonTemplate = value.PreviousPageButtonTemplate;
            NextPageButtonTemplate = value.NextPageButtonTemplate;
            ItemsPerPageTemplate = value.ItemsPerPageTemplate;
            TotalItemsShortTemplate = value.TotalItemsShortTemplate;
            TotalItemsTemplate = value.TotalItemsTemplate;
            PageSelectorTemplate = value.PageSelectorTemplate;
            PageSizesTemplate = value.PageSizesTemplate;
        }
    }

    /// <summary>
    /// Gets or sets currently selected row.
    /// </summary>
    [Parameter] public TItem SelectedRow { get; set; }

    /// <summary>
    /// Gets or sets content of page buttons of pager.
    /// </summary>
    [Parameter] public RenderFragment<PageButtonContext> PageButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of first button of pager.
    /// </summary>
    [Parameter] public RenderFragment FirstPageButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of last button of pager.
    /// </summary>
    [Parameter] public RenderFragment LastPageButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of previous button of pager.
    /// </summary>
    [Parameter] public RenderFragment PreviousPageButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of next button of pager.
    /// </summary>
    [Parameter] public RenderFragment NextPageButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of items per page of grid.
    /// </summary>
    [Parameter] public RenderFragment ItemsPerPageTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of the page selector. The selector is only displayed under the tablets breakpoint. You will have to construct it using the provided pagination context.
    /// </summary>
    [Parameter] public RenderFragment<PaginationContext<TItem>> PageSelectorTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of the page sizes selector. You will have to construct it using the provided pagination context.
    /// </summary>
    [Parameter] public RenderFragment<PaginationContext<TItem>> PageSizesTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of total items grid for small devices.
    /// </summary>
    [Parameter] public RenderFragment<PaginationContext<TItem>> TotalItemsShortTemplate { get; set; }

    /// <summary>
    /// Gets or sets content of total items grid.
    /// </summary>
    [Parameter] public RenderFragment<PaginationContext<TItem>> TotalItemsTemplate { get; set; }

    [Parameter] public EventCallback<string> OnPaginationItemClick { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="DataGrid{TItem}"/> of the this component.
    /// </summary>
    [CascadingParameter] public DataGrid<TItem> ParentDataGrid { get; set; }

    #endregion
}