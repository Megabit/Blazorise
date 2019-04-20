#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseDropdownToggle : BaseComponent, ICloseActivator
    {
        #region Members

        private bool isOpen;

        private bool actAsButton;

        private bool isSplit;

        private bool isRegistered;

        #endregion

        #region Methods

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                // make sure to unregister listener
                if ( isRegistered )
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }
            }

            base.Dispose( disposing );
        }

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.DropdownToggle(), () => !ActAsButton )
                .If( () => ClassProvider.DropdownToggleSplit(), () => !ActAsButton && IsSplit );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            Dropdown?.Hook( this );

            base.OnInit();
        }


        protected void ClickHandler()
        {
            Dropdown?.Toggle();
        }

        public bool SafeToClose( string elementId )
        {
            return true;
        }

        public void Close()
        {
            Dropdown?.Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the toggle button is clicked.
        /// </summary>
        [Parameter] protected Action<bool> Toggle { get; set; }

        /// <summary>
        /// Gets or sets the dropdown color.
        /// </summary>
        [Parameter] protected Color Color { get; set; } = Color.None;

        /// <summary>
        /// Gets or sets the dropdown size.
        /// </summary>
        [Parameter] protected ButtonSize Size { get; set; } = ButtonSize.None;

        /// <summary>
        /// Handles the visibility of dropdown toggle.
        /// </summary>
        [Parameter]
        internal bool IsOpen
        {
            get => isOpen;
            set
            {
                isOpen = value;

                if ( isOpen )
                {
                    isRegistered = true;

                    JSRunner.RegisterClosableComponent( this );
                }
                else
                {
                    isRegistered = false;

                    JSRunner.UnregisterClosableComponent( this );
                }

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Button outline.
        /// </summary>
        [Parameter] protected bool IsOutline { get; set; }

        /// <summary>
        /// Act as ordinary button instead of dropdown toggle.
        /// </summary>
        [Parameter]
        protected bool ActAsButton
        {
            get => actAsButton;
            set
            {
                actAsButton = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Handles the visibility of split button.
        /// </summary>
        [Parameter]
        protected bool IsSplit
        {
            get => isSplit;
            set
            {
                isSplit = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseDropdown Dropdown { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
