#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public class BaseBarDropdownMenu : BaseComponent
    {
        #region Members

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BarDropdownMenu() )
                .If( () => ClassProvider.BarDropdownMenuShow(), () => IsOpen );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            BarDropdown?.Hook( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        [Parameter]
        internal bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseBarDropdown BarDropdown { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
