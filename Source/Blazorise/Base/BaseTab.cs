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
    public abstract class BaseTab : BaseComponent
    {
        #region Members

        private bool isActive;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TabItem() )
                .If( () => ClassProvider.TabItemActive(), () => IsActive );

            LinkClassMapper
                .Add( () => ClassProvider.TabLink() )
                .If( () => ClassProvider.TabLinkActive(), () => IsActive );

            base.RegisterClasses();
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
            ParentTabs?.SelectTab( Name );
        }

        protected override void OnInit()
        {
            ParentTabs?.LinkTab( this );

            base.OnInit();
        }

        #endregion

        #region Properties

        protected ClassMapper LinkClassMapper { get; } = new ClassMapper();

        /// <summary>
        /// Defines the tab name.
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
                LinkClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] protected Action Clicked { get; set; }

        [CascadingParameter] protected BaseTabs ParentTabs { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
