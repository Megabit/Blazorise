﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarLink : BaseComponent
    {
        #region Members

        private bool disabled;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarLink() );
            builder.Append( ClassProvider.BarLinkDisabled(), Disabled );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Page address.
        /// </summary>
        [Parameter] public string To { get; set; }

        [Parameter] public Match Match { get; set; } = Match.All;

        [Parameter] public string Title { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
