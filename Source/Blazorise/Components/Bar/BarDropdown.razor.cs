#region Using directives
using System.Threading.Tasks;
using Blazorise.Stores;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The dropdown menu, which can include bar items and dividers.
    /// </summary>
    public partial class BarDropdown : BaseComponent
    {
        #region Members

        private BarItemStore parentBarItemStore;

        private BarDropdownStore parentBarDropdownStore;

        private BarDropdownStore store = new BarDropdownStore
        {
            NestedIndex = 1
        };

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdown( Store.Mode ) );
            builder.Append( ClassProvider.BarDropdownShow( Store.Mode ), Store.Visible );

            base.BuildClasses( builder );
        }

        protected override Task OnInitializedAsync()
        {
            // link to the parent component
            ParentBarItem?.NotifyBarDropdownInitialized( this );

            return base.OnInitializedAsync();
        }

        public override Task SetParametersAsync( ParameterView parameters )
        {
            // This is needed for the two-way binding to work properly.
            // Otherwise the internal value would not be set in the right order.
            if ( parameters.TryGetValue<bool>( nameof( Visible ), out var newVisible ) )
            {
                store = store with { Visible = newVisible };
            }

            return base.SetParametersAsync( parameters );
        }

        internal void Show()
        {
            if ( Visible )
                return;

            Visible = true;

            StateHasChanged();
        }

        internal void Hide()
        {
            if ( !Visible )
                return;

            Visible = false;

            StateHasChanged();
        }

        internal void Toggle()
        {
            // Don't allow Toggle when menu is in a vertical "popout" style mode.
            // This will be handled by mouse over actions below.
            if ( ParentBarItemStore.Mode != BarMode.Horizontal && !Store.IsInlineDisplay )
                return;

            Visible = !Visible;

            StateHasChanged();
        }

        public void OnMouseEnter()
        {
            if ( ParentBarItemStore.Mode == BarMode.Horizontal || Store.IsInlineDisplay )
                return;

            Show();
        }

        public void OnMouseLeave()
        {
            if ( ParentBarItemStore.Mode == BarMode.Horizontal || Store.IsInlineDisplay )
                return;

            Hide();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets the reference to the store object for this <see cref="BarDropdown"/> component.
        /// </summary>
        protected BarDropdownStore Store => store;

        /// <summary>
        /// Gets the <see cref="Visible"/> flag represented as a string.
        /// </summary>
        protected string VisibleString => Store.Visible.ToString().ToLower();

        /// <summary>
        /// Sets a value indicating whether the dropdown menu and all its child controls are visible.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => store.Visible;
            set
            {
                // prevent dropdown from calling the same code multiple times
                if ( value == store.Visible )
                    return;

                store = store with { Visible = value };

                VisibleChanged.InvokeAsync( value );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the component visibility changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// Cascaded <see cref="BarItem"/> component in which this <see cref="BarDropdown"/> is placed.
        /// </summary>
        [CascadingParameter] protected BarItem ParentBarItem { get; set; }

        [CascadingParameter]
        protected BarItemStore ParentBarItemStore
        {
            get => parentBarItemStore;
            set
            {
                if ( parentBarItemStore == value )
                    return;

                parentBarItemStore = value;

                store = store with { Mode = parentBarItemStore.Mode, BarVisible = parentBarItemStore.BarVisible };

                if ( !store.BarVisible )
                    Visible = false;

                DirtyClasses();
            }
        }

        [CascadingParameter]
        protected BarDropdownStore ParentBarDropdownStore
        {
            get => parentBarDropdownStore;
            set
            {
                if ( parentBarDropdownStore == value )
                    return;

                parentBarDropdownStore = value;

                store = store with { NestedIndex = parentBarDropdownStore.NestedIndex + 1 };

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
