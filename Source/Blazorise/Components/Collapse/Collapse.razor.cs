﻿#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Toggle visibility of almost any content on your pages in a vertically collapsing container.
    /// </summary>
    public partial class Collapse : BaseComponent
    {
        #region Members

        private bool visible;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Collapse() );
            builder.Append( ClassProvider.CollapseActive( Visible ) );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Toggles the collapse visibility state.
        /// </summary>
        public void Toggle()
        {
            Visible = !Visible;
            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the collapse visibility state.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => visible;
            set
            {
                visible = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Collapse"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
