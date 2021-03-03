#region Using directives
using System.Threading.Tasks;
using Blazorise.States;
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

        private BarItemState parentBarItemState;

        private BarDropdownState parentBarDropdownState;

        private BarDropdownState state = new()
        {
            NestedIndex = 1
        };

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdown( State.Mode ) );
            builder.Append( ClassProvider.BarDropdownShow( State.Mode ), State.Visible );

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
                state = state with { Visible = newVisible };
            }

            return base.SetParametersAsync( parameters );
        }

        internal void Show()
        {
            if ( Visible )
                return;

            Visible = true;

            InvokeAsync( StateHasChanged );
        }

        internal void Hide()
        {
            if ( !Visible )
                return;

            Visible = false;

            InvokeAsync( StateHasChanged );
        }

        internal void Toggle()
        {
            // Don't allow Toggle when menu is in a vertical "popout" style mode.
            // This will be handled by mouse over actions below.
            if ( ParentBarItemState.Mode != BarMode.Horizontal && !State.IsInlineDisplay )
                return;

            Visible = !Visible;

            InvokeAsync( StateHasChanged );
        }

        public void OnMouseEnter()
        {
            if ( ParentBarItemState.Mode == BarMode.Horizontal || State.IsInlineDisplay )
                return;

            Show();
        }

        public void OnMouseLeave()
        {
            if ( ParentBarItemState.Mode == BarMode.Horizontal || State.IsInlineDisplay )
                return;

            Hide();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        /// <summary>
        /// Gets the reference to the state object for this <see cref="BarDropdown"/> component.
        /// </summary>
        protected BarDropdownState State => state;

        /// <summary>
        /// Gets the <see cref="Visible"/> flag represented as a string.
        /// </summary>
        protected string VisibleString => State.Visible.ToString().ToLower();

        /// <summary>
        /// Sets a value indicating whether the dropdown menu and all its child controls are visible.
        /// </summary>
        [Parameter]
        public bool Visible
        {
            get => state.Visible;
            set
            {
                // prevent dropdown from calling the same code multiple times
                if ( value == state.Visible )
                    return;

                state = state with { Visible = value };

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
        protected BarItemState ParentBarItemState
        {
            get => parentBarItemState;
            set
            {
                if ( parentBarItemState == value )
                    return;

                parentBarItemState = value;

                state = state with { Mode = parentBarItemState.Mode, BarVisible = parentBarItemState.BarVisible };

                if ( !state.BarVisible )
                    Visible = false;

                DirtyClasses();
            }
        }

        [CascadingParameter]
        protected BarDropdownState ParentBarDropdownState
        {
            get => parentBarDropdownState;
            set
            {
                if ( parentBarDropdownState == value )
                    return;

                parentBarDropdownState = value;

                state = state with { NestedIndex = parentBarDropdownState.NestedIndex + 1 };

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
