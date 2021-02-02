#region Using directives
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// List groups are a flexible and powerful component for displaying a series of content.
    /// </summary>
    public partial class ListGroup : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the state of this list group.
        /// </summary>
        private ListGroupStore store = new ListGroupStore
        {
            Mode = ListGroupMode.Static,
        };

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ListGroup() );
            builder.Append( ClassProvider.ListGroupFlush(), Flush );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Sets the active item by the name.
        /// </summary>
        /// <param name="name"></param>
        public void SelectItem( string name )
        {
            SelectedItem = name;

            InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list group store object.
        /// </summary>
        protected ListGroupStore Store => store;

        /// <summary>
        /// Remove some borders and rounded corners to render list group items edge-to-edge in a parent container (e.g., cards).
        /// </summary>
        [Parameter]
        public bool Flush
        {
            get => store.Flush;
            set
            {
                store = store with { Flush = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the list-group behaviour mode.
        /// </summary>
        [Parameter]
        public ListGroupMode Mode
        {
            get => store.Mode;
            set
            {
                store = store with { Mode = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets currently selected item name.
        /// </summary>
        [Parameter]
        public string SelectedItem
        {
            get => store.SelectedItem;
            set
            {
                // prevent item from calling the same code multiple times
                if ( value == store.SelectedItem )
                    return;

                store = store with { SelectedItem = value };

                // raise the SelectedItemChanged notification                
                SelectedItemChanged.InvokeAsync( store.SelectedItem );

                DirtyClasses();
            }
        }

        /// <summary>
        /// An event raised when <see cref="SelectedItem"/> is changed.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedItemChanged { get; set; }

        /// <summary>
        /// Gets or sets the component child content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
