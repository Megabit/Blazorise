﻿#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.DataGrid
{
    partial class _DataGridPagination<TItem> : BaseComponent
    {
        #region Methods

        protected override void OnInitialized()
        {
            LocalizerService.LocalizationChanged += OnLocalizationChanged;

            base.OnInitialized();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                LocalizerService.LocalizationChanged -= OnLocalizationChanged;
            }

            base.Dispose( disposing );
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

        #endregion

        #region Properties

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
                return new PaginationTemplates<TItem>
                {
                    FirstPageButtonTemplate = FirstPageButtonTemplate,
                    LastPageButtonTemplate = LastPageButtonTemplate,
                    PreviousPageButtonTemplate = PreviousPageButtonTemplate,
                    NextPageButtonTemplate = NextPageButtonTemplate,
                    ItemsPerPageTemplate = ItemsPerPageTemplate,
                    TotalItemsShortTemplate = TotalItemsShortTemplate,
                    TotalItemsTemplate = TotalItemsTemplate
                };
            }
            set
            {
                FirstPageButtonTemplate = value.FirstPageButtonTemplate;
                LastPageButtonTemplate = value.LastPageButtonTemplate;
                PreviousPageButtonTemplate = value.PreviousPageButtonTemplate;
                NextPageButtonTemplate = value.NextPageButtonTemplate;
                ItemsPerPageTemplate = value.ItemsPerPageTemplate;
                TotalItemsShortTemplate = value.TotalItemsShortTemplate;
                TotalItemsTemplate = value.TotalItemsTemplate;
            }
        }

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
        /// Gets or sets content of total items grid for small devices.
        /// </summary>
        [Parameter] public RenderFragment<PaginationContext<TItem>> TotalItemsShortTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of total items grid.
        /// </summary>
        [Parameter] public RenderFragment<PaginationContext<TItem>> TotalItemsTemplate { get; set; }

        [Parameter]
        public Func<string, Task> OnPaginationItemClick { get; set; }

        [CascadingParameter] protected DataGrid<TItem> ParentDataGrid { get; set; }

        #endregion
    }
}
