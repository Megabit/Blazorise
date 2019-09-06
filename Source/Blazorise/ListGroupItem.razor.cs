#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseListGroupItem : BaseComponent
    {
        #region Members

        private bool isActive;

        private bool isDisabled;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ListGroupItem() )
                .If( () => ClassProvider.ListGroupItemActive(), () => IsActive )
                .If( () => ClassProvider.ListGroupItemDisabled(), () => IsDisabled );

            base.RegisterClasses();
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
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                isDisabled = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] public BaseListGroup ParentListGroup { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
