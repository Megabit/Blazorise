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

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TabItem() );
            builder.Append( ClassProvider.TabItemActive(), IsActive );

            LinkClassMapper
                .Add( () => ClassProvider.TabLink() )
                .If( () => ClassProvider.TabLinkActive(), () => IsActive );

            base.BuildClasses( builder );
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

                Dirty();
                LinkClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Occurs when the item is clicked.
        /// </summary>
        [Parameter] public Action Clicked { get; set; }

        [CascadingParameter] public BaseTabs ParentTabs { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
