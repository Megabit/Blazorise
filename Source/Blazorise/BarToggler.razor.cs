#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseBarToggler : BaseComponent
    {
        #region Members

        private bool isOpen;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.BarToggler() )
                .Add( () => ClassProvider.BarTogglerCollapsed( IsOpen ) );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            // NOTE: is this right?
            if ( Clicked == null )
                ParentBar?.Toggle();
            else
                Clicked?.Invoke();
        }

        protected override void OnInit()
        {
            ParentBar?.Hook( this );

            base.OnInit();
        }

        #endregion

        #region Properties

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
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] protected Action Clicked { get; set; }

        [CascadingParameter] protected BaseBar ParentBar { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
