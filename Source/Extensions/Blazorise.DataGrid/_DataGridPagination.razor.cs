﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise.DataGrid
{
    partial class _DataGridPagination : BaseComponent
    {
        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FieldJustifyContent( JustifyContent.Between ) );
            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pagination context.
        /// </summary>
        [Parameter] public PaginationContext PaginationContext { get; set; }

        [Parameter]
        public PaginationTemplates PaginationTemplates
        {
            get
            {
                return new PaginationTemplates
                {
                    FirstPageButtonTemplate = this.FirstPageButtonTemplate,
                    LastPageButtonTemplate = this.LastPageButtonTemplate,
                    PreviousPageButtonTemplate = this.PreviousPageButtonTemplate,
                    NextPageButtonTemplate = this.NextPageButtonTemplate,
                    ItemsPerPageTemplate = this.ItemsPerPageTemplate,
                    TotalItemsShortTemplate = this.TotalItemsShortTemplate,
                    TotalItemsTemplate = this.TotalItemsTemplate
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
        [Parameter] public RenderFragment<PaginationContext> TotalItemsShortTemplate { get; set; }

        /// <summary>
        /// Gets or sets content of total items grid.
        /// </summary>
        [Parameter] public RenderFragment<PaginationContext> TotalItemsTemplate { get; set; }

        [Parameter]
        public Func<string, Task> OnPaginationItemClick { get; set; }

        #endregion
    }
}
