﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseBreadcrumbLink : BaseComponent
    {
        #region Members

        private bool isDisabled;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BreadcrumbLink() );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
        }

        #endregion

        #region Properties

        protected bool IsParentBreadcrumbItemActive => ParentBreadcrumbItem?.IsActive == true;

        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        /// <summary>
        /// Link to the destination page.
        /// </summary>
        [Parameter] public string To { get; set; }

        /// <summary>
        /// URL matching behavior for a link.
        /// </summary>
        [Parameter] public Match Match { get; set; } = Match.All;

        /// <summary>
        /// Defines the title of a link, which appears to the user as a tooltip.
        /// </summary>
        [Parameter] public string Title { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [CascadingParameter] public BaseBreadcrumbItem ParentBreadcrumbItem { get; set; }

        #endregion
    }
}
