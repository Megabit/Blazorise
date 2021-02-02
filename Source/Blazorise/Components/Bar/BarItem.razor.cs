#region Using directives
using System.Threading.Tasks;
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Container for <see cref="BarLink"/> or <see cref="BarDropdown"/> components.
    /// </summary>
    public partial class BarItem : BaseComponent
    {
        #region Members

        /// <summary>
        /// Reference to the <see cref="Bar"/> store object.
        /// </summary>
        private BarStore parentStore;

        /// <summary>
        /// Holds the state for this <see cref="BarItem"/>.
        /// </summary>
        private BarItemStore store = new BarItemStore
        {
            Mode = BarMode.Horizontal,
        };

        /// <summary>
        /// Reference to the <see cref="BarDropdown"/> placed inside of this <see cref="BarItem"/>.
        /// </summary>
        private BarDropdown barDropdown;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                if ( HasDropdown )
                {
                    DirtyClasses();

                    StateHasChanged();
                }
            }

            return base.OnAfterRenderAsync( firstRender );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarItem( Store.Mode ) );
            builder.Append( ClassProvider.BarItemActive( Store.Mode ), Store.Active );
            builder.Append( ClassProvider.BarItemDisabled( Store.Mode ), Store.Disabled );
            builder.Append( ClassProvider.BarItemHasDropdown( Store.Mode ), HasDropdown );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Notifies this <see cref="BarItem"/> that one of it's child component is a <see cref="BarDropdown"/>.
        /// </summary>
        /// <param name="barDropdown">Reference to the <see cref="BarDropdown"/> placed inside of this <see cref="BarItem"/>.</param>
        internal void NotifyBarDropdownInitialized( BarDropdown barDropdown )
        {
            this.barDropdown = barDropdown;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the reference to the store for this <see cref="BarItem"/> component.
        /// </summary>
        protected BarItemStore Store => store;

        /// <summary>
        /// True if <see cref="BarDropdown"/> component is placed inside of this <see cref="BarItem"/>.
        /// </summary>
        protected bool HasDropdown => barDropdown != null;

        /// <summary>
        /// Gets or sets the flag to indicate if <see cref="BarItem"/> is active, or focused.
        /// </summary>
        [Parameter]
        public bool Active
        {
            get => store.Active;
            set
            {
                store = store with { Active = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the disabled state to make <see cref="BarItem"/> inactive.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => store.Disabled;
            set
            {
                store = store with { Disabled = value };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Cascaded <see cref="Bar"/> component store.
        /// </summary>
        [CascadingParameter]
        protected BarStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                store = store with { Mode = parentStore.Mode, BarVisible = parentStore.Visible };

                DirtyClasses();
            }
        }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="BarItem"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
