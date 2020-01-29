#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ListGroupItem : BaseComponent
    {
        #region Members

        private bool active;

        private bool disabled;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ListGroupItem() );
            builder.Append( ClassProvider.ListGroupItemActive(), Active );
            builder.Append( ClassProvider.ListGroupItemDisabled(), Disabled );

            base.BuildClasses( builder );
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
            ParentListGroup?.SelectItem( Name );
        }

        protected override void OnInitialized()
        {
            ParentListGroup?.LinkItem( this );

            base.OnInitialized();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the item name.
        /// </summary>
        [Parameter] public string Name { get; set; }

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
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] protected ListGroup ParentListGroup { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
