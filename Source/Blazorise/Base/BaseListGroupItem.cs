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

        protected override void OnInit()
        {
            ParentListGroup?.LinkItem( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the item name.
        /// </summary>
        [Parameter] internal protected string Name { get; set; }

        [Parameter]
        internal protected bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected bool IsDisabled
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
        [Parameter] protected Action Clicked { get; set; }

        [CascadingParameter] protected BaseListGroup ParentListGroup { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
