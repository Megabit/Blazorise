#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Stores;
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

        private ListGroupStore parentListGroupStore;

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
        /// <returns>Returns the awaitable task.</returns>
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
        protected bool Active => parentListGroupStore.Mode == ListGroupMode.Selectable && parentListGroupStore.SelectedItem == Name;

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

        [CascadingParameter]
        protected ListGroupStore ParentListGroupStore
        {
            get => parentListGroupStore;
            set
            {
                if ( parentListGroupStore == value )
                    return;

                parentListGroupStore = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the parent list group.
        /// </summary>
        [CascadingParameter] protected ListGroup ParentListGroup { get; set; }

        /// <summary>
        /// Gets or sets the component child content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
