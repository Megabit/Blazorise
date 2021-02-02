#region Using directives
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// A container for page numbers links.
    /// </summary>
    public partial class PaginationItem : BaseComponent
    {
        #region Members

        /// <summary>
        /// Holds the state of this pagination item.
        /// </summary>
        private PaginationItemStore store = new();

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.PaginationItem() );
            builder.Append( ClassProvider.PaginationItemActive(), Active );
            builder.Append( ClassProvider.PaginationItemDisabled(), Disabled );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicate the currently active page.
        /// </summary>
        [Parameter]
        public bool Active
        {
            get => store.Active;
            set
            {
                if ( value == store.Active )
                    return;

                store = store with { Active = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Used for links that appear un-clickable.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => store.Disabled;
            set
            {
                if ( value == store.Disabled )
                    return;

                store = store with { Disabled = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="PaginationItem"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
