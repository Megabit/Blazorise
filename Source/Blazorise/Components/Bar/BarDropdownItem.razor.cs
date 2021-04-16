﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A menu item for the <see cref="BarDropdownMenu"/> component.
    /// </summary>
    public partial class BarDropdownItem : BaseComponent
    {
        #region Members

        private BarDropdownState parentDropdownState;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdownItem( ParentDropdownState.Mode ) );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void BuildStyles( StyleBuilder builder )
        {
            base.BuildStyles( builder );

            builder.Append( $"padding-left: { Indentation * ( ParentDropdownState.NestedIndex + 1 ) }rem", ParentDropdownState.IsInlineDisplay );
        }

        /// <summary>
        /// Handles the item onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ClickHandler()
        {
            return Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback Clicked { get; set; }

        /// <summary>
        /// Specifies the URL of the page the link goes to.
        /// </summary>
        [Parameter] public string To { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the linked document.
        /// </summary>
        [Parameter] public Target Target { get; set; } = Target.None;

        /// <summary>
        /// URL matching behavior for a link.
        /// </summary>
        [Parameter] public Match Match { get; set; } = Match.All;

        /// <summary>
        /// Specify extra information about the link element.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Determines how much left padding will be applied to the dropdown item. (in rem unit)
        /// </summary>
        [Parameter] public double Indentation { get; set; } = 1.5d;

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BarDropdownItem"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Cascaded parent <see cref="BarDropdown"/> state.
        /// </summary>
        [CascadingParameter]
        protected BarDropdownState ParentDropdownState
        {
            get => parentDropdownState;
            set
            {
                if ( parentDropdownState == value )
                    return;

                parentDropdownState = value;

                DirtyClasses();
                DirtyStyles();
            }
        }

        #endregion
    }
}
