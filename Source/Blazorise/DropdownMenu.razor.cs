#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseDropdownMenu : BaseComponent
    {
        #region Members

        private bool isOpen;

        private bool isRightAligned;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.DropdownMenu() )
                .If( () => ClassProvider.DropdownMenuShow(), () => IsOpen )
                .If( () => ClassProvider.DropdownMenuRight(), () => IsRightAligned );

            BodyClassMapper
                .Add( () => ClassProvider.DropdownMenuBody() );

            base.RegisterClasses();
        }

        protected override void OnInitialized()
        {
            // link to the parent component
            Dropdown?.Hook( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        protected ClassMapper BodyClassMapper { get; private set; } = new ClassMapper();

        /// <summary>
        /// Handles the visibility of dropdown menu.
        /// </summary>
        [Parameter]
        internal protected bool IsOpen
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
        protected bool IsRightAligned
        {
            get => isRightAligned;
            set
            {
                isRightAligned = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseDropdown Dropdown { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
