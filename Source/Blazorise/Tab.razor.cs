#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
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

        protected override void OnInitialized()
        {
            ParentTabs?.Hook( this );

            base.OnInitialized();
        }

        protected void ClickHandler()
        {
            Clicked?.Invoke();
            ParentTabs?.SelectTab( Name );
        }

        #endregion

        #region Properties

        protected ClassMapper LinkClassMapper { get; } = new ClassMapper();

        /// <summary>
        /// Defines the tab name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Sets the active tab.
        /// </summary>
        [Parameter]
        public bool IsActive
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
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] protected BaseTabs ParentTabs { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
