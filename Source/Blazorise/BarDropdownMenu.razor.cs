#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public class BaseBarDropdownMenu : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isRightAligned;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BarDropdownMenu() )
                .If( () => ClassProvider.BarDropdownMenuShow(), () => IsOpen )
                .If( () => ClassProvider.BarDropdownMenuRight(), () => IsRightAligned );

            base.RegisterClasses();
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            BarDropdown?.Hook( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Handles the visibility of dropdown menu.
        /// </summary>
        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Right aligned dropdown menu.
        /// </summary>
        [Parameter]
        public bool IsRightAligned
        {
            get => isRightAligned;
            set
            {
                isRightAligned = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] public BaseBarDropdown BarDropdown { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
