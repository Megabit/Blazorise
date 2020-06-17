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
    public partial class BarDropdown : BaseComponent
    {
        #region Members

        private BarItemStore parentStore;

        private BarDropdownStore store;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarDropdown( Store.Mode ) );
            builder.Append( ClassProvider.BarDropdownShow( Store.Mode ), Store.Visible && Store.Mode != BarMode.VerticalSmall );

            base.BuildClasses( builder );
        }

        protected override Task OnInitializedAsync()
        {
            // link to the parent component
            ParentBarItem?.Hook( this );

            return base.OnInitializedAsync();
        }

        public override Task SetParametersAsync( ParameterView parameters )
        {
            // This is needed for the two-way binding to work properly.
            // Otherwise the internal value would not be set in the right order.
            if ( parameters.TryGetValue<bool>( nameof( Visible ), out var newVisible ) )
            {
                store.Visible = newVisible;
            }

            return base.SetParametersAsync( parameters );
        }

        internal void Show()
        {
            Visible = true;

            StateHasChanged();
        }

        internal void Hide()
        {
            Visible = false;

            StateHasChanged();
        }

        internal void Toggle()
        {
            if ( Store.Mode == BarMode.VerticalSmall )
                return;

            Visible = !Visible;

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected BarDropdownStore Store => store;

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

                store.Visible = value;

                VisibleChanged.InvokeAsync( value );

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the component visibility changes.
        /// </summary>
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        [CascadingParameter] protected BarItem ParentBarItem { get; set; }

        [CascadingParameter]
        protected BarItemStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                store.Mode = parentStore.Mode;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
