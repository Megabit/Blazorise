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
    public partial class BarItem : BaseComponent
    {
        #region Members

        private BarStore parentStore;

        private BarItemStore store;

        private BarDropdown barDropdown;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarItem( Store.Mode ) );
            builder.Append( ClassProvider.BarItemActive( Store.Mode ), Store.Active );
            builder.Append( ClassProvider.BarItemDisabled( Store.Mode ), Store.Disabled );
            builder.Append( ClassProvider.BarItemHasDropdown( Store.Mode ), HasDropdown );

            base.BuildClasses( builder );
        }

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

        internal void Hook( BarDropdown barDropdown )
        {
            this.barDropdown = barDropdown;
        }

        #endregion

        #region Properties

        protected BarItemStore Store => store;

        protected bool HasDropdown => barDropdown != null;

        [Parameter]
        public bool Active
        {
            get => store.Active;
            set
            {
                store.Active = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool Disabled
        {
            get => store.Disabled;
            set
            {
                store.Disabled = value;

                DirtyClasses();
            }
        }

        [CascadingParameter]
        protected BarStore ParentStore
        {
            get => parentStore;
            set
            {
                if ( parentStore == value )
                    return;

                parentStore = value;

                store.Mode = parentStore.Mode;
                store.BarVisible = parentStore.Visible;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
