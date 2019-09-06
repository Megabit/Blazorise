#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseBarItem : BaseComponent
    {
        #region Members

        private bool isActive;

        private bool isDisabled;

        private BaseBarDropdown barDropdown;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarItem() );
            builder.Append( ClassProvider.BarItemActive(), IsActive );
            builder.Append( ClassProvider.BarItemDisabled(), IsDisabled );
            builder.Append( ClassProvider.BarItemHasDropdown(), IsDropdown );
            builder.Append( ClassProvider.BarItemHasDropdownShow(), IsDropdown && barDropdown?.IsOpen == true );

            base.BuildClasses( builder );
        }

        internal void Hook( BaseBarDropdown barDropdown )
        {
            this.barDropdown = barDropdown;

            MenuChanged();
        }

        internal void MenuChanged()
        {
            DirtyClasses();

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected bool IsDropdown => barDropdown != null;

        [Parameter]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
