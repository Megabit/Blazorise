#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseDropdownToggle : BaseComponent
    {
        #region Members

        private bool actAsButton;

        private bool isSplit;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.DropdownToggle(), () => !ActAsButton )
                .If( () => ClassProvider.DropdownToggleSplit(), () => !ActAsButton && IsSplit );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Dropdown?.Toggle();
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
        [Parameter] protected Size Size { get; set; } = Size.None;

        /// <summary>
        /// Handles the visibility of dropdown items.
        /// </summary>
        [Parameter] protected bool IsOpen { get; set; }

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

        [CascadingParameter] protected Dropdown Dropdown { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
