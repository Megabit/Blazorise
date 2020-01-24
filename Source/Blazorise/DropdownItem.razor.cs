#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class DropdownItem : BaseComponent
    {
        #region Members

        private bool isActive;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DropdownItem() );
            builder.Append( ClassProvider.DropdownItemActive(), IsActive );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            Clicked.InvokeAsync( Value );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds the item value.
        /// </summary>
        [Parameter] public object Value { get; set; }

        /// <summary>
        /// Indicate the currently active item.
        /// </summary>
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

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public EventCallback<object> Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
