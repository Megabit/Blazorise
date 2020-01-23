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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarToggler() );
            builder.Append( ClassProvider.BarTogglerCollapsed( IsOpen ) );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            // NOTE: is this right?
            if ( Clicked == null )
                ParentBar?.Toggle();
            else
                Clicked?.Invoke();
        }

        protected override void OnInitialized()
        {
            ParentBar?.Hook( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        [Parameter]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] protected BaseBar ParentBar { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
