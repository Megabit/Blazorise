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
            builder.Append( ClassProvider.BarDropdownShow( Store.Mode ), Store.Visible );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            ParentBarItem?.Hook( this );

            if ( parentStore.Mode != BarMode.VerticalSmall )
                Visible = Open;

            base.OnInitialized();
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
        /// Sets a value indicating whether the control and all its child controls are displayed by default
        /// </summary>
        [Parameter]
        public bool Open { get; set; }

        public bool Visible
        {
            get => store.Visible;
            set
            {
                // prevent dropdown from calling the same code multiple times
                if ( value == store.Visible )
                    return;

                store.Visible = value;

                DirtyClasses();
            }
        }

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

                // Hack for AntDesign..
                if ( store.Mode == BarMode.VerticalSmall )
                    Visible = false;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
