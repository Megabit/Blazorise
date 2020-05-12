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

            base.OnInitialized();
        }

        public void Show()
        {
            Visible = true;

            ParentBarItem?.MenuChanged();

            StateHasChanged();
        }

        public void Hide()
        {
            Visible = false;

            ParentBarItem?.MenuChanged();

            StateHasChanged();
        }

        public void Toggle()
        {
            Visible = !Visible;

            ParentBarItem?.MenuChanged();

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected BarDropdownStore Store => store;

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its child controls are displayed.
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
                store.IconName = parentStore.IconName;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
