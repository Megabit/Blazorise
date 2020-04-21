#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarItem : BaseComponent
    {
        #region Members

        private bool active;

        private bool disabled;

        private BarDropdown barDropdown;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarItem() );
            builder.Append( ClassProvider.BarItemActive(), Active );
            builder.Append( ClassProvider.BarItemDisabled(), Disabled );
            builder.Append( ClassProvider.BarItemHasDropdown(), HasDropdown );
            builder.Append( ClassProvider.BarItemHasDropdownShow(), HasDropdown && barDropdown?.Visible == true );

            base.BuildClasses( builder );
        }

        internal void Hook( BarDropdown barDropdown )
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

        protected bool HasDropdown => barDropdown != null;

        [Parameter]
        public bool Active
        {
            get => active;
            set
            {
                active = value;

                DirtyClasses();
            }
        }

        [Parameter]
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Icon name.
        /// </summary>
        [Parameter] public object IconName { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
