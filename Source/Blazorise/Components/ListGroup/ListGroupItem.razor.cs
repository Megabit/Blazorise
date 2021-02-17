﻿#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A container component that is placed inside of an <see cref="ListGroup"/>.
    /// </summary>
    public partial class ListGroupItem : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the reference to the parent group state object.
        /// </summary>
        private ListGroupState parentListGroupState;

        /// <summary>
        /// Flag to indicate item disabled state.
        /// </summary>
        private bool disabled;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ListGroupItem() );
            builder.Append( ClassProvider.ListGroupItemActive(), Active );
            builder.Append( ClassProvider.ListGroupItemDisabled(), Disabled );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the item onclick event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected async Task ClickHandler()
        {
            if ( Disabled )
                return;

            ParentListGroup?.SelectItem( Name );

            await Clicked.InvokeAsync( null );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the string representing the disabled state.
        /// </summary>
        protected string DisabledString => Disabled.ToString().ToLowerInvariant();

        /// <summary>
        /// Gets the flag indicating the item is selected.
        /// </summary>
        protected bool Active => parentListGroupState.Mode == ListGroupMode.Selectable && parentListGroupState.SelectedItem == Name;

        /// <summary>
        /// Defines the item name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Makes the item to make it appear disabled.
        /// </summary>
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
        /// Cascaded <see cref="ListGroup"/> component state object.
        /// </summary>
        [CascadingParameter]
        protected ListGroupState ParentListGroupState
        {
            get => parentListGroupState;
            set
            {
                if ( parentListGroupState == value )
                    return;

                parentListGroupState = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Cascaded parent <see cref="ListGroup"/>.
        /// </summary>
        [CascadingParameter] protected ListGroup ParentListGroup { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ListGroupItem"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
